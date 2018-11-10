using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaFramework.Graphics
{
	public class LeaDepthStencilState
	{
		private readonly DepthStencilState depthStencilState;

		public DepthStencilState DepthStencilState => depthStencilState;

		public LeaDepthStencilState(NativeDevice nativeDevice, bool isDepthTestingEnable)
		{
			var depthStencilStateDesc = new DepthStencilStateDescription
			{
				IsDepthEnabled = isDepthTestingEnable,
				DepthWriteMask = DepthWriteMask.All,
				DepthComparison = Comparison.Less,
				IsStencilEnabled = false,
				StencilWriteMask = 0xff,
				StencilReadMask = 0xff
			};


			depthStencilState = new DepthStencilState(nativeDevice.D3D11Device, depthStencilStateDesc);

		}
	}
}
