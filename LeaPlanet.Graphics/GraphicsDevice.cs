// This is a personal academic project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com


using System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;

namespace LeaFramework.Graphics
{
	public class GraphicsDevice : IDisposable
	{
		private readonly RenderForm renderForm;
		private NativeDevice nativeDevice;
		private RenderTargetView renderTargetView;
		private BackBuffer backBuffer;
		private DepthBuffer depthBuffer;
		public NativeDevice NatiDevice1 => nativeDevice;
		public Viewport ViewPort { get; private set; }
		private LeaDepthStencilState depthStateEnable, depthStateDisable;

		public bool IsShaderSwitchHappen;

		// HashCodes
		private VertexBuffer currentVertexBuffer;
		private IndexBuffer currentIndexBuffer;
		private PrimitiveTopology currentPrimitiveTopology;
		private InputLayout currentInputLayout;
		private VertexShader currentVertexShader;
		private GeometryShader currentGeometryShader;
		private PixelShader currentPixelShader;
		private Viewport currentViewport;
		private BlendState currentBlendState;
		private RasterizerState currentRsState;
		//private bool isDepthEnabled;

		public GraphicsDevice(RenderForm renderForm)
		{
			this.renderForm = renderForm;
		
			CreateDeviceAndSwapChain();
			CreateDepthBuffer(renderForm.ClientSize.Width, renderForm.ClientSize.Height);

			SetTargets();
		}

		private void SetTargets()
		{
			SetViewPort(ViewPort);

			nativeDevice.D3D11Device.ImmediateContext1.OutputMerger.SetRenderTargets(depthBuffer.DepthStencilView, renderTargetView);
			nativeDevice.D3D11Device.ImmediateContext1.OutputMerger.SetDepthStencilState(depthStateEnable.DepthStencilState);
		//	nativeDevice.D3D11Device.ImmediateContext1.OutputMerger.DepthStencilState = depthStateEnable.DepthStencilState;
		}

		private void CreateDeviceAndSwapChain()
		{
			nativeDevice = new NativeDevice();

			backBuffer = new BackBuffer(nativeDevice.GetFactory2(), nativeDevice.D3D11Device, renderForm);

			renderTargetView = new RenderTargetView(nativeDevice.D3D11Device, backBuffer.BackBufferTexture);
			
			ViewPort = new Viewport(0, 0, renderForm.Width, renderForm.Height, 0, 1);
		}

		private void CreateDepthBuffer(int width, int height)
		{
			depthStateEnable = new LeaDepthStencilState(nativeDevice, true);
			depthStateDisable = new LeaDepthStencilState(nativeDevice, false);

			depthBuffer = new DepthBuffer(nativeDevice, width, height);
		}

		public void Resize(int newWidth, int newHeight)
		{
			if(newWidth < 10 || newHeight < 10)
			{
				Console.WriteLine("CANT RESIZE");
				return;
			}
				
			Utilities.Dispose(ref renderTargetView);
			Utilities.Dispose(ref depthBuffer);
		
			backBuffer.Resize(newWidth, newHeight);

			// Renderview on the backbuffer
			renderTargetView = new RenderTargetView(nativeDevice.D3D11Device, backBuffer.BackBufferTexture);

			CreateDepthBuffer(newWidth, newHeight);
			
			ViewPort = new Viewport(0, 0, newWidth, newHeight, 0.0f, 1.0f);

			SetViewPort(ViewPort);

			SetTargets();

			Console.WriteLine("RESIZE END");
		}

