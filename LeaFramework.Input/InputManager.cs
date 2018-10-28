using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LeaFramework.Input
{
	public sealed class InputManager
	{
		private static InputManager instance = null;
		private static readonly object padlock = new object();
		private  bool isKeyDown;

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
