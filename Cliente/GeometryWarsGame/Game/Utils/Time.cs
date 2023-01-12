using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GeometryWarsGame.Game.Utils
{
    public class Time
    {
        /// <summary>
        /// Get a reference timestamp in seconds
        /// </summary>
        /// <returns></returns>
        public static int GetReference()
        {
            return (int)(GetReferenceNano() * 1e-9);
        }

        /// <summary>
        /// Get a reference timestamp in milliseconds
        /// </summary>
        /// <returns></returns>
        public static long GetReferenceMillis()
        {
            return (long)(GetReferenceNano() * 1e-6);
        }

        /// <summary>
        /// Get a reference timestamp in nanoseconds
        /// </summary>
        /// <returns></returns>
        public static long GetReferenceNano()
        {
            return 100L * (10000L * Stopwatch.GetTimestamp() / TimeSpan.TicksPerMillisecond);
        }

    }
}
