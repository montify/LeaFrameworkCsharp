
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using LeaFramework.Effect;
using LeaFramework.Graphics;
using SharpDX;
using SharpDX.Direct3D11;
using Texture2D = SharpDX.Direct3D11.Texture2D;

namespace LeaFramework.Content
{
	public sealed class ContentManager : IDisposable
	{
		private static ContentManager instance;
		public string RootDictionary { get; set; }
	
		private readonly Dictionary<int, object> resourceList = new Dictionary<int, object>();
		private readonly Dictionary<Type, IContentLoader> contentLoaderList = new Dictionary<Type, IContentLoader>();

		private ContentManager()
		{
			contentLoaderList.Add(typeof(LeaTexture2D), new BitMapLoader());
		}
		
		public T Load<T>(GraphicsDevice graphicsDevice, string path)
		{
			int uniqueKey = path.GetHashCode() + typeof(T).GetHashCode();

			IContentLoader contentReader;

			if (resourceList.ContainsKey(uniqueKey))
			{
				return (T)resourceList[uniqueKey];
			}
			else
			{
				if (typeof(T) == typeof(LeaTexture2D))
				{
					contentReader = new BitMapLoader();
					var image = contentReader.Load(path) as Image;

					var tex = LeaTexture2D.Create(graphicsDevice, image);

					resourceList.Add(uniqueKey, tex);
				}

				if (typeof(T) == typeof(LeaEffect))
				{

				}


				return (T)resourceList[uniqueKey];
			}
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
			foreach (var resource in resourceList)
			{

				if (resource.Value is LeaTexture2D)
				{
					var x = resource.Value as LeaTexture2D;
					x.Dispose();
				}
			}
		}

	}
}
