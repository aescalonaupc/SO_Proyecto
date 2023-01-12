using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Menus
{
    public class Loading
    {
        private static bool visible = false;

        private static Ui.Label? loadingLabel;

        /// <summary>
        /// Show the menu, might be called multiple times
        /// </summary>
        public static void Show()
        {
            if (visible)
            {
                return;
            }

            loadingLabel = new Ui.Label("Loading...", new Font("Arial", 24), new Vector2D(20, Window.InitialHeight - 50), 100);

            UiManager.AddComponent(loadingLabel);

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

            UiManager.RemoveComponent(loadingLabel!);

            visible = false;
        }
    }
}
