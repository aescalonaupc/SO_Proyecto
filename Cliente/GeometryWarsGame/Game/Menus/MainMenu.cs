using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Menus
{
    public class MainMenu
    {
        private static Ui.Label? title = null;
        private static Ui.Button? playButton = null;

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

            title = new Ui.Label("Geometry Wars", new Font("Arial", 24), new Vector2D(Window.InitialWidth / 2 - 120, 20));

            playButton = new Ui.Button(200, 50, new Vector2D(Window.InitialWidth / 2 - 100, 100), "Play");
            playButton.Callback = () =>
            {
                Hide();
                Program.GameWindow.SetGameState(GameState.MainMenu, false);
                Program.GameWindow.StartGameplay();
            };

            UiManager.AddComponent(title);
            UiManager.AddComponent(playButton);

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

            UiManager.RemoveComponent(title!);
            UiManager.RemoveComponent(playButton!);

            visible = false;
        }
    }
}
