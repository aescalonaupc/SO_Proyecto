using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Utils
{
    public class TimerManager
    {
        private static readonly List<Timer> timers = new List<Timer>();

        private static long lastTime = Time.GetReferenceMillis();

        public static void Update()
        {
            long dt = Time.GetReferenceMillis();
            if (dt - lastTime > 50)
            {
                for (int i = timers.Count - 1; i >= 0; i--)
                {
                    if (dt - timers[i].Time >= timers[i].Milliseconds)
                    {
                        Logs.PrintDebug("Executing timer " + i);
                        timers[i].Run();
                    }
                }

                lastTime = dt;
            }
        }

        public class Timer
        {
            public long Time { get; set; }

            public int Milliseconds { get; private set; }

            public Action Callback { get; private set; }

            public Timer(int milliseconds, Action callback)
            {
                Milliseconds = milliseconds;
                Callback = callback;

                Time = Utils.Time.GetReferenceMillis();
                timers.Add(this);
            }

            public void Cancel()
            {
                timers.Remove(this);
            }

            public void Run()
            {
                Callback();
                timers.Remove(this);
            }
        }
    }
}
