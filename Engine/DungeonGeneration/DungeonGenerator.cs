using Humper;
using LostHope.Engine.Input;
using LostHope.Engine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.Engine.DungeonGeneration
{
    public class Node
    {
        public Rectangle rect;
        public bool isMain;
        public string roomId;
        // etc...
    }

    public class DungeonGenerator
    {
        private enum DungeonDirection
        {
            // None for random direction
            None, Up, Down, Left, Right
        }

        // Properties
        // These properties should be in the region
        // private int _tileSize = 32;
        private Point _spawnRadiusXY = new Point(300, 300);
        private Point _mainRoomNumMinMax = new Point(15, 20);
        private Point _mainRoomWidthMinMax = new Point(35, 60);
        private Point _mainRoomHeightMinMax = new Point(35, 60);

        private DungeonDirection _startRoomPos = DungeonDirection.Up;
        private DungeonDirection _endRoomPos = DungeonDirection.Down;

        private Random random;


        private List<Rectangle> _separatedRooms;

        public DungeonGenerator()
        {
            random = new Random();
        }

        private Point GetRandomPointInEllipse(int radiusX, int radiusY)
        {
            double angle = 2 * Math.PI * random.NextDouble();
            double distance = Math.Sqrt(random.NextDouble());
            int x = (int)(radiusX * distance * Math.Cos(angle));
            int y = (int)(radiusY * distance * Math.Sin(angle));

            return new Point(x, y);
        }

        private List<Rectangle> SeparateRooms(List<Rectangle> rooms)
        {
            var separatedRooms = new List<Rectangle>(rooms);
            do
            {
                for (int current = 0; current < separatedRooms.Count; current++)
                {
                    for (int other = 0; other < separatedRooms.Count; other++)
                    {
                        if (current == other || !separatedRooms[current].Intersects(separatedRooms[other])) continue;

                        var direction = separatedRooms[other].Center - separatedRooms[current].Center;
                        direction = new Point(direction.X > 1 ? 1 : 0,
                            direction.Y > 1 ? 1 : 0);

                        separatedRooms[current] = new Rectangle(separatedRooms[current].Location - direction,
                            separatedRooms[current].Size);
                        separatedRooms[other] = new Rectangle(separatedRooms[other].Location + direction,
                            separatedRooms[other].Size);
                    }
                }
            }
            while (IsAnyRoomOverlapped(separatedRooms));

            return separatedRooms;
        }

        private bool IsAnyRoomOverlapped(List<Rectangle> rooms)
        {
            bool overlap = false;
            for (int current = 0; current < rooms.Count; current++)
            {
                if (overlap) break;
                for (int other = 0; other < rooms.Count; other++)
                {
                    if (current == other || !rooms[current].Intersects(rooms[other])) continue;
                    overlap = rooms[current].Intersects(rooms[other]);
                    if (overlap) break;
                }
            }
            return overlap;
        }

        public void GenerateDungeon()
        {
            // Number of rooms to generate
            int roomNum = random.Next(_mainRoomNumMinMax.X, _mainRoomNumMinMax.Y + 1);

            _separatedRooms = null;

            // Generate rooms
            var rooms = new List<Rectangle>();
            for (int i = 0; i < roomNum; i++)
            {
                Point pos = GetRandomPointInEllipse(_spawnRadiusXY.X, _spawnRadiusXY.Y);
                int roomW = random.Next(_mainRoomWidthMinMax.X, _mainRoomWidthMinMax.Y + 1);
                int roomH = random.Next(_mainRoomHeightMinMax.X, _mainRoomHeightMinMax.Y + 1);
                rooms.Add(new Rectangle(pos.X, pos.Y, roomW, roomH));
            }

            // Seperate Rooms
            _separatedRooms = SeparateRooms(rooms);

            // TODO: Maybe seperate the rooms a little more based on some parameter
            // TODO: Triangulate
            // TODO: Do the minimum spanning tree thing and add the loops
            // TODO: Figure out how to create the graph
            // TODO: Pick a start and an end room based on the positions and maybe the graph
            // TODO: Change rooms with actual room templates based on the graph, change the sizes,
            //       and resolve any collisions
        }

        public void DebugUpdate(float delta)
        {
            // Camera Movement
            if (InputManager.IsKeyDown(Keys.D))
            {
                Globals.GameCamera.Position += new Vector2(50f, 0) * delta;
            }
            else if (InputManager.IsKeyDown(Keys.A))
            {
                Globals.GameCamera.Position -= new Vector2(50f, 0) * delta;
            }
            if (InputManager.IsKeyDown(Keys.W))
            {
                Globals.GameCamera.Position -= new Vector2(0, 50f) * delta;
            }
            else if (InputManager.IsKeyDown(Keys.S))
            {
                Globals.GameCamera.Position += new Vector2(0, 50f) * delta;
            }
            // Camera Zoom in and out
            if (InputManager.IsKeyDown(Keys.Up))
            {
                Globals.GameCamera.Zoom += 0.5f * delta;
            }
            else if (InputManager.IsKeyDown(Keys.Down))
            {
                Globals.GameCamera.Zoom -= 0.5f * delta;
            }

            // Testing
            if (InputManager.KeyPressed(Keys.G))
            {
                GenerateDungeon();
            }
        }
        public void DebugDraw()
        {
            var sb = Globals.SpriteBatch;

            sb.Begin(transformMatrix: Globals.GameCamera.Transform);

            // Drawing the ellispse
            sb.DrawEllipse(0, 0, _spawnRadiusXY.X, _spawnRadiusXY.Y, 20, Color.White);

            // Drawing the seperated rooms
            if (_separatedRooms != null)
            {
                foreach (var room in _separatedRooms)
                {
                    var newRect = new Rectangle(room.X, room.Y, room.Width, room.Height);
                    sb.DrawRectangle(newRect, Color.Red);
                }
            }
            sb.End();
        }
    }
}
