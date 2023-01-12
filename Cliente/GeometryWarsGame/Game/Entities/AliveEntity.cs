using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Entities
{
    public abstract class AliveEntity : Entity
    {
        /// <summary>
        /// Health of the entity
        /// </summary>
        public float Health { get; set; } = 100;

        /// <summary>
        /// If the entity is dead
        /// </summary>
        public bool Dead => Health <= 0;

        /// <summary>
        /// If the entity is already flagged as dead
        /// Just to make sure we dont kill an entity two times!
        /// </summary>
        public bool MarkedAsDead { get; set; } = false;

        public AliveEntity(): base()
        {

        }

        public AliveEntity(Vector2D position, int id) : this()
        {
            Id = id;
            Position = position;
        }

        public AliveEntity(int id) : this()
        {
            Id = id;
        }

        public AliveEntity(Vector2D position) : this()
        {
            Position = position;
        }
    }
}
