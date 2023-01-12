namespace GeometryWarsGame
{
    public static class Program
    {
        public static Game.Window GameWindow = new Game.Window();

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            //Application.Run(GameWindow);
            Application.Run(new Launcher.Window());
        }
    }
}