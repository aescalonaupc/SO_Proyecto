using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Entities
{
    public class Bullet : Entity, IAutoSpawn
    {
        /// <summary>
        /// Width of the bullet sprite
        /// </summary>
        public int Width = 10;

        /// <summary>
        /// Height of the bullet sprite
        /// </summary>
        public int Height = 5;

        /// <summary>
        /// Velocity of the bullet
        /// </summary>
        public float Velocity = 10;

        /// <summary>
        /// Heading of the bullet
        /// </summary>
        public float Heading { get; set; }

        /// <summary>
        /// Brush (color) to paint the bullet
        /// </summary>
        public Brush DrawingBrush { get; set; } = Utils.Ui.WhiteBrush;

        /// <summary>
        /// Damage of the bullet
        /// </summary>
        public int Damage { get; set; } = 25;

        public Bullet(float heading, Vector2D position): base(position)
        {
            Heading = heading;
        }

        public override void OnCreate()
        {
            if (!Program.GameWindow.IsPvPAllowed() && Creator != null && Creator is Player)
            {
                DrawingBrush = Utils.Ui.GrayPen.Brush;
            }
        }

        public override void Render(Graphics g)
        {
            {
                g.TranslateTransform(CamPosition.X, CamPosition.Y);
                g.RotateTransform(Heading);

                g.FillRectangle(DrawingBrush, 0, 0, Width, Height);

                g.ResetTransform();
            }
        }

        public override void Update()
        {
            if (State == EntityState.Destroyed)
            {
                return;
            }

            Position.X += (float)(Velocity * Math.Cos(Utils.Maths.DegreesToRadians(Heading)));
            Position.Y += (float)(Velocity * Math.Sin(Utils.Maths.DegreesToRadians(Heading)));

            if (Position.X > World.Width || Position.X < 0 || Position.Y > World.Height|| Position.Y < 0)
            {
                MarkForDestroy();
                return;
            }

            if (Program.GameWindow.Master)
            {
                List<AliveEntity> entities = EntityManager.GetAllAliveSpawned();
                double radius = Math.Pow(Player.Scale + 1, 2);
                foreach (AliveEntity e in entities)
                {
                    // avoid dead entities
                    if (e.Dead)
                    {
                        continue;
                    }

                    // avoid creator of bullet
                    if (e == Creator)
                    {
                        continue;
                    }

                    // must be close to bullet
                    if (Utils.Maths.DistanceFast(Position, e.Position) > radius)
                    {
                        continue;
                    }

                    // bullets in sandbox and coop do not affect players!
                    if (!Program.GameWindow.IsPvPAllowed() && e is Player)
                    {
                        continue;
                    }

                    // apply damage, notify new health and destroy bullet
                    e.Health -= Damage;
                    MarkForDestroy();
                    Network.Send("100/7/" + e.Id + "/" + e.Health + "$100/8/" + Id);
                }
            }
        }
    }
}
