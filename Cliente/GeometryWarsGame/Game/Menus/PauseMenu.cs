using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Menus
{
    public static class PauseMenu
    {
        private static Ui.Label? pauseLabel = null;
        private static Ui.Button? playButton = null;
        private static Ui.Button? controlMusic = null;

        private static bool visible = false;

        /// <summary>
        /// Show the menu, might be called multiple times
        /// </summary>
        public static void Show()
        {
            if (visible)
            {
                return;
            }

            pauseLabel = new Ui.Label("Pause menu", new Font("Arial", 24), new Vector2D(Window.InitialWidth / 2 - 120, 100));
            pauseLabel.IsXCentered = true;

            playButton = new Ui.Button(200, 50, new Vector2D(Window.InitialWidth / 2 - 100, 150), "Return to game");
            playButton.IsXCentered = true;

            controlMusic = new Ui.Button(200, 50, new Vector2D(Window.InitialWidth / 2 - 100, 220), SoundManager.IsPlaying() ? "Stop music" : "Play music");
            controlMusic.IsXCentered = true;

            playButton.Callback = () =>
            {
                Hide();
                Program.GameWindow.SetGameState(GameState.PauseMenu, false);
            };

            controlMusic.Callback = () =>
            {
                if (SoundManager.IsPlaying())
                {
                    //SoundManager.StopQueue();
                    SoundManager.SetVolume(0);
                }
                else
                {
                    //SoundManager.PlayIngame();
                    SoundManager.SetVolume(1);
                }
            };

            UiManager.AddComponent(pauseLabel);
            UiManager.AddComponent(playButton);
            UiManager.AddComponent(controlMusic);

            visible = true;
        }

        /// <summary>
        /// Hide the menu, might be called multiple times
        /// </summary>
        public static void Hide()
        {
            if (!visible)
            {
                return;
            }

            UiManager.RemoveComponent(pauseLabel!);
            UiManager.RemoveComponent(playButton!);
            UiManager.RemoveComponent(controlMusic!);

            visible = false;
        }

        public static void Update()
        {
            if (!visible)
            {
                return;
            }

            if (controlMusic != null)
            {
                controlMusic.Text = SoundManager.IsPlaying() ? "Stop music" : "Play music";
            }
        }
    }
}
