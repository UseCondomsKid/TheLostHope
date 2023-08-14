using Humper;
using LDtk;
using LDtkTypes;
using LostHope.Engine.ContentLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.GameCode.Rooms
{
    public class Room
    {
        public struct RoomData
        {
            public LDtkLevel LevelData;
            public World PhysicsWorld;
            public LDtkPlayer PlayerData;
            public List<LDtkEnemy> EnemyDataList;
            public List<LDtkGun> GunDataList;
            public List<LDtkLevelTransition> LevelTransitionDataList;
        }

        public RoomData Initialize(string levelName)
        {
            RoomData data = new RoomData();

            // Load the world, this should happen somewhere else I feel like, but
            // let's keep it here for now. We also need to load different worlds if I ever make multiple ones
            // instead of hard coding the one here.
            LDtkWorld world = ContentLoader.LDtkFile.LoadWorld(Worlds.World.Iid);

            // Load the level
            data.LevelData = world.LoadLevel(levelName);

            // Debug and check
            Console.Write($"Level: {levelName}. Loaded? : {data.LevelData.Loaded}");
            if (!data.LevelData.Loaded) return data;

            // Load entities
            data.PlayerData = data.LevelData.GetEntity<LDtkPlayer>();

            data.EnemyDataList = new List<LDtkEnemy>();
            foreach (var e in data.LevelData.GetEntities<LDtkEnemy>())
            {
                data.EnemyDataList.Add(e);
            }

            data.GunDataList = new List<LDtkGun>();
            foreach (var g in data.LevelData.GetEntities<LDtkGun>())
            {
                data.GunDataList.Add(g);
            }

            data.LevelTransitionDataList = new List<LDtkLevelTransition>();
            foreach (var t in data.LevelData.GetEntities<LDtkLevelTransition>())
            {
                data.LevelTransitionDataList.Add(t);
            }

            // Physics
            LDtkIntGrid collisions = data.LevelData.GetIntGrid("Collisions");
            data.PhysicsWorld = new World(data.LevelData.Size.X, data.LevelData.Size.Y, collisions.GridSize.X);

            // Spawn all tiles
            for (int x = 0; x < data.LevelData.; x++)
            {
                for (int y = 0; y < bottomRightGrid.Y; y++)
                {
                    long intGridValue = collisions.GetValueAt(x, y);
                    if (intGridValue is 6 or 7)
                    {
                        tiles.Add(new Box(level.Position.ToVector2() + new Vector2(x * collisions.TileSize, y * collisions.TileSize), new Vector2(collisions.TileSize), Vector2.Zero));
                    }
                }
            }


            return data;
        }
    }
}
