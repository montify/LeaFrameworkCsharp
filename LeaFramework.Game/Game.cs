// This is a personal academic project. Dear PVS-Studio, please check it.

// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using LeaFramework.Game;
using LeaFramework.Graphics;
using SharpDX;
using SharpDX.Windows;

namespace LeaPlanet.src
{
	public abstract class Game : IDisposable
	{
		public RenderForm RenderForm => renderForm;

		private RenderForm renderForm;
		private GraphicsDevice graphicsDevice;
		private bool isResize;
		
		private readonly GameTimer timer = new GameTimer();
		private int frameCount;
		private float timeElapsed;

		public GraphicsDevice GraphicsDevice => graphicsDevice;
		
		public string WindowTitle { get; set; }
		public int WindowWidth{ get; set; }
		public int WindowHeight { get; set; }
		public bool IsVSyncEnable;

		public float CurrentFps { get; private set; }

		public bool IsRunning { get; set; }

		protected Game()
		{
			IsRunning = true;
			
		}

		protected void Init()
		{
			
			timer.Start();
			renderForm = new RenderForm
			{
				Text = WindowTitle,
				Width = WindowWidth,
				Height = WindowHeight				
			};

			graphicsDevice = new GraphicsDevice(RenderForm);

			RenderForm.UserResized += (sender, args) => isResize = true;
			
		}

		private void CalculateFrameRate()
		{
			timer.Tick();
			frameCount++;

			if (!(timer.TotalTime - timeElapsed >= 1.0f))
				return;

			 CurrentFps = (float)frameCount;
			var mspf = 1000.0f / CurrentFps;
			var s = WindowTitle + $" | FPS: {CurrentFps} Frame Time: {mspf} (ms)";

			RenderForm.Text = s;

			frameCount = 0;
			timeElapsed += 1.0f;
		}

		public void Run()
		{
			timer.Reset();
			Load();

			
			RenderLoop.Run(RenderForm, () => 
			{
				CalculateFrameRate();

				if (isResize)
				{
					graphicsDevice.Resize(RenderForm.ClientSize.Width, RenderForm.ClientSize.Height);
					isResize = false;
				}

				Update(timer);
				Render(timer);

				
				graphicsDevice.Present(IsVSyncEnable);

				if (!IsRunning)
					renderForm.Close();
					
			});

			Unload();
		}


			public virtual void Load() { }
			public virtual void Unload() { }
			public virtual void Update(GameTimer gameTime) { }
			public virtual void Render(GameTimer gameTime) { }


		public void Dispose()
		{
			graphicsDevice.Dispose();
			Utilities.Dispose(ref renderForm);
		}

	}
}
