using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometryWarsGame.Game.Entities
{
    /// <summary>
    /// Entity lifecycle:
    /// () -> created -> spawned -> exists -> marked as destroyable -> destroyed -> ()
    /// </summary>
    public abstract class Entity
    {
        public int Id { get; set; } = -1;

        /// <summary>
        /// Entity position with respect to world coordinate system
        /// </summary>
        public Vector2D Position { get; set; } = Vector2D.Zero();

        /// <summary>
        /// Entity position with respect to camera coordinate system
        /// </summary>
        public Vector2D CamPosition => World.FromWorldCoordinateToCam(Position);

        /// <summary>
        /// State of the entity now
        /// </summary>
        public EntityState State { get; set; } = EntityState.None;

        /// <summary>
        /// Creator of the entity, if any
        /// </summary>
        public Entity? Creator { get; set; } = null;

        public abstract void Render(Graphics g);

        public abstract void Update();

        /// <summary>
        /// Fired when the entity is created
        /// </summary>
        public virtual void OnCreate() { }

        /// <summary>
        /// Fired when the entity is spawned
        /// </summary>
        public virtual void OnSpawn() { }

        public Entity()
        {

        }

        public Entity(Vector2D position, int id) : this()
        {
            Id = id;
            Position = position;
        }

        public Entity(int id) : this()
        {
            Id = id;
        }

        public Entity(Vector2D position) : this()
        {
            Position = position;
        }

        /// <summary>
        /// Creates the entity and is added to entity pool
        /// That is, ensures all fields are valid and the entity will be properly spawned
        /// If the entity is `IAutoSpawn`, it is spawned too
        /// </summary>
        public void Create()
        {
            // If the entity does not have an id at `Create`, it is assigned randomly
            if (Id == -1)
            {
                Id = EntityManager.GetFreeId();
            }

            State = EntityState.Created;
            EntityManager.AddEntity(this);

            if (this is IAutoSpawn)
            {
                Spawn();
            }
        }

        /// <summary>
        /// Spawns the entity in the world
        /// </summary>
        public void Spawn()
        {
            State = EntityState.Spawned;
        }

        /// <summary>
        /// Destroy the entity from the world
        /// </summary>
        public void Destroy()
        {
            EntityManager.RemoveEntity(this);
            State = EntityState.Destroyed;

            if (Program.GameWindow.MyPlayer != null && Id == Program.GameWindow.MyPlayer.Id)
            {
                Program.GameWindow.MyPlayer = null;
            }
        }

        /// <summary>
        /// Mark the entity to be destroyed in next tick
        /// </summary>
        public void MarkForDestroy()
        {
            State = EntityState.Destroyable;
        }

        /// <summary>
        /// Returns the reference coordinate of the entity to take into account for different computations
        /// </summary>
        /// <returns></returns>
        public virtual Vector2D GetReferenceCoordinate()
        {
            return Position;
        }

    }

    /// <summary>
    /// Helper interface for entities which are automatically spawned as soon as they are created
    /// </summary>
    public interface IAutoSpawn
    {
        
    }

    /// <summary>
    /// Enum of possible entity states
    /// </summary>
    public enum EntityState
    {
        None,
        Created,
        Spawned,
        Destroyable,
        Destroyed
    }
}
