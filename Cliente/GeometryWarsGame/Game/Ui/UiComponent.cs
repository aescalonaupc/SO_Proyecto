using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Ui
{
    public abstract class UiComponent : IDisposable
    {
        /// <summary>
        /// Position of the Ui component in screen
        /// </summary>
        public Vector2D Position { get; set; }

        /// <summary>
        /// Layer in which the Ui component lives
        /// </summary>
        public int Layer { get; set; }

        public abstract void Update();

        public abstract void Render(Graphics g);

        public UiComponent(Vector2D position, int layer)
        {
            Position = position;
            Layer = layer;

            if (this is IClickable)
            {
                Logs.PrintDebug("Registered OnUiClick for IClickable " + this);
                UiManager.OnUiClick += UiManager_OnUiClick;
            }
            //UiManager.AddComponent(this);
        }

        private void UiManager_OnUiClick(Vector2D position, MouseButtons button)
        {
            if (this is IClickable)
            {
                ((IClickable)this).Click(position, button);
            }
        }

        /// <summary>
        /// Dispose the Ui component
        /// </summary>
        public void Dispose()
        {
            UiManager.OnUiClick -= UiManager_OnUiClick;
        }
    }

    /// <summary>
    /// Represents a Ui component which should respond to click events
    /// </summary>
    public interface IClickable
    {
        /// <summary>
        /// Method executed when the given mouse buton is pressed in the given screen position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="button"></param>
        public abstract void Click(Vector2D position, MouseButtons button);
    }

    /// <summary>
    /// Represents a Ui component which should respond to its size
    /// </summary>
    public interface IWideComponent
    {
        /// <summary>
        /// If the given position is inside of the Ui component
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public abstract bool IsInside(Vector2D position);
    }
}
