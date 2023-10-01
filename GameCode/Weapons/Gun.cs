using Humper;
using LDtkTypes;
using LostHope.Engine.Animations;
using LostHope.GameCode.GameStates;
using LostHope.GameCode.Weapons.Bullets;
using Microsoft.Xna.Framework;
using MonoGame.Aseprite;
using System;
using System.Collections.Generic;
using TheLostHope.GameCode.ObjectStateMachine;

namespace LostHope.GameCode.Weapons
{
    public class Gun : StatefullObject
    {
        private LDtkGun _gunData;
        private GameplayManager _gameplayManager;

        public Gun(Game game, AsepriteFile asepriteFile, LDtkGun gunData, GameplayManager gameplayManager) : base(game, asepriteFile)
        {
            _gunData = gunData;
            _gameplayManager = gameplayManager;
        }

        public void SpawnGun()
        {
        }
        public override void Update(GameTime gameTime)
        {
        }
        public override void Draw(GameTime gameTime)
        {
        }
    }
}
