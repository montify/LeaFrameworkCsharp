using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaFramework.Game.SpriteBatch
{
	public static class BitmapHelper
	{
		public static void CopyRegionIntoImage(Bitmap srcBitmap, System.Drawing.Rectangle srcRegion,
			ref Bitmap destBitmap, Rectangle destRegion)
		{
			using (System.Drawing.Graphics grD = System.Drawing.Graphics.FromImage(destBitmap))
			{
				grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
			}
		}
	}
}
