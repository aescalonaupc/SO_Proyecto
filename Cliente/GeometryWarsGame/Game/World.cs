using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game
{
    public static class World
    {
        /// <summary>
        /// Width of the camera
        /// </summary>
        private static int CameraWidth => Program.GameWindow.Width;

        /// <summary>
        /// Height of the camera
        /// </summary>
        private static int CameraHeight => Program.GameWindow.Height;

        /// <summary>
        /// Velocity at which the camera is moved
        /// </summary>
        private static readonly float CameraVelocity = 4.0f;

        /// <summary>
        /// Width of the world
        /// </summary>
        public static int Width { get; private set; }

        /// <summary>
        /// Height of the world
        /// </summary>
        public static int Height { get; private set; }

        /// <summary>
        /// X at which the world is drawn in camera coordinates
        /// </summary>
        private static float OffsetX { get; set; }

        /// <summary>
        /// Y at which the world is drawn in camera coordinates
        /// </summary>
        private static float OffsetY { get; set; }

        private static readonly Brush BgBrush = new Pen(Utils.Ui.BackgroundColor).Brush;

        public static void Render(Graphics g)
        {
            g.DrawRectangle(Utils.Ui.RedPen, OffsetX - 1, OffsetY - 1, Width + 2, Height + 2);
            g.FillRectangle(BgBrush, OffsetX, OffsetY, Width, Height);

            // kind of grid
            {
                const int separation = 100;
                Vector2D pos;
                for (int i = separation; i < Width; i+= separation)
                {
                    for (int j = separation; j < Height; j += separation)
                    {
                        pos = FromWorldCoordinateToCam(i, j);
                        g.FillRectangle(Utils.Ui.GrayPen.Brush, pos.X, pos.Y, 5, 5);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the world
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="initialOffsetX"></param>
        /// <param name="initialOffsetY"></param>
        public static void SetWorld(int width, int height, float initialOffsetX = 0, float initialOffsetY = 0)
        {
            Width = width;
            Height = height;
            OffsetX = initialOffsetX;
            OffsetY = initialOffsetY;

            Logs.PrintDebug("Set world (" + Width + "x" + Height + ") to OffsetX=" + OffsetX + ", OffsetY=" + OffsetY);
        }

        /// <summary>
        /// Focus the camera on the given world coordinates position
        /// </summary>
        /// <param name="position"></param>
        public static void SetFocusPosition(Vector2D position)
        {
            OffsetX = CameraWidth / 2 - position.X;
            OffsetY = CameraHeight / 2 - position.Y;
        }

        /// <summary>
        /// Moves the camera in the given direction
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="delta"></param>
        public static void MoveCamera(CameraMovementDirection direction, float delta = 1.0f)
        {
            switch (direction)
            {
                case CameraMovementDirection.Up:
                    OffsetY += CameraVelocity * delta;
                    break;
                case CameraMovementDirection.Down:
                    OffsetY -= CameraVelocity * delta;
                    break;
                case CameraMovementDirection.Left:
                    OffsetX += CameraVelocity * delta;
                    break;
                case CameraMovementDirection.Right:
                    OffsetX -= CameraVelocity * delta;
                    break;
            }
        }

        /// <summary>
        /// Returns if the given world coordinate (x,y) is inside the camera
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool IsWorldCoordinateVisible(float x, float y)
        {
            return x + OffsetX > 0 && y + OffsetY > 0 && x < CameraWidth - OffsetX && y < CameraHeight - OffsetY;
        }

        /// <summary>
        /// Converts from world coordinate system to cam
        /// e.g. to draw elements on screen
        /// </summary>
        /// <param name="worldCoord"></param>
        /// <returns></returns>
        public static Vector2D FromWorldCoordinateToCam(Vector2D worldCoord)
        {
            return FromWorldCoordinateToCam(worldCoord.X, worldCoord.Y);
        }

        /// <summary>
        /// Converts from cam coordinate system to world
        /// </summary>
        /// <param name="camCoord"></param>
        /// <returns></returns>
        public static Vector2D FromCamCoordinateToWorld(Vector2D camCoord)
        {
            return FromCamCoordinateToWorld(camCoord.X, camCoord.Y);
        }

        /// <summary>
        /// Converts from world coordinate system to cam
        /// e.g. to draw elements on screen
        /// </summary>
        /// <returns></returns>
        public static Vector2D FromWorldCoordinateToCam(float x, float y)
        {
            return new Vector2D(x + OffsetX, y + OffsetY);
        }

        /// <summary>
        /// Converts from cam coordinate system to world
        /// </summary>
        /// <returns></returns>
        public static Vector2D FromCamCoordinateToWorld(float x, float y)
        {
            return new Vector2D(x - OffsetX, y - OffsetY);
        }
    }

    public enum CameraMovementDirection
    {
        Down,
        Up,
        Right,
        Left
    }
}
