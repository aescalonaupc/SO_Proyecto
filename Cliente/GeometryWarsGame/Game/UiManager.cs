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

            bool shouldHandCursor = false;
            Vector2D mouseCoords = Program.GameWindow.MouseCoords;

            int[] layers = components.Keys.ToArray();
            for (int i = 0; i < layers.Length; i++)
            {
                List<UiComponent> lc = components[layers[i]].ToList();
                foreach (UiComponent c in lc)
                {
                    c.Update();

                    if (!shouldHandCursor)
                    {
                        if (c is IWideComponent && ((IWideComponent)c).IsInside(mouseCoords))
                        {
                            shouldHandCursor = true;
                        }
                    }
                    
                }
            }

            //if (shouldHandCursor)
            //{
            //    Program.GameWindow.Invoke(() =>
            //    {
            //        Program.GameWindow.Cursor = Cursors.Hand;
            //    });
            //}
            //else
            //{
            //    if (Program.GameWindow.IsGameRunning())
            //    {
            //        Program.GameWindow.Invoke(() =>
            //        {
            //            Program.GameWindow.Cursor = Cursors.Cross;
            //        });
            //    }
            //    else
            //    {
            //        Program.GameWindow.Invoke(() =>
            //        {
            //            Program.GameWindow.Cursor = Cursors.Default;
            //        });
            //    }
            //}

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
