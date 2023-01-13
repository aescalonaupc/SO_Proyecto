using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Menus
{
    public class MusicPlayer
    {
        private static Ui.RichLabel? nowPlaying;

        private static bool visible = false;

        public static void Show()
        {
            if (visible)
            {
                return;
            }

            nowPlaying = new Ui.RichLabel("🔊 Now Playing: " + (SoundManager.IsPlaying() ? SoundManager.CurrentTrack!.Name : "Nothing") + " 🔊", Utils.Ui.DebugFont, new Vector2D(5, Program.GameWindow.Height - 100), 100);

            UiManager.AddComponent(nowPlaying);

            visible = true;
        }

        public static void Hide()
        {
            if (!visible)
            {
                return;
            }

            UiManager.RemoveComponent(nowPlaying!);

            visible = false;
        }

        public static void Update()
        {
            if (!visible)
            {
                return;
            }

            if (nowPlaying != null)
            {
                nowPlaying.Position.Y = Program.GameWindow.Height - 100;
                nowPlaying.Text = "🔊 Now Playing: " + (SoundManager.IsPlaying() ? SoundManager.CurrentTrack!.Name : "Nothing") + " 🔊";
            }
        }
    }
}
