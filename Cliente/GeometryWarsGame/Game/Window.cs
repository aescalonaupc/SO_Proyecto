using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GeometryWarsGame.Game.Entities;
using GeometryWarsGame.Game.Utils;
using System.Runtime.InteropServices;

namespace GeometryWarsGame.Game
{
    public partial class Window : Form
    {
        /// <summary>
        /// Initial width of the window
        /// </summary>
        public const int InitialWidth = 1280;

        /// <summary>
        /// Initial height of the window
        /// </summary>
        public const int InitialHeight = 720;

        /// <summary>
        /// Game name for the window title
        /// </summary>
        public const string Title = "Geometry Wars";

        /// <summary>
        /// The username of the local player
        /// </summary>
        public string MyUsername = "";

        /// <summary>
        /// The id of the local user in database
        /// </summary>
        public int MyId = -1;

        /// <summary>
        /// Whether the local player is master in the game or not
        /// master has the role of deciding entity ids and other game logic
        /// </summary>
        public bool Master = true;

        /// <summary>
        /// Spawn data received by the launcher used in networking to spawn players initially
        /// </summary>
        public List<PlayerSpawnData> PlayerSpawnData = new List<PlayerSpawnData>();

        /// <summary>
        /// List of actions called from other threads which need to be run in the main game thread
        /// </summary>
        private ConcurrentQueue<Action> ThreadSafeActions = new ConcurrentQueue<Action>();

        /// <summary>
        /// List of actions called from other threads to be run in main ui thread
        /// </summary>
        private ConcurrentQueue<Action> UIThreadSafeActions = new ConcurrentQueue<Action>();

        /// <summary>
        /// List of unique actions called from other threads to be run in main ui thread
        /// </summary>
        private ConcurrentDictionary<int, Action> UIThreadSafeUniqueActions = new ConcurrentDictionary<int, Action>();

        /// <summary>
        /// Current game states
        /// </summary>
        public GameState GameState = GameState.Loading;

        /// <summary>
        /// Game type playing
        /// </summary>
        public GameType GameType = GameType.Sandbox;

        private readonly WeakReference<LocalPlayer?> myPlayer = new WeakReference<LocalPlayer?>(null);

        /// <summary>
        /// Local player
        /// </summary>
        public LocalPlayer? MyPlayer
        {
            get
            {
                if (myPlayer.TryGetTarget(out LocalPlayer? t))
                {
                    return t;
                }

                return null;
            }

            set
            {
                myPlayer.SetTarget(value);
            }
        }

        /// <summary>
        /// Mouse coordinates with respect to the window (camera)
        /// </summary>
        public Vector2D MouseCoords;

        /// <summary>
        /// Count of frames in current second and fps counter
        /// </summary>
        private int currentFrames, fps;

        /// <summary>
        /// Main game thread executing logic
        /// </summary>
        private Thread? gameThread;

        /// <summary>
        /// Event fired on main game thread spawn
        /// </summary>
        public static event GameStartEvent? OnGameStart;

        /// <summary>
        /// Event fired on key down
        /// </summary>
        public static event KeyEvent? OnGameKeyDown;

        /// <summary>
        /// Event fired on key up
        /// </summary>
        public static event KeyEvent? OnGameKeyUp;

        /// <summary>
        /// Event fired on game state change
        /// </summary>
        public static event GameStateChangeEvent? OnGameStateChange;

        public delegate void KeyEvent(KeyEventArgs e);
        public delegate void GameStartEvent();
        public delegate void GameStateChangeEvent(GameState oldState, GameState newState);

        /// <summary>
        /// Represents the window's gdi interface (visible)
        /// </summary>
        private Graphics graphics;

        /// <summary>
        /// Buffered graphics object which will be drawn and swapped with `graphics` to be rendered
        /// </summary>
        private BufferedGraphics? bGraphics;

        /// <summary>
        /// `BufferedGraphicsContext` object to perform double-buffered logic
        /// </summary>
        private BufferedGraphicsContext bgContext = BufferedGraphicsManager.Current;

