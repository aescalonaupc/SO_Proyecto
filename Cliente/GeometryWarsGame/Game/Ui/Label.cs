using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Ui
{
    public class Label : UiComponent
    {
        /// <summary>
        /// Text of the label
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Font of the label
        /// </summary>
        public Font Font { get; set; }

        /// <summary>
        /// Color of the label
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// If label is centered in X screen
        /// </summary>
        public bool IsXCentered { get; set; } = false;

        /// <summary>
        /// If label is centered in Y screen
        /// </summary>
        public bool IsYCentered { get; set; } = false;

        public Label(string text, Font font, Vector2D position, int layer = 0) : base(position, layer)
        {
            Text = text;
            Font = font;
        }

        public override void Render(Graphics g)
        {
            if (IsXCentered || IsYCentered)
            {
                Rectangle rect = new Rectangle((int)Position.X, (int)Position.Y, Program.GameWindow.Width, Program.GameWindow.Height);
                StringFormat format = new StringFormat();

                if (IsXCentered)
                {
                    rect.X = 0;
                    format.Alignment = StringAlignment.Center;
                }

                if (IsYCentered)
                {
                    rect.Y = 0;
                    format.LineAlignment = StringAlignment.Center;
                }

                g.DrawString(Text, Font, new Pen(Color).Brush, rect, format);
                return;
            }

            g.DrawString(Text, Font, new Pen(Color).Brush, Position.X, Position.Y);
        }

        public override void Update() { }
    }
}