		public void Clear(ClearFlags clearFlags, Color clearColor)
		{
			switch (clearFlags)
			{
				case ClearFlags.RenderTarget:
					nativeDevice.D3D11Device.ImmediateContext1.ClearRenderTargetView(renderTargetView, clearColor);
					break;
				case ClearFlags.DepthBuffer:
					nativeDevice.D3D11Device.ImmediateContext.ClearDepthStencilView(depthBuffer.DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
					break;
				case ClearFlags.RenderTarget | ClearFlags.DepthBuffer:
					nativeDevice.D3D11Device.ImmediateContext1.ClearRenderTargetView(renderTargetView, clearColor);
					nativeDevice.D3D11Device.ImmediateContext.ClearDepthStencilView(depthBuffer.DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
					break;
			}
		}

		public void ResetTargets()
		{
			nativeDevice.D3D11Device.ImmediateContext1.OutputMerger.ResetTargets();
		}

		public void SetViewPort(Viewport viewPort)
		{
			if (currentViewport == viewPort)
				return;

			currentViewport = viewPort;
			nativeDevice.D3D11Device.ImmediateContext1.Rasterizer.SetViewport(viewPort);
		}

		public void SetTopology(PrimitiveTopology topology)
		{
			if (topology == currentPrimitiveTopology)
				return;

			currentPrimitiveTopology = topology;
			nativeDevice.D3D11Device.ImmediateContext1.InputAssembler.PrimitiveTopology = topology;
		}

		public void SetVertexBuffer(VertexBuffer vertexBuffer)
		{

			if (vertexBuffer == currentVertexBuffer)
				return;

			currentVertexBuffer = vertexBuffer;
			nativeDevice.D3D11Device.ImmediateContext1.InputAssembler.SetVertexBuffers(0, vertexBuffer.VertexBufferBinding);
		}

		public void SetIndexBuffer(IndexBuffer indexBuffer, int offset)
		{
			if (indexBuffer == currentIndexBuffer)
				return;

			currentIndexBuffer = indexBuffer;
			nativeDevice.D3D11Device.ImmediateContext1.InputAssembler.SetIndexBuffer(indexBuffer.NativeBuffer, indexBuffer.Format, offset);
		}
		
		public void SetInputLayout(InputLayout inputLayout)
		{
			//TODO: All this current Thing works only with the same reference(adress)
			if (currentInputLayout == inputLayout)
				return;

			currentInputLayout = inputLayout;
			nativeDevice.D3D11Device.ImmediateContext1.InputAssembler.InputLayout = inputLayout;
		}

		public void Draw(int vertexCount, int startLocation)
		{
			nativeDevice.D3D11Device.ImmediateContext1.Draw(vertexCount, startLocation);
		}

		public void DrawIndexed(int indicesCount, int start, int end)
		{
			if (indicesCount == 0)
				throw new Exception("Kein gültiger Wert");

			nativeDevice.D3D11Device.ImmediateContext1.DrawIndexed(indicesCount, start, end);
		}

		public void SetVertexShader(VertexShader vertexShader)
		{
			if (currentVertexShader == vertexShader)
				return;

		
			IsShaderSwitchHappen = true;
			currentVertexShader = vertexShader;
			nativeDevice.D3D11Device.ImmediateContext1.VertexShader.Set(vertexShader);
		}

		public void SetGeometryShader(GeometryShader geometryShader)
		{
			if (currentGeometryShader == geometryShader)
				return;

			IsShaderSwitchHappen = true;
			currentGeometryShader = geometryShader;
			nativeDevice.D3D11Device.ImmediateContext1.GeometryShader.Set(geometryShader);
		}

		public void SetPixelShader(PixelShader pixelShader)
		{
			if (currentPixelShader == pixelShader)
				return;

			IsShaderSwitchHappen = true;
			currentPixelShader = pixelShader;
			nativeDevice.D3D11Device.ImmediateContext1.PixelShader.Set(pixelShader);
		}
		public void SetComputeShader(ComputeShader computeShader)
		{
			
			nativeDevice.D3D11Device.ImmediateContext1.ComputeShader.Set(computeShader);
		}

		public void SetUAV(int slot, UnorderedAccessView uav)
		{
			nativeDevice.D3D11Device.ImmediateContext1.ComputeShader.SetUnorderedAccessView(slot, uav);

		}
		public void Dispatch(int x, int y, int z)
		{
			nativeDevice.D3D11Device.ImmediateContext1.Dispatch(x, y, z);
		}

		

		public void SetblendState(BlendState blendState)
		{
			if (blendState != currentBlendState)
			{
				nativeDevice.D3D11Device.ImmediateContext1.OutputMerger.SetBlendState(blendState);
				currentBlendState = blendState;
			}
			
		}

		public void SetRasterizerState(RasterizerState rs)
		{
			if (rs != currentRsState)
			{
				nativeDevice.D3D11Device.ImmediateContext1.Rasterizer.State = rs;
				currentRsState = rs;
			}
		}

		public void IsDepthEnable(bool isDepthTesting)
		{
			if (isDepthTesting)
				nativeDevice.D3D11Device.ImmediateContext1.OutputMerger.DepthStencilState = depthStateEnable.DepthStencilState;
			else
				nativeDevice.D3D11Device.ImmediateContext1.OutputMerger.DepthStencilState = depthStateDisable.DepthStencilState;
		}


		public void Present(bool isVSyncEbable)
		{
			IsShaderSwitchHappen = false;

			backBuffer.SwapChain.Present(isVSyncEbable ? 1 : 0, PresentFlags.None);
		}

		public void Dispose()
		{
			Utilities.Dispose(ref depthBuffer);
			Utilities.Dispose(ref backBuffer);
			Utilities.Dispose(ref renderTargetView);
			Utilities.Dispose(ref nativeDevice);

            Utilities.Dispose(ref currentBlendState);
            Utilities.Dispose(ref currentGeometryShader);
            Utilities.Dispose(ref currentIndexBuffer);
            Utilities.Dispose(ref currentInputLayout);
            Utilities.Dispose(ref currentVertexShader);
            Utilities.Dispose(ref currentPixelShader);
            Utilities.Dispose(ref currentRsState);
        }
	}
}
