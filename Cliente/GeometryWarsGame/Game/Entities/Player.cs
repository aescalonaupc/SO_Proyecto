using GeometryWarsGame.Game.Utils;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Entities
{
    public class Player : AliveEntity
    {
        /// <summary>
        /// velocity at which the player moves
        /// </summary>
        protected static float Velocity { get; set; } = 4.0f;

        /// <summary>
        /// Size of the player sprite
        /// </summary>
        public static float Scale { get; set; } = 24;

        /// <summary>
        /// Color of the player sprite
        /// </summary>
        public Color Color { get; set; } = Utils.Ui.PlayerColor;

        /// <summary>
        /// Brush used to paint the player from color
        /// </summary>
        private Brush Brush => new Pen(Color).Brush;

        /// <summary>
        /// Name of the player
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Heading of the player
        /// </summary>
        public float Heading { get; set; } = 0;

        /// <summary>
        /// Number of available lifes
        /// </summary>
        public int Lifes { get; set; } = 3;

        public Player() : base()
        {
            // Enemies have a different color in all vs all mode
            if (Program.GameWindow.GameType == GameType.AvA)
            {
                Color = Utils.Ui.EnemyColor;
            }
        }

        public Player(string name) : this()
        {
            Name = name;
        }

        public Player(string name, float x, float y) : this()
        {
            Name = name;
            Position = new Vector2D(x, y);
        }

        public Player(int id, string name, float x, float y) : this(name, x, y)
        {
            Id = id;
        }

        /// <summary>
        /// Shoot a buller from player position and heading
        /// </summary>
        public void Shoot()
        {
            Network.Send("100/6/" + Id + "/" + Heading + "/" + Position.X + "/" + Position.Y);
        }

        public override Vector2D GetReferenceCoordinate()
        {
            return new Vector2D(Position.X + Scale / 2, Position.Y + Scale / 2);
        }

        public override void Update()
        {
            if (State == EntityState.Destroyed)
            {
                return;
            }

            if (Program.GameWindow.Master)
            {
                if (Dead && !MarkedAsDead)
                {
                    // The player has died
                    Lifes--;
                    MarkedAsDead = true;

                    Logs.PrintDebug("Detected player death " + Name + " (" + Id + "), remaining lifes " + Lifes);

                    // If the player doesn't have enough lifes,
                    // notify about the permanent death and lose
                    if (Lifes <= 0)
                    {
                        Network.Send("100/9/" + Id);
                        return;
                    }

                    // If the player has enough lifes,
                    // notify about the death and remaining lifes
                    Network.Send("100/10/" + Id + "/" + Lifes);

                    // Note: on networking thread, as soon as we receive
                    // the ack of player death (packet 10), we will cause player respawn
                }
            }
        }

        
        public override void Render(Graphics g)
        {
            if (Dead)
            {
                return;
            }

            // Nametag
            g.DrawString(Name + " (x" + Lifes + ")", Utils.Ui.NametagFont, Utils.Ui.WhiteBrush, CamPosition.X - Scale / 2 - 3, CamPosition.Y - Scale - 30);

            // Health bar
            {
                g.FillRectangle(Utils.Ui.RedBrush, CamPosition.X - Scale / 2 - 1, CamPosition.Y - Scale - 11, 32, 7);

                const int totalHealth = 100;
                const int totalWidth = 30;
                int width = (int)(((double)Health / totalHealth) * totalWidth);
                g.FillRectangle(Utils.Ui.GreenBrush, CamPosition.X - Scale / 2, CamPosition.Y - Scale - 10, width, 5);
            }

            // Player render
            {
                // Rotate graphics by player heading and set origin to player coords
                g.TranslateTransform(CamPosition.X, CamPosition.Y);
                g.RotateTransform(Heading);

                // Base
                g.FillRectangle(Brush, -Scale / 2, -Scale / 2, Scale, Scale);

                // Cannon
                float cannonWidth = 20f;
                float cannonHeight = 5f;
                g.FillRectangle(Utils.Ui.WhitePen.Brush, 0, -cannonHeight / 2, cannonWidth, cannonHeight);

                // Reset rotation and origin translation
                g.ResetTransform();
            }
        }
    }
}
