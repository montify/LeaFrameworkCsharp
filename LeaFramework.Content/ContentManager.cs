
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using LeaFramework.Graphics;
using SharpDX;
using SharpDX.Direct3D11;
using Texture2D = SharpDX.Direct3D11.Texture2D;

namespace LeaFramework.Content
{
	public sealed class ContentManager : IDisposable
	{
		private static  ContentManager instance;
		
		public string rootDictionary { get; set; }

		private Dictionary<string, ShaderResourceView> textureList = new Dictionary<string, ShaderResourceView>();

		private ContentManager()
		{
		//	var b = ImageReader.LoadImageFromFile("Content/test.png");
		}

		public void LoadTexture(GraphicsDevice graphicsDevice, string path)
		{
			string textureName = Path.GetFileName(path);

			var tex = TextureLoader.GetSRV(graphicsDevice.NatiDevice1.D3D11Device, path);


		

			textureList.Add(textureName, tex);
		}

		public ShaderResourceView GetTexture(string name)
		{
			return textureList[name];
		}


		

		public static ContentManager Instance
		{
			get
			{
				if (instance == null)
				{

					if (instance == null)
						instance = new ContentManager();

				}

				return instance;
			}

		}

		public void Dispose()
		{
			foreach (var texture in textureList)
			{
				texture.Value.Dispose();
			}
		}

	}
}
