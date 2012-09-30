using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pong.Services;
using Pong.Level;
using Microsoft.Xna.Framework.Content;
using Pong.Screens;
using Microsoft.Xna.Framework.Audio;
using Pong.Extension;

namespace Pong.Actors
{
    /// <summary>
    /// Holds data about the ball
    /// </summary>
    public class Ball : DrawableGameComponent, ICollidable
    {
        /// <summary>
        /// The top-left position of the object
        /// </summary>
        public Vector2 Position { get; set;  }
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
        /// Level reference
        /// </summary>
        protected Level.Level Level { get; set; }

        /// <summary>
        /// Constituates the Phong Shading effect
        /// </summary>
        public Effect Phong { get; protected set; }
        
        protected ContentManager _contentManager;
        protected SpriteBatch _spriteBatch;
        protected Texture2D _texture;
        protected Vector2 _scaleFactor;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Game reference</param>
        /// <param name="level">Level reference</param>
        public Ball(Game game, Level.Level level)
            : base(game)
        {
            this.Level = level;
            this.Size = GameSettings.Instance.BallSize;

            ((CollisionManager)game.Services.GetService(typeof(CollisionManager))).Register(this);
        }

        /// <summary>
        /// Initializes the ball
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Handles the collision
        /// </summary>
        /// <param name="other"></param>
        public void HandleCollision(ICollidable other)
        {
            if (!other.IsSolid)
                return;
            
            Vector2 othercenter = other.Position + other.Size / 2;
            Vector2 mycenter = Position + Size / 2;
            Single perc = (mycenter.Y - othercenter.Y) / (other.Size.Y / 2);

            // The added speed is the spin speed
            Single addvel = perc * GameSettings.Instance.PaddleDirectionalInfluence + 
                other.Velocity.Y / GameSettings.Instance.PaddleMoveSpeed * GameSettings.Instance.PaddleSpinInfluence;

            // Updates the velocity (bounce angle) and moves the ball outside the paddle. Because we don't do bullet interpolation
            // with the ball and paddel collision, on each collision we have to move the ball outside the paddle, or collisions will
            // occur sequentially each frame.
            this.Velocity = new Vector2(-this.Velocity.X * (GameSettings.Instance.BallCollisionSpeed / 100f + 1), this.Velocity.Y + addvel);
            this.Position = new Vector2((other.Position.X + other.Size.X / 2) + ((other.Size.X / 2) + (Size.X)) * Math.Sign(Velocity.X), Position.Y);

            try
            {
                this.Level.Screen.AudioManager.Play("paddle");
            }
            catch (ObjectDisposedException)
            {
                // Sometimes wav files glitch and the soundeffect instance is
                // released too early before re-cached. Simply ignore the play
                // and continue. 
            }
        }

        /// <summary>
        /// Loads all resources and initialize phong shader
        /// </summary>
        /// <param name="contentManager"></param>
        public void LoadContent(ContentManager contentManager)
        {
            base.LoadContent();

            _contentManager = contentManager;
            _spriteBatch = new SpriteBatch(this.Game.GraphicsDevice);
            _texture = _contentManager.Load<Texture2D>("Graphics/Blank");

            this.Level.Screen.AudioManager.Load("blip", "paddle", 1f, 1f);
            this.Level.Screen.AudioManager.Load("blip", "wall", 1f, .7f);

            this.Phong = contentManager.Load<Effect>("Shaders\\PhongEffect");
            this.Phong.CurrentTechnique = Phong.Techniques["Draw"];

            //Default projection
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, this.Level.Screen.ScreenManager.ScreenWidth, this.Level.Screen.ScreenManager.ScreenHeight, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            this.Phong.Parameters["Projection"].SetValue(halfPixelOffset * projection);

            // This is used to determine the drawing size;
            _scaleFactor = Vector2.UnitX * (this.Size.X / _texture.Width) + Vector2.UnitY * (this.Size.Y / _texture.Height);
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
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing Values</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            Vector3 light = new Vector3(Level.Size.X / 2,Level.Size.Y / 2,100) - new Vector3(Position, 0);
            light.Normalize();

            var shadowColor = ColorExtensions.RainbowContinuous(gameTime);

            Phong.Parameters["LightDir"].SetValue(light);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, Phong);
            _spriteBatch.Draw(_texture, this.Position, _texture.Bounds, shadowColor, 0, Vector2.Zero, _scaleFactor, SpriteEffects.None, 0 ); 
            _spriteBatch.End();
        }

        /// <summary>
        /// Frame Update 
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing Values</param>
        public override void Update(GameTime gameTime)
        {
            // Bounce off walls
            if (this.Position.Y < 0)
            {
                this.Position = new Vector2(this.Position.X, 0);
                this.Velocity = new Vector2(this.Velocity.X, -this.Velocity.Y);
                this.Level.Screen.AudioManager.Play("wall");
            }
            else if (this.Position.Y > this.Level.Size.Y - this.Size.Y)
            {
                this.Position = new Vector2(this.Position.X, this.Level.Size.Y - this.Size.Y);
                this.Velocity = new Vector2(this.Velocity.X, -this.Velocity.Y);
                this.Level.Screen.AudioManager.Play("wall");
            }
            else
            {
                this.Position = this.Position + this.Velocity * (Single)(gameTime.ElapsedGameTime.TotalSeconds);
            }

            // Depress water
            ((PlayingScreen)Level.Screen).Water.Changes.Add(new Tuple<Vector2, float>(Position + Size / 2, -1f));
        }
    }
}
