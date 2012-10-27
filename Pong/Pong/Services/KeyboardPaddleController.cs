using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PerfectPong.Services
{
    public class KeyboardPaddleController : GameComponent, IPaddleController
    {
        /// <summary>
        /// Direction the controller wants the paddle to go
        /// </summary>
        public PaddleDirection Direction
        {
            get;
            protected set;
        }

        private Keys _up, _down;
        private InputManager _inputManager;

        /// <summary>
        /// Creates a new Paddle Controller
        /// </summary>
        /// <param name="game"></param>
        /// <param name="up"></param>
        /// <param name="down"></param>
        public KeyboardPaddleController(Game game, Keys up, Keys down) : base(game)
        {
            _up = up;
            _down = down;

            /*
             * For four player support, extend this class to include left and right. Enable
             * the keys on creation and have the update function also set the left and right
             * paddle direction.
             */
        }

        /// <summary>
        /// Initializes controller
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            _inputManager = (InputManager)this.Game.Services.GetService(typeof(InputManager));
        }

        /// <summary>
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Direction = PaddleDirection.None;

            //
            if (_inputManager.Keyboard.IsKeyDown(_up))
                this.Direction = PaddleDirection.Up;
            //
            if (_inputManager.Keyboard.IsKeyDown(_down))
                this.Direction = this.Direction == PaddleDirection.Up ? PaddleDirection.None : PaddleDirection.Down;
        }
    }
}
