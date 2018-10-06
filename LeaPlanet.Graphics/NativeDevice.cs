using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using Device1 = SharpDX.Direct3D11.Device1;

namespace LeaFramework.Graphics
{
	public class NativeDevice : IDisposable
	{
		private Device1 device1;
		public Device1 D3D11Device => device1;

	public NativeDevice()
		{
#if DEBUG
			var flags = DeviceCreationFlags.Debug | DeviceCreationFlags.BgraSupport;
#else
			var flags = DeviceCreationFlags.BgraSupport;
#endif
			var device11_0 = new Device(DriverType.Hardware, flags, FeatureLevel.Level_11_1);

			device1 = device11_0.QueryInterfaceOrNull<Device1>();

			if (device1 == null)
				throw new NotSupportedException("SharpDX.Direct3D11.Device1 is not supported");


			//Release D3D11.0 Device
			Utilities.Dispose(ref device11_0);
		}


		public Factory2 GetFactory2()
		{
			using (var dxgi = device1.QueryInterface<SharpDX.DXGI.Device2>())
				using (var adapter = dxgi.Adapter)
					return adapter.GetParent<Factory2>();
		}

		public void Dispose()
		{
			Utilities.Dispose(ref device1);
		}
	}
}
