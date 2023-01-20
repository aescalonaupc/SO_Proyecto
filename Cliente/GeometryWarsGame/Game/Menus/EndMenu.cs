using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Menus
{
    public class EndMenu
    {
        /// <summary>
        /// Animation type (text), 0 = Win animation, 1 = Lose animation
        /// </summary>
        private static int animationType = -1;

        /// <summary>
        /// Brush for lose text, create only once!
        /// </summary>
        private static Brush LoseBrush = new Pen(Color.FromArgb(80, Color.Red)).Brush;

        /// <summary>
        /// Brush for victory text, create only once!
        /// </summary>
        private static Brush VictoryBrush = new Pen(Color.FromArgb(80, Utils.Ui.GreenPen.Color)).Brush;

        /// <summary>
        /// Show win text
        /// </summary>
        public static void ShowWin()
        {
            animationType = 0;
        }

        /// <summary>
        /// SHoww lose text
        /// </summary>
        public static void ShowLose()
        {
            animationType = 1;
        }

        public static void Stop()
        {
            animationType = -1;
        }

        public static void Render(Graphics g)
        {
            if (animationType < 0)
            {
                return;
            }

            // Position to draw in the screen (will be centered on X, on Y it is hardcoded)
            Rectangle rect = new Rectangle(0, 0, Program.GameWindow.Width, Program.GameWindow.Height);

            using GraphicsPath gp = new GraphicsPath();
            using Pen outline = new Pen(Color.Black, 2) { LineJoin = LineJoin.Round };
            using StringFormat format = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

            // Draw text as usual
            gp.AddString(animationType > 0 ? "Derrota" : "Victoria", Utils.Ui.NotificationFont.FontFamily, (int)Utils.Ui.NotificationFont.Style, Utils.Ui.NotificationFont.Size * 3.0f, rect, format);

            // Once we have the text bounds,
            // We draw the background
            {
                const int diameter = 20;
                Size size = new Size(diameter, diameter);

                Rectangle bounds = Rectangle.Round(RectangleF.Inflate(gp.GetBounds(), 120.0f, 30.0f));
                Rectangle arc = new Rectangle(bounds.Location, size);

                GraphicsPath path = new GraphicsPath();

                // Top left arc  
                path.AddArc(arc, 180, 90);

                // Top right arc  
                arc.X = bounds.Right - diameter;
                path.AddArc(arc, 270, 90);

                // Bottom right arc  
                arc.Y = bounds.Bottom - diameter;
                path.AddArc(arc, 0, 90);

                // Bottom left arc 
                arc.X = bounds.Left;
                path.AddArc(arc, 90, 90);

                path.CloseFigure();
                g.FillPath(animationType > 0 ? LoseBrush : VictoryBrush, path);
            }

            // Then draw the text outline
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawPath(outline, gp);
            g.FillPath(Utils.Ui.WhiteBrush, gp);
        }
    }
}
