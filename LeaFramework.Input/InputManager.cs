using SharpDX;
using SharpDX.Windows;
using System.Collections.Generic;
using System.Windows.Forms;


namespace LeaFramework.Input
{
    public static class InputManager
    {
        private static int NUM_KEYCODES = 256;
        private static int[] NUM_MouseButtons = new[] { 1048576, 2097152, 4194304 }; //Left, Right, Middle MouseCode
        private static List<int> tmpList = new List<int>();
        private static List<int> currentKeys = new List<int>();
        private static List<int> downKeys = new List<int>();
        private static List<int> upKeys = new List<int>();

        private static List<int> tmpListMouse = new List<int>();
        private static List<int> currentMouse = new List<int>();
        private static List<int> downMouse = new List<int>();
        private static List<int> upMouse = new List<int>();

        //  public static RenderForm form;
        private static Vector2 mousePosition;
        private static Vector2 rawMousePosition;

        public static Vector2 MousePosition => mousePosition;
        public static Vector2 RawMousePosition => rawMousePosition;
        /// <summary>
        /// Nicht in der Update/Draw Methode aufrufen da Event
        /// </summary>

        public static void Listen(RenderForm form)
        {
            form.MouseMove += (sender, args) =>
            {
                //TODO: Mouseposition eventl. nicht in MouseMove sonder nur wenn resized wird?
                //http://gamedev.stackexchange.com/questions/101092/problem-why-the-mouse-cursor-position-is-totally-wrong-after-resizing-the-wind

                //So, a smarter way of achieving the same result 
                //is simply by multiplying your mouse position by a Vector2 scale = new Vector2(originalWidth / width, originalHeight / height);.
                var GameWidth = 1700; // the original game's width
                var GameHeight = 900; // the original game's height

                var ScaleMatrix = Matrix.Scaling((float)form.Width / GameWidth, (float)form.Height / GameHeight, 1f);
                var Scale = new Vector2(ScaleMatrix.M11, ScaleMatrix.M22);

                mousePosition.X = args.X;
                mousePosition.Y = args.Y;
                rawMousePosition = mousePosition;
                mousePosition = mousePosition / Scale;

                //   Console.WriteLine(mousePosition);
            };

            form.KeyDown += (sender, args) =>
            {
                if (!tmpList.Contains(args.KeyValue))
                    tmpList.Add(args.KeyValue);
            };

            form.KeyUp += (sender, args) =>
            {
                tmpList.Remove(args.KeyValue);
            };

            form.MouseDown += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                    if (!tmpListMouse.Contains((int)MouseButtons.Left))
                        tmpListMouse.Add((int)MouseButtons.Left);

                if (args.Button == MouseButtons.Right)
                    if (!tmpListMouse.Contains((int)MouseButtons.Right))
                        tmpListMouse.Add((int)MouseButtons.Right);

                if (args.Button == MouseButtons.Middle)
                    if (!tmpListMouse.Contains((int)MouseButtons.Middle))
                        tmpListMouse.Add((int)MouseButtons.Middle);
            };

            form.MouseUp += (sender, args) =>
            {

                if (args.Button == MouseButtons.Left)
                    tmpListMouse.Remove((int)MouseButtons.Left);

                if (args.Button == MouseButtons.Right)
                    tmpListMouse.Remove((int)MouseButtons.Right);

                if (args.Button == MouseButtons.Middle)
                    tmpListMouse.Remove((int)MouseButtons.Middle);
            };

        }

        /// <summary>
        /// In Update aufrufen
        /// </summary>
        public static void Update()
        {
            #region Mouse
            upMouse.Clear();

            foreach (var i in NUM_MouseButtons)
                if (!GetMouse((MouseButtons)i) && currentMouse.Contains(i))
                    upMouse.Add(i);

            downMouse.Clear();

            foreach (var i in NUM_MouseButtons)
                if (GetMouse((MouseButtons)i) && !currentMouse.Contains(i))
                    downMouse.Add(i);

            currentMouse.Clear();

            foreach (var i in NUM_MouseButtons)
                if (GetMouse((MouseButtons)i))
                    currentMouse.Add(i);

            #endregion

            #region Keyboard
            upKeys.Clear();

            for (var i = 0; i < NUM_KEYCODES; i++)
                if (!GetKey((Keys)i) && currentKeys.Contains(i))
                    upKeys.Add(i);

            downKeys.Clear();

            for (var i = 0; i < NUM_KEYCODES; i++)
                if (GetKey((Keys)i) && !currentKeys.Contains(i))
                    downKeys.Add(i);


            currentKeys.Clear();

            for (var i = 0; i < NUM_KEYCODES; i++)
                if (GetKey((Keys)i))
                    currentKeys.Add(i);

            #endregion
        }

        public static bool GetKey(Keys keys)
        {
            return tmpList.Contains((int)keys);
        }

        public static bool IsKeyDown(Keys key)
        {
            return downKeys.Contains((int)key);
        }

        public static bool IsKeyUp(Keys key)
        {
            return upKeys.Contains((int)key);
        }

        public static bool GetMouse(MouseButtons button)
        {
            return tmpListMouse.Contains((int)button);
        }

        public static bool IsMouseDown(MouseButtons button)
        {
            return downMouse.Contains((int)button);
        }

        public static bool IsMouseUp(MouseButtons button)
        {
            return upMouse.Contains((int)button);
        }

    }
}
