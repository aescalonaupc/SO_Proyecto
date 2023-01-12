using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace GeometryWarsGame.Game
{
    public static class Logs
    {
        /// <summary>
        /// Concurrent pool of messages to print from loggin thread
        /// </summary>
        private static BlockingCollection<string> messagePool = new BlockingCollection<string>();

        /// <summary>
        /// Initialize the logging thread
        /// </summary>
        public static void Initialize()
        {
            Task.Run(() =>
            {
                for (; ; )
                {
                    Print(messagePool.Take());
                }
            });
        }

        /// <summary>
        /// Print the given text to the console
        /// </summary>
        /// <param name="text"></param>
        public static void PrintDebug(string text)
        {
            DateTime dtNow = DateTime.Now;
            string tsNow = "[" + dtNow.Hour.ToString("00") + ":" + dtNow.Minute.ToString("00") + ":" + dtNow.Second.ToString("00") + "]";

            messagePool.Add("[DEBUG] " + tsNow + " (Thread #" + Environment.CurrentManagedThreadId + ") " + text);
        }

        private static void Print(string text)
        {
            Console.WriteLine(text);
        }
    }
}
