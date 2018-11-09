using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpFont;

namespace LeaFramework.Game.SpriteBatch
{
	public struct GlyphInfo
	{
		public GlyphMetrics metrics;
		public Vector2 offset;

		public GlyphInfo(GlyphMetrics metrics, Vector2 offset)
		{
			this.metrics = metrics;
			this.offset = offset;
			
		}
	}
}
