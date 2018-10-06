using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using LeaFramework.Graphics;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.WIC;
using PixelFormat = SharpDX.WIC.PixelFormat;

namespace LeaFramework.Content
{
	public static class ImageReader
	{
		private static readonly ImagingFactory Imgfactory = new ImagingFactory();


		public static LeaTexture2D Load(string path, GraphicsDevice graphicsDevice)
		{
			var image = LoadImageFromFile(path);

			var dataStream = new DataStream(image.Size.Height * image.Size.Width * 4, true, true);
			image.CopyPixels(image.Size.Width * 4, dataStream);

			var rect = new DataRectangle(dataStream.DataPointer, image.Size.Width * 4);

			return new LeaTexture2D(image.Size.Width, image.Size.Height,rect, graphicsDevice, BindFlags.ShaderResource);
		}

		private static BitmapSource LoadImageFromFile(string filename)
		{
			var d = new BitmapDecoder(
				Imgfactory,
				filename,
				DecodeOptions.CacheOnDemand
			);
			
			var frame = d.GetFrame(0);
			var fconv = new FormatConverter(Imgfactory);
			fconv.Initialize(frame, PixelFormat.Format32bppPRGBA, BitmapDitherType.None, null, 0.0, BitmapPaletteType.Custom);

			return fconv;
		}
	}
}
