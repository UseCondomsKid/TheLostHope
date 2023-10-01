using LostHope.Engine.Animations;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHope.GameCode.Objects;

namespace TheLostHope.GameCode.ObjectStateMachine
{
    public class StatefullObject : GameObject
    {
        // State machine
        public ObjectStateMachine StateMachine { get; private set; }
        // Animator
        public Animator Animator { get; private set; }

        public StatefullObject(Game game, AsepriteFile asepriteFile) : base(game)
        {
            StateMachine = new ObjectStateMachine();
            Animator = new Animator(asepriteFile, game.GraphicsDevice);
        }
    }
}
