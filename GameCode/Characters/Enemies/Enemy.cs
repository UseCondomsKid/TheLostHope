using Humper;
using LDtk;
using LostHope.GameCode.Characters.FSM;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.GameCode.Characters.Enemies
{
    public abstract class Enemy : Character
    {
        protected Enemy(Game game, AsepriteFile asepriteFile) : base(game, asepriteFile)
        {
        }
    }
}
