using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game
{
    public class NotificationManager
    {
        private static readonly Font Font = Utils.Ui.NotificationFont;

        private static readonly Brush Brush = new Pen(Color.FromArgb(80, Color.Red)).Brush;

        private static ConcurrentQueue<Notification> notifications = new ConcurrentQueue<Notification>();
        private static Notification? currentNotification = null;

        /// <summary>
        /// Send the specified notification to the queue and display when possible
        /// </summary>
        /// <param name="text"></param>
        /// <param name="duration"></param>
        public static void Notify(string text, int duration)
        {
            Notification not = new Notification(text, duration);
            notifications.Enqueue(not);

            Logs.PrintDebug("Notification queue size: " + notifications.Count);
        }

        public static void Render(Graphics g)
        {
            if (currentNotification == null && notifications.Count <= 0)
            {
                return;
            }

            if (currentNotification == null)
            {
                if (!notifications.TryDequeue(out currentNotification))
                {
                    return;
                }

                currentNotification.Time = Utils.Time.GetReference();
            }

            if (Utils.Time.GetReference() - currentNotification.Time >= currentNotification.Duration)
            {
                currentNotification = null;
                return;
            }

            Rectangle rect = new Rectangle(0, Program.GameWindow.Height / 2 + 100, Program.GameWindow.Width, Program.GameWindow.Height);

            using GraphicsPath gp = new GraphicsPath();
            using Pen outline = new Pen(Color.Black, 2) { LineJoin = LineJoin.Round };
            using StringFormat format = new StringFormat() { Alignment = StringAlignment.Center };

            gp.AddString(currentNotification.Text, Font.FontFamily, (int)Font.Style, Font.Size, rect, format);

            RectangleF bounds = RectangleF.Inflate(gp.GetBounds(), 10.0f, 10.0f);
            g.FillRectangle(Brush, bounds);

            g.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawPath(outline, gp);
            g.FillPath(Utils.Ui.WhiteBrush, gp);
            g.ResetTransform();
        }

    }

    public class Notification
    {
        /// <summary>
        /// Text to display
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Display duration in seconds
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// When the notification was created
        /// </summary>
        public int Time { get; set; }

        public Notification(string text, int duration)
        {
            Text = text;
            Duration = duration;
        }
    }
}
