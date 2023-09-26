using LostHope.Engine.Animations;
using LostHope.GameCode.Characters.FSM;
using Microsoft.Xna.Framework;

namespace LostHope.GameCode.Characters.Enemies
{
    public abstract class Enemy : Character
    {
        protected Enemy(Game game, AsepriteExportData asepriteExportData) : base(game, asepriteExportData)
        {
        }
    }
}
