using SharpDX;
using SharpDX.Windows;
using System.Collections.Generic;


namespace LeaFramework.Input
{
	public sealed class InputManager
	{
		private static InputManager instance = null;
		private static readonly object padlock = new object();
	

		public Vector2 MousePosition;

		private Dictionary<int, bool> keyList = new Dictionary<int, bool>(100);

		InputManager()
		{
			for (int i = 0; i < 100; i++)
			{
				keyList[i] = false;
			}
		}


		public void Listen(RenderForm renderForm)
		{
			renderForm.KeyDown += (object sender, System.Windows.Forms.KeyEventArgs e) =>
			{
				keyList[e.KeyValue] = true;
			};

			renderForm.KeyUp += (object sender, System.Windows.Forms.KeyEventArgs e) =>
			{
				keyList[e.KeyValue] = false;
			};

			renderForm.MouseMove += (object sender, System.Windows.Forms.MouseEventArgs e) =>
			{
				MousePosition.X = e.X;
				MousePosition.Y = e.Y;
			};
		}

		public bool IsKeyDown(Key key)
		{
			return keyList[(int)key];
		}

		

		public static InputManager Instance
		{
			get
			{
				if (instance == null)
				{
					lock (padlock)
					{
						if (instance == null)
						{
							instance = new InputManager();
						}
					}
				}
				return instance;
			}
		}

	}
}
