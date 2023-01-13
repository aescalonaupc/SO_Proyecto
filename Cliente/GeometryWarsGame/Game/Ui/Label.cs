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
        /// String format used for centering on screen
        /// </summary>
        private StringFormat stringFormat = new StringFormat();

        public Label(string text, Font font, Vector2D position, int layer = 0) : base(position, layer)
        {
            Text = text;
            Font = font;
        }

        /// <summary>
        /// Center on X position
        /// </summary>
        public void CenterOnX()
        {
            Position.X = 0;
            stringFormat.Alignment = StringAlignment.Center;
        }

        /// <summary>
        /// Center on Y position
        /// </summary>
        public void CenterOnY()
        {
            Position.Y = 0;
            stringFormat.LineAlignment = StringAlignment.Center;
        }

        public override void Render(Graphics g)
        {
            Rectangle rect = new Rectangle((int)Position.X, (int)Position.Y, Program.GameWindow.Width, Program.GameWindow.Height);
            g.DrawString(Text, Font, new Pen(Color).Brush, rect, stringFormat);
        }

        public override void Update() { }
    }
}
