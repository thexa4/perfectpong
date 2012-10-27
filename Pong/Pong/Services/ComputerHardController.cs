using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PerfectPong.Actors;

namespace PerfectPong.Services
{
    public class ComputerHardController : GameComponent, IPaddleAIController
    {
        /// <summary>
        /// Direction the controller wants the paddle to go
        /// </summary>
        public PaddleDirection Direction
        {
            get;
            protected set;
        }

        protected Level.Level _level;
        protected Paddle _paddle;

        /// <summary>
        /// Creates a new Hard Computer, moves the paddle to the spot the ball is going to be.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="up"></param>
        /// <param name="down"></param>
        public ComputerHardController(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Initializes controller
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            _paddle = _level.Players.Find(a => a.Controller == this).Paddle;
        }

        /// <summary>
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Direction = PaddleDirection.None;

            // TODO closest ball
            var ball = _level.Balls[0];

            var paddleCenter = _paddle.Position + _paddle.Size / 2;
            var ballCenter = ball.Position + ball.Size / 2;
            
            // If ball is moving away, do nothing
            if (Math.Abs(paddleCenter.X - ballCenter.X) < Math.Abs(paddleCenter.X - ballCenter.X - ball.Velocity.X))
                return;

            // Find linear endpoint
            var xdist = Math.Abs(paddleCenter.X - ballCenter.X) - GameSettings.Instance.PaddleWidth / 2;
            var endpoint = ballCenter + ball.Velocity * (xdist / Math.Abs(ball.Velocity.X));

            // Correct for bounces
            if (endpoint.Y < 0)
            {
                endpoint = new Vector2(endpoint.X, Math.Abs(endpoint.Y % (2 * _level.Size.Y)));
            }
            else if (endpoint.Y > _level.Size.Y)
            {
                endpoint = new Vector2(endpoint.X, _level.Size.Y - Math.Abs((endpoint.Y % (2 * _level.Size.Y)) - _level.Size.Y));
            }

            // And move to correct position
            if (endpoint.Y - paddleCenter.Y > ball.Size.Y)
                this.Direction = PaddleDirection.Down;
            if (endpoint.Y - paddleCenter.Y < -ball.Size.Y)
                this.Direction = PaddleDirection.Up;
        }

        /// <summary>
        /// 
        /// </summary>
        public Level.Level Level
        {
            set { _level = value; }
        }
    }
}
