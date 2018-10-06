using System;
using LeaFramework.Game;
using LeaFramework.Graphics;
using SharpDX;
using SharpDX.Windows;

namespace LeaPlanet.src
{
	public abstract class Game : IDisposable
	{
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

		protected Game()
		{
			
			#region FreeType
			/*	Library library = new Library();
				Face face = new Face(library, "calibri.ttf");
				face.SetCharSize(0, 50, 72, 72);

				string allChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!§$%&/()=?`´^-_.,;:<>";

				for (int i = 0; i < allChars.Length; i++)
				{
					var x = face.GetCharIndex(allChars[i]);
					face.LoadGlyph(x, LoadFlags.Default, LoadTarget.Normal);
					face.Glyph.RenderGlyph(RenderMode.Normal);


					FTBitmap bm = face.Glyph.Bitmap;

					var xa = bm.ToGdipBitmap(System.Drawing.Color.AliceBlue);

					xa.Save(allChars[i]+".bmp");

				}
				*/
			#endregion
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

			graphicsDevice = new GraphicsDevice(renderForm);

			renderForm.UserResized += (sender, args) => isResize = true;

		}

		private void CalculateFrameRate()
		{
			timer.Tick();
			frameCount++;

			if (!(timer.TotalTime - timeElapsed >= 1.0f))
				return;

			var fps = (float)frameCount;
			var mspf = 1000.0f / fps;
			var s = WindowTitle + $" | FPS: {fps} Frame Time: {mspf} (ms)";

			renderForm.Text = s;

			frameCount = 0;
			timeElapsed += 1.0f;
		}

		public void Run()
		{
			timer.Reset();
			Load();

			RenderLoop.Run(renderForm, () => 
			{
				CalculateFrameRate();

				if (isResize)
				{
					graphicsDevice.Resize(renderForm.ClientSize.Width, renderForm.ClientSize.Height);
					isResize = false;
				}

				Update(timer);
				Render(timer);

				
				graphicsDevice.Present(IsVSyncEnable);
				
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
