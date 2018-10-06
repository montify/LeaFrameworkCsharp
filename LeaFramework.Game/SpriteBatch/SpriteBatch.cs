﻿using System;
using System.Collections.Generic;
using System.Linq;
using LeaFramework.Effect;
using LeaFramework.Game.Properties;
using LeaFramework.Graphics;
using LeaFramework.Graphics.VertexStructs;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

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

		private SortMode sortMode;

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

		public void End()
		{
			if(sortMode == SortMode.Texture)
				spriteList.Sort((x, y) => x.textureID.CompareTo(y.textureID));


			CreateRenderBatches();
			RenderBatches();
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
				effect.SetTexture(rb.texture, 0, ShaderType.PixelShader);
				effect.SetVariable("ProjMatrix", "perFrame", MVP, ShaderType.GeometryShader);
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
