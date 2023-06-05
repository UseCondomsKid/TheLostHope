using LostHope.Engine.DungeonGeneration;
using LostHope.Engine.StateManagement;
using Microsoft.Xna.Framework;

namespace LostHope.GameCode.GameStates
{
    public class DungeonTesting : GameState
    {
        DungeonGenerator generator;
        public DungeonTesting(Game1 gameRef, GameStateManager stateManager) : base(gameRef, stateManager)
        {
        }

        public override void Enter()
        {
            base.Enter();
            generator = new DungeonGenerator();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            generator.DebugUpdate(delta);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            generator.DebugDraw();
        }
    }
}
