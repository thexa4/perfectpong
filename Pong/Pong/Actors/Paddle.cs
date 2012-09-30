using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pong.Services;
using Microsoft.Xna.Framework.Content;
using Pong.Screens;
using Pong.Extension;

namespace Pong.Actors
{
    /// <summary>
    /// Stores data about paddles and updates it's position based on controllers
    /// </summary>
    public class Paddle : DrawableGameComponent, ICollidable
    {
        /// <summary>
        /// The top-left position of the object
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// The velocity of the object
        /// </summary>
        public Vector2 Velocity { get; set; }
        /// <summary>
        /// The size of the object
        /// </summary>
        public Vector2 Size { get; set; }
        /// <summary>
        /// Indicates wether the object is a solid
        /// </summary>
        public Boolean IsSolid { get { return true; } }

        /// <summary>
        /// Controller
        /// </summary>
        public IPaddleController Controller { get; set; }

        /// <summary>
        /// Reference level
        /// </summary>
        public Level.Level Level { get; set; }

        protected ContentManager _contentManager;
        protected SpriteBatch _spriteBatch;
        protected Texture2D _texture;
        protected Rectangle _destinationRectangle;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Game reference</param>
        /// <param name="level">Level reference</param>
        public Paddle(Game game, Level.Level level)
            : base(game)
        {
            ((CollisionManager)game.Services.GetService(typeof(CollisionManager))).Register(this);

            this.Level = level;
            this.Size = GameSettings.Instance.PaddleSize;
        }

        /// <summary>
        /// Intializes the paddle
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            _destinationRectangle = new Rectangle((Int32)Math.Round(this.Position.X), (Int32)Math.Round(this.Position.Y),
                (Int32)Math.Round(this.Size.X), (Int32)Math.Round(this.Size.Y));
        }

        /// <summary>
        /// Handles collision
        /// </summary>
        /// <param name="other"></param>
        public void HandleCollision(ICollidable other)
        {
            // Paddle shouldn't react to any collision
            return;
        }

        /// <summary>
        /// Loads all resources
        /// </summary>
        /// <param name="contentManager"></param>
        public void LoadContent(ContentManager contentManager)
        {
            base.LoadContent();

            _contentManager = contentManager;
            _spriteBatch = new SpriteBatch(this.Game.GraphicsDevice);
            _texture = _contentManager.Load<Texture2D>("Graphics/Blank");
        }

        /// <summary>
        /// Unloads all resources
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();

            _spriteBatch.Dispose();
        }

        /// <summary>
        /// Frame update
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing Value</param>
        public override void Update(GameTime gameTime)
        {
            Vector2 newvel = Vector2.Zero;

            switch(Controller.Direction)
            {
                case PaddleDirection.Up:
                    newvel = new Vector2(0,-GameSettings.Instance.PaddleMoveSpeed);
                    break;
                case PaddleDirection.Down:
                    newvel = new Vector2(0,GameSettings.Instance.PaddleMoveSpeed);
                    break;
            }

            this.Velocity = newvel;

            // Updates the position
            var oldPosition = this.Position;
            this.Position = new Vector2(
                MathHelper.Clamp(this.Position.X + this.Velocity.X * (Single)(gameTime.ElapsedGameTime.TotalSeconds), -90, this.Level.Size.X - this.Size.X + 90),
                MathHelper.Clamp(this.Position.Y + this.Velocity.Y * (Single)(gameTime.ElapsedGameTime.TotalSeconds), 0, this.Level.Size.Y - this.Size.Y));

            // If moved, update the water
            newvel = (this.Position - oldPosition) / (Single)(gameTime.ElapsedGameTime.TotalSeconds);
            if (newvel.LengthSquared() > 0)
                ((PlayingScreen)Level.Screen).Water.Changes.Add(new Tuple<Vector2, float>(
                    new Vector2(
                        (Single)MathHelper.Clamp(Position.X + (Size.X) / 2, 
                        0, this.Level.Size.X), 
                        Position.Y + (Size.Y / 2)), 0.5f));

            // Drawing rectangle
            _destinationRectangle.X = (Int32)Math.Round(this.Position.X);
            _destinationRectangle.Y = (Int32)Math.Round(this.Position.Y);
        }

        /// <summary>
        /// Frame Draw
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing Values</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_texture, _destinationRectangle, Color.White);
            _destinationRectangle.Inflate(-2, 0);
            _spriteBatch.Draw(_texture, _destinationRectangle, ColorExtensions.RainbowContinuous(gameTime));
            _destinationRectangle.Inflate(-2, 0);
            _spriteBatch.Draw(_texture, _destinationRectangle, Color.LightSlateGray);
            _destinationRectangle.Inflate(4, 0);
            _spriteBatch.End();
        }
    }
}
