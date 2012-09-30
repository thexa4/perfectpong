using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Pong.Services
{
    public class GamepadPaddleController : GameComponent, IPaddleController
    {
        /// <summary>
        /// Direction the controller wants the paddle to go
        /// </summary>
        public PaddleDirection Direction
        {
            get;
            protected set;
        }

        private InputManager _inputManager;
        private PlayerIndex _gamePadIndex;

        /// <summary>
        /// Creates a new GamepadPaddleController
        /// </summary>
        /// <param name="game"></param>
        /// <param name="gamePadIndex"></param>
        public GamepadPaddleController(Game game, PlayerIndex gamePadIndex)
            : base(game)
        {
            _gamePadIndex = gamePadIndex;
        }

        /// <summary>
        /// Initializes the controller
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            _inputManager = (InputManager)this.Game.Services.GetService(typeof(InputManager));
        }

        /// <summary>
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing Values</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var stickDirection = GetThumbY();
            this.Direction = stickDirection < 0 ? PaddleDirection.Up : (stickDirection > 0 ? PaddleDirection.Down : PaddleDirection.None);
        }

        /// <summary>
        /// Get Thumb Y value
        /// </summary>
        /// <returns></returns>
        public Single GetThumbY()
        {
            return _inputManager.GamePad.GamePadPlayerCurrentState(_gamePadIndex).ThumbSticks.Right.Y;
        }
    }
}
