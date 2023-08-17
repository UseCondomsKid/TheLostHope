using Humper;
using LDtk;
using LDtkTypes;
using LostHope.Engine.ContentLoading;
using LostHope.Engine.Rendering;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LostHope.GameCode.Rooms
{
    public struct RoomData
    {
        public LDtkLevel LevelData;
        public ModifiedLDtkRenderer LevelRenderer;
        public World PhysicsWorld;
        public LDtkPlayer PlayerData;
        public List<LDtkEnemy> EnemyDataList;
        public List<LDtkGun> GunDataList;
        public List<LDtkLevelTransition> LevelTransitionDataList;
    }
    public class Room
    {
        public RoomData Initialize(string levelName)
        {
            RoomData data = new RoomData();

            // Load the world, this should happen somewhere else I feel like, but
            // let's keep it here for now. We also need to load different worlds if I ever make multiple ones
            // instead of hard coding the one here.
            LDtkWorld world = ContentLoader.LDtkFile.LoadWorld(Worlds.World.Iid);

            // Load the level
            data.LevelData = world.LoadLevel(levelName);

            // Create the renderer
            data.LevelRenderer = new ModifiedLDtkRenderer(Globals.SpriteBatch, ContentLoader.Content);
            // Pre-render the level
            data.LevelRenderer.PrerenderLevel(data.LevelData);

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
            data.PhysicsWorld = new World(data.LevelData.Size.X, data.LevelData.Size.Y, collisions.TileSize);

            // Spawn all tiles
            for (int x = 0; x < (data.LevelData.Size.X / collisions.TileSize); x++)
            {
                for (int y = 0; y < (data.LevelData.Size.Y / collisions.TileSize); y++)
                {
                    long intGridValue = collisions.GetValueAt(x, y);
                    if (intGridValue is 1)
                    {
                        IBox tile = data.PhysicsWorld.Create(x * collisions.TileSize, y * collisions.TileSize,
                            collisions.TileSize, collisions.TileSize);
                        tile.AddTags(CollisionTags.Ground);
                    }
                }
            }

            return data;
        }
    }
}