        #region native unmanaged code
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr Handle;
            public uint Message;
            public IntPtr WParameter;
            public IntPtr LParameter;
            public uint Time;
            public Point Location;
        }

        [DllImport("user32.dll")]
        public static extern int PeekMessage(out NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);
        #endregion

        public Window()
        {
            InitializeComponent();

            DoubleBuffered = true;
            ClientSize = new Size(InitialWidth, InitialHeight);
            Text = Title;

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, false);

            BackColor = Color.Black;

            MouseCoords = Vector2D.Zero();
            currentFrames = 0;
            fps = 0;

            // Get gdi interface from window handle
            graphics = Graphics.FromHwnd(Handle);

            // Set buffer graphics' buffer size
            // Double buffered graphics helps reduce flickering
            bgContext.MaximumBuffer = new Size(Width + 1, Height + 1);

            Logs.Initialize();

            // World is a "worldWidth x worldHeight" rectangle initially in the middle of the camera
            // `OffsetX` and `OffsetY` represent the cam displacement from the top-left world corner
            const int worldWidth = 1300, worldHeight = 1300;
            World.SetWorld(worldWidth, worldHeight, InitialWidth / 2 - worldWidth / 2, InitialHeight / 2 - worldHeight / 2);

            // Set common decimal separator, fix float format issues on different PCs
            if (System.Globalization.CultureInfo.DefaultThreadCurrentCulture == null)
            {
                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.InvariantCulture.Clone();
            }

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture.NumberFormat.NumberGroupSeparator = ".";
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture.NumberFormat.NumberDecimalSeparator = ",";

            KeyDown += OnNativeKeyDown;
            KeyUp += OnNativeKeyUp;

            MouseMove += OnNativeMouseMove;
            MouseClick += OnNativeMouseClick;

            OnGameStart += OnGameStartEvent;
            OnGameStateChange += OnGameStateChangeEvent;
            Shown += OnNativeShownEvent;

            // `Idle` is fired when app is not busy processing system messages
            // We use this fraction of time to execute queued thread safe actions
            Application.Idle += (object? _, EventArgs _) =>
            {
                if (gameThread != null && gameThread.IsAlive)
                {
                    while (0 == PeekMessage(out _, IntPtr.Zero, 0, 0, 0))
                    {
                        while (!UIThreadSafeActions.IsEmpty)
                        {
                            if (UIThreadSafeActions.TryDequeue(out Action? a))
                            {
                                a();
                            }
                        }

                        foreach (Action a in UIThreadSafeUniqueActions.Values)
                        {
                            a();
                        }

                        UIThreadSafeUniqueActions.Clear();
                    }
                }
            };

