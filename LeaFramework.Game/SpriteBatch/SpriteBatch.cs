using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using LeaFramework.Effect;
using LeaFramework.Game.Properties;
using LeaFramework.Graphics;
using LeaFramework.Graphics.VertexStructs;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WIC;
using SharpFont;
using Bitmap = System.Drawing.Bitmap;
using Color = System.Drawing.Color;
using PixelFormat = SharpDX.WIC.PixelFormat;

// GLYPH = SpriteInfo
namespace LeaFramework.Game.SpriteBatch
{
	public class SpriteBatch : IDisposable
	{
		private GraphicsDevice graphicsDevice;
		private LeaEffect effect;
		private LeaSamplerState sampler;
		private Matrix MVP;
		private VertexBuffer vertexBuffer;
		private List<SpriteInfo> spriteList;
		private List<RenderBatch> renderBatches;
		private float t; 
		private Dictionary<uint, Glyph> GlypList = new Dictionary<uint, Glyph>();
		private SortMode sortMode;
		private Face face;
		public SpriteBatch(GraphicsDevice graphicsDevice, int maxBatchSize = 1024)
		{
			this.graphicsDevice = graphicsDevice;		

			spriteList = new List<SpriteInfo>(maxBatchSize);
			renderBatches = new List<RenderBatch>();
			var vertexShaderSource = Resources.vertexShader;
			var geometryShaderSource = Resources.geometryShader;
			var pixelShaderSource = Resources.pixelShader;

			var createInfo = new EffectCreateInfo
			{
				VertexShaderBlob = vertexShaderSource,
				GeometryShaderBlob = geometryShaderSource,
				PixelShaderBlob = pixelShaderSource,

				VertexShaderEntryPoint = "VSMain",
				GeometryShaderEntryPoint = "GSMain",
				PixelShaderEntryPoint = "PSMain"
			};

			sampler = new LeaSamplerState();
			sampler.GenerateSamplers(graphicsDevice);

			effect = new LeaEffect(graphicsDevice, createInfo);

			CreateGlyphs();
		}

		public void CreateGlyphs()
		{
			#region FreeType
			Library library = new Library();
			 face = new Face(library, "OpenSans-Regular.ttf");
			face.SetCharSize(0, 50, 72, 72);
			
			string allChars = "!#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
			//string allChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

			for (int i = 0; i < allChars.Length; i++)
			{
				var x = face.GetCharIndex(allChars[i]);
				face.LoadGlyph(x, LoadFlags.Render, LoadTarget.Normal);
				face.Glyph.RenderGlyph(RenderMode.Normal);
				
				FTBitmap bm = face.Glyph.Bitmap;
					
				
				//###################
				var gdiBitmap = bm.ToGdipBitmap(Color.White);
				var data = gdiBitmap.LockBits(new System.Drawing.Rectangle(0, 0, gdiBitmap.Width, gdiBitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

				var ret = new Texture2D(graphicsDevice.NatiDevice1.D3D11Device, new Texture2DDescription()
				{
					Width = gdiBitmap.Width,
					Height = gdiBitmap.Height,
					ArraySize = 1,
					BindFlags = BindFlags.ShaderResource,
					Usage = ResourceUsage.Immutable,
					CpuAccessFlags = CpuAccessFlags.None,
					Format = Format.B8G8R8A8_UNorm,
					MipLevels = 1,
					OptionFlags = ResourceOptionFlags.None,
					SampleDescription = new SampleDescription(1, 0),
				}, new DataRectangle(data.Scan0, data.Stride));
				gdiBitmap.UnlockBits(data);
				//###################

				var srv = new ShaderResourceView(graphicsDevice.NatiDevice1.D3D11Device, ret);
				var me = face.Glyph.Metrics;
				GlypList.Add(x, new Glyph(srv, gdiBitmap.Width, gdiBitmap.Height, face.Glyph.Metrics));

			}

			#endregion
		}

		public void Begin(SortMode sortMode = SortMode.Texture)
		{
			spriteList.Clear();
			renderBatches.Clear();
			this.sortMode = sortMode;
			// Should Calc. MVP here? When yes - when window is maximized the spriteList dont change :(
			MVP = Matrix.OrthoOffCenterLH(0, graphicsDevice.ViewPort.Width, graphicsDevice.ViewPort.Height, 0, 0, 1);
			MVP = Matrix.Transpose(MVP);
		}
		
		public void Submit(ShaderResourceView tex, Vector2 position, Vector2 size, Vector4 color)
		{
			var tmpSprite = new SpriteInfo {position = position, size = size, color = color, srv = tex, textureID = tex.GetHashCode()};

			spriteList.Add(tmpSprite);
		}


		public void SubmitString(string str, Vector2 position)
		{

			for (int i = 0; i < str.Length; i++)
			{
				var c = face.GetCharIndex(str[i]);

				var metrics = GlypList[c].metrics;
			
				var xpos = position.X + metrics.HorizontalBearingX.ToSingle();
				var ypos = position.Y -  metrics.HorizontalBearingY.ToSingle();
				
				var w = metrics.Width.ToSingle();
				var h = metrics.Height.ToSingle();

				Submit(GlypList[c].texture, new Vector2(xpos , ypos), new Vector2(w , h), SharpDX.Color.White.ToVector4());

				position.X += metrics.HorizontalAdvance.ToInt32();
			}

			
		}

		public void End()
		{
			if(sortMode == SortMode.Texture)
				spriteList.Sort((x, y) => x.textureID.CompareTo(y.textureID));


			if (spriteList.Count > 0)
			{
				CreateRenderBatches();
				RenderBatches();
			}
			
		}

		private void CreateRenderBatches()
		{
			var vertices = new List<SpriteBatchVertex>();
			
			if (spriteList.Count == 0)
				return;

			int offset = 0;
			renderBatches.Add(new RenderBatch(spriteList[0].srv, 0, 1));

			vertices.Add(new SpriteBatchVertex(spriteList[0].position, spriteList[0].size, spriteList[0].color, spriteList[0].textureID));

			offset++;

			for (int i = 1; i < spriteList.Count; i++)
			{
				if (spriteList[i].textureID != spriteList[i - 1].textureID)
					renderBatches.Add(new RenderBatch(spriteList[i].srv, offset, 1));
				else
					renderBatches.Last().numVertices += 1;

				vertices.Add(new SpriteBatchVertex(spriteList[i].position, spriteList[i].size, spriteList[i].color, spriteList[i].textureID));

				offset++;
			}

			if (vertexBuffer == null)
			{
				vertexBuffer = new VertexBuffer(graphicsDevice, true);
				vertexBuffer.CreateAndSetData(vertices.ToArray());
			}
			
			vertexBuffer.UpdateBuffer(vertices.ToArray(), 0);

			vertices.Clear();
		}

		private void RenderBatches()
		{
			graphicsDevice.SetTopology(PrimitiveTopology.PointList);

			graphicsDevice.SetVertexBuffer(vertexBuffer);

			foreach (var rb in renderBatches)
			{
				effect.SetVariable("ProjMatrix", "perFrame", MVP, ShaderType.GeometryShader);

				effect.SetTexture(rb.texture, 0, ShaderType.PixelShader);
			//	effect.SetTexture(GlypList[0].texture, 0, ShaderType.PixelShader);
				effect.SetSampler(sampler, 0, ShaderType.PixelShader);

				effect.Apply();

				graphicsDevice.Draw(rb.numVertices, rb.offset);
			}
		}

		public void Dispose()
		{
			effect.Dispose();
			sampler.Dispose();
			vertexBuffer.Dispose();
		}
	}
}
