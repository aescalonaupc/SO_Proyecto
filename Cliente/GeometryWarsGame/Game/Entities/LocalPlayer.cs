using GeometryWarsGame.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Entities
{
    public class LocalPlayer : Player
    {
        private KeyId keyStates = 0;

        private long lastTimeShoot = Time.GetReferenceMillis();

        public LocalPlayer() : base()
        {
            Window.OnGameKeyUp += OnKeyUp;
            Window.OnGameKeyDown += OnKeyDown;

            // Enemies have a different color in all vs all mode
            // force local player to be player color
            if (Program.GameWindow.GameType == GameType.AvA)
            {
                Color = Utils.Ui.PlayerColor;
            }
        }

        public LocalPlayer(int id, string name) : this()
        {
            Id = id;
            Name = name;
        }

        public LocalPlayer(string name) : this()
        {
            Name = name;
        }

        private void OnKeyDown(KeyEventArgs e)
        {
            if (!Program.GameWindow.IsGameRunning())
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.W:
                    keyStates |= KeyId.MOVE_UP;
                    break;
                case Keys.S:
                    keyStates |= KeyId.MOVE_DOWN;
                    break;
                case Keys.D:
                    keyStates |= KeyId.MOVE_RIGHT;
                    break;
                case Keys.A:
                    keyStates |= KeyId.MOVE_LEFT;
                    break;
                case Keys.Space:
                    keyStates |= KeyId.SHOOT;
                    break;
            }
        }

        private void OnKeyUp(KeyEventArgs e)
        {
            if (!Program.GameWindow.IsGameRunning())
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.W:
                    keyStates &= ~KeyId.MOVE_UP;
                    break;
                case Keys.S:
                    keyStates &= ~KeyId.MOVE_DOWN;
                    break;
                case Keys.D:
                    keyStates &= ~KeyId.MOVE_RIGHT;
                    break;
                case Keys.A:
                    keyStates &= ~KeyId.MOVE_LEFT;
                    break;
                case Keys.Space:
                    keyStates &= ~KeyId.SHOOT;
                    break;
            }
        }

        private bool IsKeyActive(KeyId key)
        {
            return (keyStates & key) == key;
        }

        long lastSync = Time.GetReferenceMillis();
        public override void Update()
        {
            if (State == EntityState.Destroyed)
            {
                return;
            }

            if (!Dead)
            {
                if (IsKeyActive(KeyId.MOVE_UP))
                {
                    if (Position.Y - Scale / 2 > 0)
                    {
                        Position.Y -= Velocity;
                        World.MoveCamera(CameraMovementDirection.Up);
                    }
                }

                if (IsKeyActive(KeyId.MOVE_DOWN))
                {
                    if (Position.Y + Scale / 2 < World.Height)
                    {
                        Position.Y += Velocity;
                        World.MoveCamera(CameraMovementDirection.Down);
                    }
                }

                if (IsKeyActive(KeyId.MOVE_RIGHT))
                {
                    if (Position.X + Scale / 2 < World.Width)
                    {
                        Position.X += Velocity;
                        World.MoveCamera(CameraMovementDirection.Right);
                    }
                }

                if (IsKeyActive(KeyId.MOVE_LEFT))
                {
                    if (Position.X - Scale / 2 > 0)
                    {
                        Position.X -= Velocity;
                        World.MoveCamera(CameraMovementDirection.Left);
                    }
                }

                if (IsKeyActive(KeyId.SHOOT))
                {
                    if (Time.GetReferenceMillis() - lastTimeShoot > 250)
                    {
                        Shoot();
                        lastTimeShoot = Time.GetReferenceMillis();
                    }
                }

                Vector2D mousePosition = Program.GameWindow.MouseCoords;
                Vector2D mouseVector = new Vector2D(mousePosition.X - CamPosition.X, mousePosition.Y - CamPosition.Y);
                double angle = Math.Atan(mouseVector.Y / mouseVector.X);

                if (mouseVector.X < 0)
                {
                    angle += Math.PI;
                }

                float f = (float)Maths.RadiansToDegrees(angle);

                if (!float.IsNaN(f))
                {
                    Heading = f;
                }

                if (Time.GetReferenceMillis() - lastSync > 10)
                {
                    Network.Send("100/5/" + Id + "/" + Position.X + "/" + Position.Y + "/" + Heading);
                    lastSync = Time.GetReferenceMillis();
                }

                World.SetFocusPosition(Position);
            }

            base.Update();
        }

        public override void Render(Graphics g)
        {
            if (Dead)
            {
                return;
            }

            //g.DrawString("(debug) player position " + Position.ToString(), Utils.Ui.DebugFont, Utils.Ui.WhiteBrush, 5, 55);
            //g.DrawString("(debug) player cam position " + CamPosition.ToString(), Utils.Ui.DebugFont, Utils.Ui.WhiteBrush, 5, 75);
            //g.DrawString("(debug) player heading " + Heading, Utils.Ui.DebugFont, Utils.Ui.WhiteBrush, 5, 100);
            //g.DrawLine(new Pen(Color.Gray), CamPosition.X, CamPosition.Y, Program.GameWindow.MouseCoords.X, Program.GameWindow.MouseCoords.Y);

            base.Render(g);
        }

        private static volatile int specId = -1;
        public static volatile bool SpecatorMode = false;
        public static void StartSpecMode()
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                Logs.PrintDebug("Starting spectator mode from ThreadPool");
                SpecatorMode = true;

                while (SpecatorMode)
                {
                    List<AliveEntity> ents = EntityManager.GetAllAliveSpawned();
                    Random r = new Random();

                    int id = -1;
                    while (ents.Count > 1 && (id == specId || id == -1)) id = ents[r.Next(ents.Count)].Id;

                    if (ents.Count > 1)
                    {
                        specId = id;

                        NotificationManager.Notify("Observando " + ((Player)EntityManager.GetById(specId)!).Name, 3);
                        Thread.Sleep(7_000);
                    }
                }
            });
        }

        public static void StopSpectatorMode()
        {
            SpecatorMode = false;
        }

        public static void FocusSpectatedPlayer()
        { 
            if (SpecatorMode)
            {
                Entity? p = (Entity?)EntityManager.GetById(specId);

                if (p != null && p is Player)
                {
                    World.SetFocusPosition(p.Position);
                }
            }
        }
    }
}
