using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game
{
    public class Vector2D
    {
        /// <summary>
        /// X component
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y component
        /// </summary>
        public float Y { get; set; }

        public Vector2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return "{X=" + X + ",Y=" + Y + "}";
        }

        /// <summary>
        /// Build a Vector2D with components (0,0)
        /// </summary>
        /// <returns></returns>
        public static Vector2D Zero()
        {
            return new Vector2D(0, 0);
        }

        /// <summary>
        /// Build an independent copy of the current Vector2D
        /// </summary>
        /// <returns></returns>
        public Vector2D Clone()
        {
            return new Vector2D(X, Y);
        }
    }
}
