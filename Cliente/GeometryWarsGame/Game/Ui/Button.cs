using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Ui
{
    public class Button : UiComponent, IClickable, IWideComponent
    {
        /// <summary>
        /// Width of the button
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of the button
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Text inside the button, might be empty
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Color of the button
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Color of the button text, if any
        /// </summary>
        public Color TextColor { get; set; } = Color.White;

        /// <summary>
        /// Whether the button should be filled or just drawn its border
        /// </summary>
        public bool Fill { get; set; } = false;

        /// <summary>
        /// Callback when the button is clicked
        /// </summary>
        public Action? Callback { get; set; }

        /// <summary>
        /// If the button is being hovered currently
        /// </summary>
        private bool beingHovered = false;

        public Button(int width, int height, Vector2D position, string text, int layer = 0) : base(position, layer)
        {
            Width = width;
            Height = height;
            Text = text;
        }

        /// <summary>
        /// Change Position to center on X
        /// </summary>
        public void CenterOnX()
        {
            Position.X = Program.GameWindow.Width / 2 - Width / 2;
        }

        /// <summary>
        /// Change position to center on Y
        /// </summary>
        public void CenterOnY()
        {
            Position.Y = Program.GameWindow.Height / 2 - Height / 2;
        }

        public override void Update() { }

        public override void Render(Graphics g)
        {
            Rectangle rect = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

            if (beingHovered)
            {
                g.FillRectangle(new Pen(Color.FromArgb(80, 255, 255, 255)).Brush, rect);
            } else
            {
                if (Fill)
                {
                    g.FillRectangle(new Pen(Color).Brush, rect);
                }
                else
                {
                    g.DrawRectangle(new Pen(Color), rect);
                }
            }

            g.DrawString(Text, Utils.Ui.DebugFont, new Pen(TextColor).Brush, rect, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

        }

        public void Click(Vector2D position, MouseButtons button)
        {
            if (button != MouseButtons.Left)
            {
                return;
            }

            if (IsInside(position))
            {
                Callback?.Invoke();
            }
        }

        public bool IsInside(Vector2D position)
        {
            return position.X > Position.X && position.X < Position.X + Width && position.Y > Position.Y && position.Y < Position.Y + Height;
        }
    }
}
