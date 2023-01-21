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
        private static Ui.Button? unloadMusic = null;
        private static Ui.Button? quitGame = null;

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
            pauseLabel.CenterOnX();

            playButton = new Ui.Button(200, 50, new Vector2D(Window.InitialWidth / 2 - 100, 150), "Return to game");
            playButton.CenterOnX();

            if (!SoundManager.IsPlayerUnloaded())
            {
                controlMusic = new Ui.Button(200, 50, new Vector2D(Window.InitialWidth / 2 - 100, 220), SoundManager.IsPlaying() ? "Stop music" : "Play music");
                controlMusic.CenterOnX();

                unloadMusic = new Ui.Button(200, 50, new Vector2D(Window.InitialWidth / 2 - 100, 290), "Unload music");
                unloadMusic.CenterOnX();

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

                unloadMusic.Callback = () =>
                {
                    SoundManager.Unload();
                    Hide();
                };
            }

            quitGame = new Ui.Button(200, 50, new Vector2D(Window.InitialWidth / 2 - 100, 360), "Quit game");
            quitGame.CenterOnX();

            playButton.Callback = () =>
            {
                Hide();
                Program.GameWindow.SetGameState(GameState.PauseMenu, false);
            };

            quitGame.Callback = () =>
            {
                Window.CloseGame();
            };

            UiManager.AddComponent(pauseLabel);
            UiManager.AddComponent(playButton);

            if (controlMusic != null)
            {
                UiManager.AddComponent(controlMusic);
            }

            if (unloadMusic != null)
            {
                UiManager.AddComponent(unloadMusic);
            }

            UiManager.AddComponent(quitGame);

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

            if (controlMusic != null)
            {
                UiManager.RemoveComponent(controlMusic);
            }

            if (unloadMusic != null)
            {
                UiManager.RemoveComponent(unloadMusic);
            }
            
            UiManager.RemoveComponent(quitGame!);

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
