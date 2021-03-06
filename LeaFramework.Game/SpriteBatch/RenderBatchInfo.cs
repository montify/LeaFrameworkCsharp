﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;

namespace LeaFramework.Game.SpriteBatch
{
	public class RenderBatchInfo
	{
		public int offset;
		public int numVertices;
		public ShaderResourceView texture;


		public RenderBatchInfo(ShaderResourceView texture, int offset, int numVertices)
		{
			this.texture = texture;
			this.offset = offset;
			this.numVertices = numVertices;
		}
	}
}
