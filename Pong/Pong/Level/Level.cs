using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pong.Services;
using Pong.Actors;
using Pong.Screens;

namespace Pong.Level
{
    /// <summary>
    /// Creates levels and stores information about them
    /// </summary>
    public class Level : GameComponent
    {
        public Vector2 Size { get; set; }
        protected Random Random { get; set; }
        public GameScreen Screen { get; private set; }

        public List<Player> Players { get; set; }
        public List<Ball> Balls { get; set; }
        public List<ICollidable> Actors { get; set; }

        /// <summary>
        /// Creat a new default level
        /// </summary>
        /// <param name="game"></param>
        /// <param name="screen"></param>
        public Level(Game game, GameScreen screen) : base(game)
        {
            this.Screen = screen;
            this.Random = new Random();
            this.Size = GameSettings.Instance.LevelSize;

            // Need 2 players
            if (GameSettings.Instance.Players.Count < 2)
            {
                Die();
                return;
            }

            this.Players = new List<Player>();
            this.Actors = new List<ICollidable>();
            this.Balls = new List<Ball>();

            // Add all the players from game settings
            for (Int32 i = 0; i < GameSettings.Instance.Players.Count; i++)
            {
                var player = GameSettings.Instance.Players[i];
                player.Level = this;
                this.Players.Add(player);

                var ai = player.Controller as IPaddleAIController;
                if (ai != null)
                    ai.Level = this;
            }
                
            // Player 1
            this.Actors.Add(new DeadZone(this.Game, this, this.Players[0])
            {
                Position = new Vector2(-1050, -20),
                Size = new Vector2(1000, this.Size.Y + 40),
            });

            this.Actors.Add(new Paddle(this.Game, this)
            {
                Controller = this.Players[0].Controller,
                Position = new Vector2(-90, this.Size.Y / 2 - GameSettings.Instance.PaddleSize.Y / 2),
            });
            this.Players[0].Paddle = this.Actors[this.Actors.Count - 1] as Paddle;

            // Player 2
            this.Actors.Add(new DeadZone(this.Game, this, this.Players[1])
            {
                Position = new Vector2(this.Size.X + 50, -20),
                Size = new Vector2(1000, this.Size.Y + 40),
            });

            this.Actors.Add(new Paddle(this.Game, this)
            {
                Controller = this.Players[1].Controller,
                Position = new Vector2(this.Size.X - GameSettings.Instance.PaddleSize.X + 90, this.Size.Y / 2 - GameSettings.Instance.PaddleSize.Y / 2),
            });
            this.Players[1].Paddle = this.Actors[this.Actors.Count - 1] as Paddle;

            /*
             * Room for adding more players. Just define them here. Make sure their controller and 
             * position are correctly set. Don't forget to override the ball class with a ball that
             * doesn't bounce on the top and bottom, and add deadzones in the appropiate places. 
             * The former isn't needed if you place the deadzone slightly inside the level.
             */

            // Single ball
            this.Balls.Add(new Ball(this.Game, this));

            /*
            * Room for adding more balls. This will JustWork. Just define them here.
            */

            // Reset the level
            Reset();
        }

        /// <summary>
        /// Initializes all components
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            foreach (var player in this.Players)
                player.Controller.Initialize();

            foreach (var actor in this.Actors)
                actor.Initialize();

            foreach (var ball in this.Balls)
                ball.Initialize();
        }

        /// <summary>
        /// Loads all content
        /// </summary>
        /// <param name="contentManager"></param>
        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            foreach (var ball in this.Balls)
                ball.LoadContent(contentManager);

            foreach (var actor in this.Actors)
                actor.LoadContent(contentManager);
        }

        /// <summary>
        /// Resets the level
        /// </summary>
        public void Reset()
        {
            // Check if a player is permantly dead
            foreach (Player p in this.Players)
            {
                if (p.Lives <= 0)
                {
                    Die();
                    ((CollisionManager)this.Game.Services.GetService(typeof(CollisionManager))).Reset();
                    return;
                }
            }

            // Re place the balls
            foreach (var ball in this.Balls)
            {
                ball.Position = this.Size / 2;
                ball.Velocity = new Vector2((Single)(this.Random.Next(0, 2) * 2 - 1) * GameSettings.Instance.BallStartSpeed,
                    ((Single)(this.Random.Next(0, 2) * 2 - 1) * GameSettings.Instance.BallStartSpeed / 2));
            }
        }

        /// <summary>
        /// Frame Update
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing Values</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            foreach (var actor in this.Actors)
                actor.Update(gameTime);

            foreach (var ball in this.Balls)
                ball.Update(gameTime);
        }

        /// <summary>
        /// Die the level
        /// </summary>
        public void Die()
        {
            this.Screen.Next = new GameOverScreen(this.Players.Where((p) => p.Lives != 0).FirstOrDefault().Id);
            this.Screen.ExitScreenAnd();
        }

    }
}
