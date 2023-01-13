namespace GeometryWarsGame
{
    public static class Program
    {
        public static Game.Window GameWindow = new Game.Window();

        public const bool SkipOpenLauncher = true;

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
                Application.Run(new Launcher.Window());
            }
        }
    }
}