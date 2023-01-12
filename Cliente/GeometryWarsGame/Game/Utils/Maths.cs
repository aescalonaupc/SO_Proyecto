using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Utils
{
    public static class Maths
    {
        private static readonly float RADIANS_PER_DEGREE = (float) Math.PI / 180;

        /// <summary>
        /// Convert the given degrees to radians
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static double DegreesToRadians(double degree)
        {
            return degree * RADIANS_PER_DEGREE;
        }

        /// <summary>
        /// Convert the given radias to degrees
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians / RADIANS_PER_DEGREE;
        }

        /// <summary>
        /// Distance between two vectors
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double Distance(Vector2D v1, Vector2D v2)
        {
            return Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2));
        }

        /// <summary>
        /// Fast computation of distance between vectors, avoids square root
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double DistanceFast(Vector2D v1, Vector2D v2)
        {
            return Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2);
        }
    }
}
