using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometryWarsGame.Game.Ui;

namespace GeometryWarsGame.Game
{
    public class UiManager
    {
        /// <summary>
        /// List of UI components at each layer
        /// [layer id] -> List<UiComponent>
        /// </summary>
        private readonly static Dictionary<int, List<UiComponent>> components = new Dictionary<int, List<UiComponent>>();

        /// <summary>
        /// Event fired when click is performed in Ui
        /// </summary>
        public static event UiClick? OnUiClick;
        public delegate void UiClick(Vector2D position, MouseButtons button);

        public static void Update()
        {
            if (components.Count <= 0)
            {
                return;
            }

            Vector2D mouseCoords = Program.GameWindow.MouseCoords;
            bool checkHandCursor = false;

            int[] layers = components.Keys.ToArray();
            for (int i = 0; i < layers.Length; i++)
            {
                List<UiComponent> lc = components[layers[i]].ToList();
                foreach (UiComponent c in lc)
                {
                    c.Update();

                    if (!checkHandCursor && c is IClickable && c is IWideComponent)
                    {
                        checkHandCursor = (c as IWideComponent)!.IsInside(mouseCoords);
                    }
                }
            }

            Window.CallUIThread(() =>
            {
                if (checkHandCursor && Program.GameWindow.Cursor != Cursors.Hand)
                {
                    Logs.PrintDebug("Change to hand cursor!");
                    Program.GameWindow.Cursor = Cursors.Hand;
                    return;
                }

                if (!checkHandCursor && Program.GameWindow.Cursor == Cursors.Hand)
                {
                    Logs.PrintDebug("Change to default cursor!");
                    Program.GameWindow.Cursor = Cursors.Default;
                    return;
                }
            }, uniquenessKey: 0);
        }

        public static void Render(Graphics g)
        {
            if (components.Count <= 0)
            {
                return;
            }

            int[] layers = components.Keys.ToArray();
            for (int i = 0; i < layers.Length; i++)
            {
                List<UiComponent> lc = components[layers[i]].ToList();
                foreach (UiComponent c in lc)
                {
                    c.Render(g);
                }
            }
        }

        public static void Click(Vector2D position, MouseButtons button)
        {
            OnUiClick?.Invoke(position, button);
        }

        /// <summary>
        /// Add the given component to the screen
        /// </summary>
        /// <param name="component"></param>
        public static void AddComponent(UiComponent component)
        {
            if (!components.ContainsKey(component.Layer))
            {
                components.Add(component.Layer, new List<UiComponent>());
            }

            components[component.Layer].Add(component);
        }

        /// <summary>
        /// Remove the given component from the screen
        /// </summary>
        /// <param name="component"></param>
        public static void RemoveComponent(UiComponent component)
        {
            if (!components.ContainsKey(component.Layer))
            {
                return;
            }

            components[component.Layer].Remove(component);
            component.Dispose();
        }
    }
}
