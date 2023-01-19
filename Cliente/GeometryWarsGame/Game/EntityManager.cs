using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometryWarsGame.Game.Entities;
using System.Collections.Concurrent;

namespace GeometryWarsGame.Game
{
    public static class EntityManager
    {
        /// <summary>
        /// List of entities related by their Id
        /// </summary>
        private static readonly ConcurrentDictionary<int, Entity> entities = new ConcurrentDictionary<int, Entity>();

        /// <summary>
        /// Get the first free id in the entity pool
        /// </summary>
        /// <returns></returns>
        public static int GetFreeId()
        {
            int id = 0;
            while (entities.ContainsKey(id)) id++;
            return id;
        }

        /// <summary>
        /// Get total entity count in pool
        /// </summary>
        /// <returns></returns>
        public static int GetCount()
        {
            return entities.Count;
        }

        /// <summary>
        /// Get total count of players
        /// </summary>
        /// <returns></returns>
        public static int GetPlayerCount()
        {
            int count = 0;

            foreach (Entity e in entities.Values)
            {
                if (e is Player) count++;
            }

            return count;
        }

        /// <summary>
        /// Add given entity to the entity pool
        /// </summary>
        /// <param name="entity"></param>
        public static void AddEntity(Entity entity)
        {
            if (!entities.TryAdd(entity.Id, entity))
            {
                return;
            }

            Logs.PrintDebug("Added entity (" + entity.Id + ", " + entity + ") to entity pool");
        }

        /// <summary>
        /// Remov the given entity from the entity pool
        /// </summary>
        /// <param name="entity"></param>
        public static void RemoveEntity(Entity entity)
        {
            if (!entities.TryRemove(entity.Id, out _))
            {
                return;
            }

            Logs.PrintDebug("Removed entity (" + entity.Id + ") from entity pool");
        }

        /// <summary>
        /// Get the entity by their Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Entity? GetById(int id)
        {
            return entities!.GetValueOrDefault(id, null);
        }

        /// <summary>
        /// Get the player by their name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Player? GetPlayerByName(string name)
        {
            Player? result = null;

            foreach (Entity e in entities.Values)
            {
                if (e is not Player)
                {
                    continue;
                }

                if ((e as Player)!.Name == name)
                {
                    result = e as Player;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Get all entities in the pool
        /// </summary>
        /// <returns></returns>
        public static List<Entity> GetAll()
        {
            return entities.Values.ToList();
        }

        /// <summary>
        /// Get all the entities in the pool which are spawned (includes destroyable)
        /// </summary>
        /// <returns></returns>
        public static List<Entity> GetAllSpawned()
        {
            List<Entity> result = new List<Entity>();

            foreach (Entity entity in entities.Values)
            {
                if (entity.State == EntityState.Spawned || entity.State == EntityState.Destroyable)
                {
                    result.Add(entity);
                }
            }

            return result;
        }

        /// <summary>
        /// Get all the alive entities in the pool which are spawned (includes destroyable)
        /// </summary>
        /// <returns></returns>
        public static List<AliveEntity> GetAllAliveSpawned()
        {
            List<AliveEntity> result = new List<AliveEntity>();
            foreach (Entity e in GetAll())
            {
                if ((e.State == EntityState.Spawned || e.State == EntityState.Destroyable) && e is AliveEntity)
                {
                    result.Add((AliveEntity)e);
                }
            }

            return result;
        }

        public static void Render(Graphics g)
        {
            List<Entity> list = GetAllSpawned();
            
            for (int i = list.Count - 1; i >= 0; i--)
            {
                Vector2D refCoord = list[i].GetReferenceCoordinate();

                if (!World.IsWorldCoordinateVisible(refCoord.X, refCoord.Y))
                {
                    continue;
                }

                list[i].Render(g);
            }
        }

        public static void Update()
        {
            List<Entity> list = GetAllSpawned();

            for (int i = list.Count - 1; i >= 0; i--)
            {
                Entity e = list[i];

                if (e.State == EntityState.Destroyable)
                {
                    e.Destroy();
                    continue;
                }

                e.Update();
            }
        }
    }
}
