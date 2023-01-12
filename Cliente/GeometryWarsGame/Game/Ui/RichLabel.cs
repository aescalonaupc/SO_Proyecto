using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Ui
{
    public class RichLabel : UiComponent
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

        public RichLabel(string text, Font font, Vector2D position, int layer = 0) : base(position, layer)
        {
            Text = text;
            Font = font;
        }

        public override void Render(Graphics g)
        {
            TextRenderer.DrawText(g, Text, Font, new Point((int)Position.X, (int)Position.Y), Color);
        }

        public override void Update()
        {

        }
    }
}
