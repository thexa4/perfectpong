using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using PerfectPong.Services;

namespace PerfectPong.Level
{
    /// <summary>
    /// Draws the water effect and updates it
    /// </summary>
    public class WaterEffect : DrawableGameComponent
    {
        public RenderTarget2D HeightData { get; protected set; }
        public RenderTarget2D HeightData2 { get; protected set; }
        public RenderTarget2D VelocityData { get; protected set; }
        public Int32 Width { get; protected set; }
        public Int32 Height { get; protected set; }
        public SpriteBatch SpriteBatch { get; protected set; }
        public Effect Effect { get; protected set; }
        public Texture2D Empty { get; set; }
        public Texture2D Background { get; set; }

        /// <summary>
        /// The queued points that have to be changed to a certain velocity
        /// </summary>
        public List<Tuple<Vector2, float>> Changes { get; protected set; }

        /// <summary>
        /// Creates a new watereffect
        /// </summary>
        /// <param name="game"></param>
        /// <param name="camera"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public WaterEffect(Game game, int width, int height)
            : base(game)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Loads the background, creates the rendertargets to draw in and sets up the shader
        /// </summary>
        /// <param name="contentManager"></param>
        public void LoadContent(ContentManager contentManager)
        {
            this.HeightData = new RenderTarget2D(this.Game.GraphicsDevice, this.Width, this.Height, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            this.HeightData2 = new RenderTarget2D(this.Game.GraphicsDevice, this.Width, this.Height, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            this.VelocityData = new RenderTarget2D(this.Game.GraphicsDevice, this.Width, this.Height, false, SurfaceFormat.Single, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            this.Empty = new Texture2D(this.Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Single);
            this.Changes = new List<Tuple<Vector2, float>>();

            this.Background = contentManager.Load<Texture2D>("Graphics\\background");

            this.SpriteBatch = new SpriteBatch(Game.GraphicsDevice);
            this.Effect = contentManager.Load<Effect>("Shaders\\WaterEffect");

            //Default projection
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, this.Width, this.Height, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            this.Effect.Parameters["Projection"].SetValue(halfPixelOffset * projection);
            this.Effect.Parameters["Resolution"].SetValue(new Vector2(1.0f/this.Width, 1.0f/this.Height));
        }

        /// <summary>
        /// Updates the water heights, changes the velocities as requested and finally draws the effect
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // Previous heightdata
            this.Effect.Parameters["HeightTexture"].SetValue(this.HeightData);

            // Update heightdata with previous (changed) velocity data
            this.Game.GraphicsDevice.SetRenderTarget(this.HeightData2);
            this.Effect.CurrentTechnique = this.Effect.Techniques["UpdateHeight"];
            this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, null, null, this.Effect);
            this.SpriteBatch.Draw(this.VelocityData, Vector2.Zero, Color.White);
            this.SpriteBatch.End();

            // Recalculate the needed velocity
            this.Game.GraphicsDevice.SetRenderTarget(this.VelocityData);
            this.Effect.CurrentTechnique = this.Effect.Techniques["UpdateVelocity"];
            this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, null, null, this.Effect);
            this.SpriteBatch.Draw(this.HeightData2, Vector2.Zero, Color.White);
            this.SpriteBatch.End();

            // Apply all effects from actors
            this.Effect.CurrentTechnique = this.Effect.Techniques["Change"];
            this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, null, null, this.Effect);
            foreach (var t in this.Changes)
            {
                this.SpriteBatch.Draw(Empty, new Rectangle((int)(t.Item1.X - GameSettings.Instance.WaterDisplacementSize / 2), (int)(t.Item1.Y - GameSettings.Instance.WaterDisplacementSize / 2), (int)GameSettings.Instance.WaterDisplacementSize, (int)GameSettings.Instance.WaterDisplacementSize),
                    new Color(t.Item2 > 0 ? t.Item2 : 0, t.Item2 < 0 ? -t.Item2 : 0, 0, 1));
            }
            this.SpriteBatch.End();
            this.Changes.Clear();

            this.Game.GraphicsDevice.SetRenderTarget(null);

            // Draw result
            this.Effect.CurrentTechnique = this.Effect.Techniques["Draw"];
            this.Effect.Parameters["HeightTexture"].SetValue(this.HeightData2);
            this.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, null, null, this.Effect);
            this.SpriteBatch.Draw(Background, new Rectangle(0, 0, Width, Height), Color.White);
            this.SpriteBatch.End();

            //Switch height textures
            var tmp = this.HeightData;
            this.HeightData = this.HeightData2;
            this.HeightData2 = tmp;
        }
    }
}
