using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Utils
{
    public static class Ui
    {
        public static readonly Font DebugFont = new Font("Consolas", 12);

        public static readonly Font SmallDebugFont = new Font("Consolas", 8);

        public static readonly Font NametagFont = new Font("Consolas", 10);

        public static readonly Font NotificationFont = new Font("Segoe UI", 24);

        public static readonly Pen WhitePen = new Pen(Color.White);

        public static readonly Pen GrayPen = new Pen(Color.Gray);

        public static readonly Pen RedPen = new Pen(Color.Red);

        public static readonly Pen BlackPen = new Pen(Color.Black);

        public static readonly Pen GreenPen = new Pen(Color.FromArgb(255, 66, 255, 110));
        
        public static readonly Brush RedBrush = RedPen.Brush;

        public static readonly Brush GreenBrush = GreenPen.Brush;

        public static readonly Brush WhiteBrush = WhitePen.Brush;

        public static readonly Brush BlackBrush = BlackPen.Brush;

        public static readonly Color PlayerColor = Color.FromArgb(23, 117, 212);

        public static readonly Color EnemyColor = Color.FromArgb(212, 35, 19);

        public static readonly Color BackgroundColor = Color.FromArgb(23, 23, 23);

    }
}