            Logs.PrintDebug("Window initialized");
        }

        private void OnGameStartEvent()
        {
            {
                if (!Program.SkipOpenLauncher)
                {
                    return;
                }

                MyUsername = "test";
                MyId = 1;

                Invoke(() =>
                {
                    SetLocalPlayer(MyUsername, MyId, true);
                    MyPlayer.Position = new Vector2D(100, 100);
                    World.SetFocusPosition(MyPlayer.Position);
                });

                //for (int i = 0; i <= World.Width; i += 100)
                //{
                //    for (int j = 0; j <= World.Height; j += 100)
                //    {
                //        Player p = new Player(i + " " + j, i, j);
                //        p.Color = (i / 100) % 2 == 0 ? Color.Yellow : Color.Red;

                //        p.Create();
                //        p.Spawn();
                //    }
                //}

                StartGameplay();

                Menus.EndMenu.ShowWin();

                NotificationManager.Notify("Test notification", 5);
                NotificationManager.Notify("Works >:)", 5);

                // respawn timer
                NotificationManager.Notify("3", 1);
                NotificationManager.Notify("2", 1);
                NotificationManager.Notify("1", 1);
                NotificationManager.Notify("Remaining lifes: " + MyPlayer.Lifes, 5);
            }
        }

        private void GameLoop()
        {
            Logs.PrintDebug("Started main game loop thread");

            // Fire `OnGameStart` event
            OnGameStart?.Invoke();

            // The following logic helps us perform
            // updates at a fixed rate of `APS_OBJETIVO` per second

            const long NS_SEGUNDO = (long)1e9;
            const byte APS_OBJETIVO = 60;
            const double NS_ACTUALIZACION = NS_SEGUNDO / APS_OBJETIVO;

            long referenciaActualizacion = Time.GetReferenceNano();
            long referenciaContador = Time.GetReferenceNano();

            long tiempoTranscurrido;
            double delta = 0;

            while (gameThread!.IsAlive)
            {
                long inicioBucle = Time.GetReferenceNano();
                tiempoTranscurrido = inicioBucle - referenciaActualizacion;
                referenciaActualizacion = inicioBucle;

                delta += tiempoTranscurrido / NS_ACTUALIZACION;

                while (delta >= 1)
                {
                    GameUpdate();
                    delta--;
                }

                // Allocate bew buffered graphics and perform render logic
                // Target `Graphics` is the window gdi interface `graphics`
                bGraphics = bgContext.Allocate(graphics, DisplayRectangle);
                GameRender(bGraphics.Graphics);

                // Every second we update fps counter
                if (Time.GetReferenceNano() - referenciaContador > NS_SEGUNDO)
                {
                    fps = currentFrames;
                    currentFrames = 0;
                    referenciaContador = Time.GetReferenceNano();
                }
            }
        }

        private void OnGameStateChangeEvent(GameState oldState, GameState newState)
        {
            Logs.PrintDebug("Changed game state from " + oldState + " to " + newState);

            if (
                ((oldState & GameState.Loading) > 0) && (newState & GameState.Loading) == 0
            ) {
                Menus.Loading.Hide();
            }

            if (IsGameRunning())
            {
                Window.CallUIThread(() =>
                {
                    Program.GameWindow.Cursor = Cursors.Cross;
                }, uniquenessKey: 0);
            } else if (IsAnyMenuOpen())
            {
                Window.CallUIThread(() =>
                {
                    Program.GameWindow.Cursor = Cursors.Default;
                }, uniquenessKey: 0);
            }
        }

        private void OnNativeMouseClick(object? sender, MouseEventArgs e)
        {
            UiManager.Click(new Vector2D(e.X, e.Y), e.Button);
        }

        /// <summary>
        /// Enable/disable the given game state
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="active"></param>
        public void SetGameState(GameState gameState, bool active)
        {
            GameState old =  GameState;

            if (active)
            {
                GameState |= gameState;
            } else
            {
                GameState &= ~gameState;
            }

            OnGameStateChange?.Invoke(old, GameState);
        }

        /// <summary>
        /// Check if the given game state is active
        /// </summary>
        /// <param name="gameState"></param>
        /// <returns></returns>
        public bool IsGameState(GameState gameState)
        {
            return (GameState & gameState) == gameState;
        }

        /// <summary>
        /// Check if any menu is open
        /// </summary>
        /// <returns></returns>
        public bool IsAnyMenuOpen()
        {
            return IsGameState(GameState.PauseMenu) || IsGameState(GameState.MainMenu);
        }

        /// <summary>
        /// Check if gameplay is currently running (not paused, etc)
        /// </summary>
        /// <returns></returns>
        public bool IsGameRunning()
        {
            return IsGameState(GameState.InGame) && !IsAnyMenuOpen();
        }

        /// <summary>
        /// Check if the game has ended or not
        /// </summary>
        /// <returns></returns>
        public bool HasGameEnded()
        {
            return IsGameState(GameState.EndWon) || IsGameState(GameState.EndLost);
        }

        /// <summary>
        /// Check if PvP is allowed in the current gametype
        /// </summary>
        /// <returns></returns>
        public bool IsPvPAllowed()
        {
            return GameType == GameType.AvA;
        }

        /// <summary>
        /// Calls `a` from the main game thread
        /// 
        /// Used for entity updates, logic, etc.
        /// Used as a bridge from networking thread
        /// </summary>
        /// <param name="a"></param>
        public static void CallThreaded(Action a)
        {
            Program.GameWindow.ThreadSafeActions.Enqueue(a);
        }

        /// <summary>
        /// Calls `a` from the main ui thread
        /// 
        /// Used for winforms calls. Invoke is an
        /// alternative but im not 100% sure how async that is
        /// 
        /// `a` is executed asynchronously when form is idle, there is no guarantee when it will be
        /// 
        /// If the action is supposed to be unique in the queue, that is, concurrent calls should not be
        /// stack but only the last one, unique key can be set so as to identify the unique action
        /// </summary>
        /// <param name="a"></param>
        public static void CallUIThread(Action a, int uniquenessKey = -1)
        {
            if (uniquenessKey >= 0)
            {
                Program.GameWindow.UIThreadSafeUniqueActions.AddOrUpdate(uniquenessKey, a, (_, _) => a);
            } else
            {
                Program.GameWindow.UIThreadSafeActions.Enqueue(a);
            }
        }

        /// <summary>
        /// Close all menus and prepare everything to start gameeplay
        /// </summary>
        public void StartGameplay()
        {
            if (MyPlayer == null)
            {
                return;
            }

            MyPlayer.Spawn();

            SetGameState(GameState.Loading, false);
            GameState = 0;

            SetGameState(GameState.InGame, true);
        }

        /// <summary>
        /// Closes the game window and (theoretically) returns to launcher
        /// </summary>
        public static void CloseGame(string reason = "")
        {
            if (reason.Length > 0)
            {
                MessageBox.Show("El juego se ha cerrado. Motivo: " + reason);
            }
            
            Logs.PrintDebug("Closing game");
            Environment.Exit(0);
        }

        /// <summary>
        /// Set local player data from launcher
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userId"></param>
        /// <param name="master"></param>
        public void SetLocalPlayer(string username, int userId, bool master)
        {
            MyUsername = username;
            MyId = userId;
            Master = master;

            Text += " | " + MyUsername + " | " + (Master ? "Master" : "Slave");
            Logs.PrintDebug("Decision role: " + (Master ? "Master" : "Slave"));

            // If the player is `master`,
            // then the `LocalPlayer` is created here and assigned Id = 0
            // If the player is not `master`,
            // then the `LocalPlayer` will be created during networking
            if (master)
            {
                MyPlayer = new LocalPlayer(MyUsername);
                MyPlayer.Create();
            }
        }

        /// <summary>
        /// This event is fired when the window is shown to the user,
        /// it is executed after `SetLocalPlayer` and `SetInitialData`
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNativeShownEvent(object? sender, EventArgs e)
        {
            // Notify server
            if (Master)
            {
                Logs.PrintDebug("Notify server game is starting");
                Network.Send("100/0/" + (int)GameType);
            }

            SoundManager.PlayIngame();

            // It's ok, just wait few ms
            //while (!SoundManager.IsPlayerReady()) ;

            gameThread = new Thread(GameLoop);
            gameThread.Priority = ThreadPriority.Highest;
            gameThread.IsBackground = true;
            gameThread.Start();

            // Show splash anim
            BeginInvoke(() =>
            {
                //introPb.Dock = DockStyle.Fill;
                //introPb.SizeMode = PictureBoxSizeMode.StretchImage;
                //introPb.Visible = true;

                //System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
                //timer1.Interval = 3000;

                //timer1.Tick += (object? sender, EventArgs e) =>
                //{
                //    introPb.Visible = false;
                //    introPb.Image.Dispose();
                //    introPb.Dispose();
                //    Controls.Remove(introPb);

                //    timer1.Stop();
                //    timer1.Dispose();

                //    gameThread = new Thread(GameLoop);
                //    gameThread.Priority = ThreadPriority.Highest;
                //    gameThread.IsBackground = true;
                //    gameThread.Start();
                //};

                //timer1.Start();
            });
        }

        /// <summary>
        /// Set initial data from launcher
        /// </summary>
        /// <param name="data"></param>
        /// <param name="gameType"></param>
        public void SetInitialData(List<string> playerUsernames, int gameType)
        {
            GameType = (GameType)gameType;
            Logs.PrintDebug("Playing mode: " + GameType);

            if (Master)
            {
                List<Vector2D> teamPlayerSpawns = new List<Vector2D>()
                {
                    new Vector2D(100, 100),
                    new Vector2D(200, 100),
                    new Vector2D(300, 100),
                    new Vector2D(400, 100),
                    new Vector2D(500, 100),
                };

                switch (GameType)
                {
                    case GameType.Sandbox:
                    case GameType.Coop:
                        for (int i = 0; i < playerUsernames.Count; i++)
                        {
                            PlayerSpawnData.Add(new PlayerSpawnData(playerUsernames[i], teamPlayerSpawns[i].X, teamPlayerSpawns[i].Y));
                        }
                        break;
                    case GameType.AvA:
                        Random r = new Random();
                        foreach (string username in playerUsernames)
                        {
                            float x = r.NextSingle() * World.Width;
                            float y = r.NextSingle() * World.Height;
                            PlayerSpawnData.Add(new PlayerSpawnData(username, x, y));
                        }
                        break;
                }
            }
        }

        private void OnNativeMouseMove(object? sender, MouseEventArgs e)
        {
            MouseCoords = new Vector2D(e.X, e.Y);
        }

        private void OnNativeKeyUp(object? sender, KeyEventArgs e)
        {
            OnGameKeyUp?.Invoke(e);
        }

        private void OnNativeKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (IsGameRunning())
                {
                    SetGameState(GameState.PauseMenu, true);
                } else if (IsGameState(GameState.PauseMenu))
                {
                    Menus.PauseMenu.Hide();
                    SetGameState(GameState.PauseMenu, false);
                }
            }

            OnGameKeyDown?.Invoke(e);
        }

        /// <summary>
        /// Perform game update logic
        /// </summary>
        private void GameUpdate()
        {
            // Execute timers first for better accuracy
            TimerManager.Update();

            // Execute queued threadsafe action
            while (!ThreadSafeActions.IsEmpty)
            {
                ThreadSafeActions.TryDequeue(out Action? a);
                a?.Invoke();
            }

            if (GameType == GameType.AvA || IsGameRunning())
            {
                EntityManager.Update();
            }

            UiManager.Update();

            if (IsGameRunning() && SoundManager.IsPlaying())
            {
                Menus.MusicPlayer.Update();
            }

            if (IsGameState(GameState.PauseMenu))
            {
                Menus.PauseMenu.Update();
            }

            if (LocalPlayer.SpecatorMode)
            {
                LocalPlayer.FocusSpectatedPlayer();
            }

            // Check game state if we are `Master`
            if (Master)
            {
                // When only one player remains alive, we check who has lost and won the game
                // We will notify the server, and it will forward the state to all players, including us
                if (EntityManager.GetPlayerCount() <= 1 && !HasGameEnded())
                {
                    List<Entity> entities = EntityManager.GetAllSpawned();
                    Player? winner = null;

                    foreach (Entity e in entities)
                    {
                        if (e is not Player)
                        {
                            continue;
                        }

                        winner = e as Player;
                        break;
                    }

                    // Wtf?
                    if (winner == null)
                    {
                        Logs.PrintDebug("Winner is null, wtf?");
                        return;
                    }

                    // Should not be done here, but we need to quit check condition
                    if (MyPlayer == null || MyPlayer.Id != winner.Id)
                    {
                        SetGameState(GameState.EndLost, true);
                    } else
                    {
                        SetGameState(GameState.EndWon, true);
                    }

                    Network.Send("100/200/" + winner.Name);
                }
            }
        }

        /// <summary>
        /// Perform render logic and draw to `g` target
        /// Then, swap buffers
        /// </summary>
        /// <param name="g"></param>
        private void GameRender(Graphics g)
        {
            // Increment number of frames this second
            currentFrames++;

            // Clear the screen (draw black)
            // Equivalent of invalidating with `Invalidate`
            g.FillRectangle(Brushes.Black, 0, 0, Width, Height);

            // Menu rendering
            // Depending on the current states some menus should be rendered
            {
                if (IsGameState(GameState.PauseMenu))
                {
                    Menus.PauseMenu.Show();
                }

                if (IsGameState(GameState.MainMenu))
                {
                    Menus.MainMenu.Show();
                }

                if (IsGameState(GameState.Loading))
                {
                    Menus.Loading.Show();
                }

                if (IsGameRunning() && SoundManager.IsPlaying())
                {
                    Menus.MusicPlayer.Show();
                }
                else
                {
                    Menus.MusicPlayer.Hide();
                }
            }

            // Render the game itself (World + Entities)
            if (IsGameRunning())
            {
                World.Render(g);
                EntityManager.Render(g);

                g.DrawString("geometry wars | " + GameType.ToString(), Utils.Ui.DebugFont, Utils.Ui.WhiteBrush, 5, 15);
                g.DrawString("fps: " + fps, Utils.Ui.DebugFont, Utils.Ui.WhiteBrush, 5, 35);
                g.DrawString("entity count: " + EntityManager.GetCount(), Utils.Ui.DebugFont, Utils.Ui.WhiteBrush, 5, Height - 75);
            }

            // Render user interface and alerts
            UiManager.Render(g);
            NotificationManager.Render(g);

            if (HasGameEnded())
            {
                Menus.EndMenu.Render(g);
            }

            // Swap buffers between `g` (which is allocated each time we render, is this even good?) and window buffer
            // Then, dispose current buffered graphics
            if (bGraphics != null)
            {
                bGraphics.Render();
                bGraphics.Dispose();
            }

            // Perform a GC collection, im not even sure if this is good but
            // memory usage is lower, updates per second are constant and fps are ok
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Default, false);
            GC.WaitForPendingFinalizers();
        }
    }

    /// <summary>
    /// Enumeration for key Ids,
    /// the fact that they are bitfields allows us to have multiple
    /// concurrent key states efficiently
    /// </summary>
    [Flags]
    public enum KeyId : uint
    {
        MOVE_UP = 1 << 0,
        MOVE_DOWN = 1 << 1,
        MOVE_LEFT = 1 << 2,
        MOVE_RIGHT = 1 << 3,

        SHOOT = 1 << 4,
    }

    /// <summary>
    /// Enumeration for game states
    /// the fact that they are bitfields allows us to have multiple
    /// concurrent game states efficiently
    /// </summary>
    [Flags]
    public enum GameState : uint
    {
        MainMenu = 1 << 0,
        InGame = 1 << 1,
        PauseMenu = 1 << 2,
        Loading = 1 << 3,

        EndWon = 1 << 4,
        EndLost = 1 << 5,
    }

    public enum GameType : uint
    {
        /// <summary>
        /// Sandbox mode just move without enemies or objectives
        /// </summary>
        Sandbox = 0,

        /// <summary>
        /// All vs All, all players against all players
        /// </summary>
        AvA = 1,

        /// <summary>
        /// Co-op, players vs ai
        /// </summary>
        Coop = 2
    }

    /// <summary>
    /// Helper struct for players initial spawn
    /// </summary>
    public struct PlayerSpawnData
    {
        /// <summary>
        /// Player name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Where the player will be spawned
        /// </summary>
        public Vector2D Position { get; set; }

        public PlayerSpawnData(string name, float x, float y)
        {
            Name = name;
            Position = new Vector2D(x, y);
        }
    }
}
