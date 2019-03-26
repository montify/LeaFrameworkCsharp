using LeaFramework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LeaFramework.Input;
using SharpDX;

namespace LeaFramework.PlayGround.Misc
{
    public class SpaceCam
    {
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }
        public static float FARPLANE = 1000000;
        public static float NEARPLANE = 0.001f;
        public static float FoV = MathUtil.PiOverFour;
       // public static float BBHeight;

        public Vector3Double Position { get; set; }
        public Vector3Double Target { get; private set; }

        public bool handleInput;
        public Vector2 lastMouseState;
        private GraphicsDevice device;

        public Vector3Double Up { get; set; }
        public Vector3Double Right { get; set; }
        public Vector3Double Forward { get; set; }

        public float speed;

        public BoundingFrustum Frustum;


        public SpaceCam(Vector3 Position, bool HandleInput, GraphicsDevice device, float speed)
        {
            this.device = device;
         //   BBHeight = device.PresentationParameters.BackBufferHeight;

            GeneratePerspectiveProjectionMatrix(MathUtil.PiOverFour, device);

            this.Position = Position;

            Up = Vector3.Up;
            Forward = Vector3.ForwardLH;

            SetHandleInput(HandleInput);

            this.speed = speed;

            Frustum = new BoundingFrustum(View * Projection);
        }

        private void GeneratePerspectiveProjectionMatrix(float fieldOfView, GraphicsDevice device)
        {
           
            float aspectRatio = (float) device.ViewPort.Width/(float) device.ViewPort.Height;
            this.Projection = Matrix.PerspectiveFovLH(fieldOfView, aspectRatio, NEARPLANE, FARPLANE);
        }

        public void SetHandleInput(bool handleInput)
        {
            this.handleInput = handleInput;
            if (handleInput)
            {
               // Mouse.SetPosition(250, 250);
                lastMouseState = InputManager.MousePosition;
            }
        }

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        private void Rotate(float YawChange, float PitchChange, float RollChange)
        {
            RollCam(RollChange);
            YawCam(YawChange);
            PitchCam(PitchChange);
        }

        private void RollCam(float amount)
        {
            Up.Normalize();
            Up = (Vector3) Vector3.Transform(Up, Matrix.RotationAxis(Forward, MathUtil.DegreesToRadians(amount)));
        }

        private void YawCam(float amount)
        {
            Forward.Normalize();
            Forward = (Vector3)Vector3.Transform(Forward, Matrix.RotationAxis(Up, MathUtil.DegreesToRadians(amount)));
        }

        private void PitchCam(float amount)
        {
            Forward.Normalize();
            Vector3 left = Vector3.Cross(Up, Forward);

            Forward = (Vector3) Vector3.Transform(Forward, Matrix.RotationAxis(left, MathUtil.DegreesToRadians(amount)));
            Up = (Vector3) Vector3.Transform(Up, Matrix.RotationAxis(left, MathUtil.DegreesToRadians(amount)));
        }

        private void MoveForwBackw(float amount)
        {
            Forward.Normalize();
            Position += Forward * amount;

        }

        private void MoveLeftRight(float amount)
        {
            Right = Vector3.Cross(Up, Forward);
            Right.Normalize();
            Position += Right * amount;

        }

        public void Update(float elapsed)
        {
            //if (handleInput)
            //{
                var mouseState = InputManager.MousePosition;


                if(InputManager.GetMouse(MouseButtons.Right))
                {
                    float deltaX = (float)lastMouseState.X - (float)mouseState.X;
                    float deltaY = (float)lastMouseState.Y - (float)mouseState.Y;
                    float deltaZ = 0.0f;
                    if (InputManager.GetKey(Keys.Q))
                        deltaZ += 1f;
                    else if (InputManager.GetKey(Keys.E))
                        deltaZ -= 1f;

                    Rotate(-deltaX * 0.04f, -deltaY * 0.04f, deltaZ);
                   // Mouse.SetPosition(250, 250);
                }

                //Handle KeyInput
                Vector3 newTranslation = Vector3.Zero;
                if (InputManager.GetKey(Keys.W))
                    newTranslation += Vector3.BackwardRH;
                if (InputManager.GetKey(Keys.S))
                    newTranslation += Vector3.ForwardRH;
                if (InputManager.GetKey(Keys.A))
                    newTranslation += Vector3.Left;
                if (InputManager.GetKey(Keys.D))
                    newTranslation += Vector3.Right;
                if (InputManager.GetKey(Keys.Shift))
                {
                    newTranslation *= speed  * 4000.0003f;
                }

            //if (mouseState.ScrollWheelValue != 0)
            //{
            //    float C = mouseState.ScrollWheelValue / 320.0f;

            //    speed += (C * 0.03f);

            //    if (speed < 0.001f) speed = 0.00001f;
            //    if (speed > 0.01) speed = 0.01f;
            //}


            newTranslation *= speed ;
                MoveForwBackw(newTranslation.Z);
                MoveLeftRight(newTranslation.X);

                lastMouseState = InputManager.MousePosition;
           // }
          //  Console.WriteLine(Position);
            View = Matrix.LookAtLH(Vector3.Zero, Forward, Up);

            Frustum.Matrix = View * Projection;
        }
    }
}
