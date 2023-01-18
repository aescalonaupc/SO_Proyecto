#define USE_SHIVA
using System.Net.Sockets;

namespace GeometryWarsGame
{
    public static class Program
    {
        /// <summary>
        /// Global object for the game window
        /// </summary>
        public static Game.Window GameWindow = new Game.Window();

#if !USE_SHIVA
        private static string ip = "192.168.56.101";
        private static int port = 5059;
#else
        private static string ip = "147.83.117.22";
        private static int port = 50059;
#endif

        public const bool SkipOpenLauncher = false;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            if (SkipOpenLauncher)
            {
                Application.Run(GameWindow);
            } else
            {
                if (!Shared.NetworkHandler.Initialiaze(ip, port, 100, 100))
                {
                    MessageBox.Show("No se ha podido conectar con el servidor!");
                    return;
                }

                Application.Run(new Launcher.Window());
            }
        }
    }
}