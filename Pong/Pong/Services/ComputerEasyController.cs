using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PerfectPong.Actors;

namespace PerfectPong.Services
{
    public class ComputerEasyController : GameComponent, IPaddleAIController
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
        /// Creates a new Easy Computer, moves the paddle to the spot the ball is right now.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="up"></param>
        /// <param name="down"></param>
        public ComputerEasyController(Game game)
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

            // And move to correct position
            if (ballCenter.Y - paddleCenter.Y > ball.Size.Y)
                this.Direction = PaddleDirection.Down;
            if (ballCenter.Y - paddleCenter.Y < -ball.Size.Y)
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
