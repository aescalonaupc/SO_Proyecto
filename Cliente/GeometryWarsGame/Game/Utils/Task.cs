using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Utils
{
    public class Task
    {
        /// <summary>
        /// Apparently runs task asynchronously and continues execution by
        /// adding the task to the queue
        /// </summary>
        /// <param name="task"></param>
        public static void RunAndForget(System.Threading.Tasks.Task task)
        {
            System.Threading.Tasks.Task.Run(async () => { await task; });
        }
    }
}
