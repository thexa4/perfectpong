using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerfectPong.Services;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using PerfectPong.Extension;
using Microsoft.Xna.Framework.Graphics;

namespace PerfectPong.Screens
{
    /// <summary>
    /// Displays the current score
    /// </summary>
    public class HeadsUpDisplayPopup : GameScreen
    {
        private const String TitleString = "Perfect Pong";
        private const String PlayerString = "Player {0}";

        protected Vector2 _positionTitle;
        protected Level.Level _level;
        protected Color _shadowColor;
        protected Texture2D _texture;

        private Int32 _startingLives;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="level">Level reference</param>
        public HeadsUpDisplayPopup(Level.Level level)
        {
            _level = level;
        }

        /// <summary>
        /// Initializes the screen
        /// </summary>
        public override void Initialize()
        {
            this.IsPopup = true;
            this.IsCapturingInput = false;
            this.TransitionOnTime = TimeSpan.FromSeconds(0.5f);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.5f);

            base.Initialize();

            _startingLives = _level.Players[0].Lives;
        }

        /// <summary>
        /// Load content
        /// </summary>
        /// <param name="contentManager"></param>
        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);

            this.ScreenManager.SpriteFonts.LoadFont("Title", "Fonts/Title");
            this.ScreenManager.SpriteFonts.LoadFont("Help", "Fonts/Default");

            _texture = this.ContentManager.Load<Texture2D>("Graphics/blank");
        }

        /// <summary>
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            _shadowColor = ColorExtensions.Rainbow(gameTime);
        }

        /// <summary>
        /// Frame Draw
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing Values</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var transitAlpha = (1.0f - this.TransitionPosition);

            this.ScreenManager.SpriteBatch.Begin();
            this.ScreenManager.SpriteBatch.Draw(_texture, new Rectangle(0, 0, 1280, 45), Color.Black * 0.5f * transitAlpha);

            this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Title"], TitleString, Vector2.One * 5,
                Color.White * transitAlpha, _shadowColor * transitAlpha);
            
            // Player 1
            this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Help"], String.Format(PlayerString, 1),
                Vector2.One * 5 + Vector2.UnitY * 15, Color.White * transitAlpha, _shadowColor * transitAlpha);
            
            for (Int32 i = 0; i < _level.Players[0].Lives; i++)
            {
                this.ScreenManager.SpriteBatch.Draw(_texture, Vector2.One * 6 + Vector2.UnitX * 5 * i + Vector2.UnitY * 28, _shadowColor * transitAlpha);
                this.ScreenManager.SpriteBatch.Draw(_texture, Vector2.One * 5 + Vector2.UnitX * 5 * i + Vector2.UnitY * 28, Color.White * transitAlpha);
            }

            for (Int32 i = _level.Players[0].Lives; i < _startingLives; i++)
            {
                this.ScreenManager.SpriteBatch.Draw(_texture, Vector2.One * 6 + Vector2.UnitX * 5 * i + Vector2.UnitY * 28, Color.Black * transitAlpha);
                this.ScreenManager.SpriteBatch.Draw(_texture, Vector2.One * 5 + Vector2.UnitX * 5 * i + Vector2.UnitY * 28, Color.Gray * transitAlpha);
            }

            // Player 2
            this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Help"], String.Format(PlayerString, 2),
                Vector2.One * 5 + Vector2.UnitY * 15 + Vector2.UnitX * (1270 - this.ScreenManager.SpriteFonts["Help"].MeasureString(String.Format(PlayerString, 2)).X),
                Color.White * transitAlpha, _shadowColor * transitAlpha);

            for (Int32 i = 0; i < _level.Players[1].Lives; i++)
            {
                this.ScreenManager.SpriteBatch.Draw(_texture, Vector2.One * 6 + Vector2.UnitX * (1265 - 5 * i) + Vector2.UnitY * 28, _shadowColor * transitAlpha);
                this.ScreenManager.SpriteBatch.Draw(_texture, Vector2.One * 5 + Vector2.UnitX * (1265 - 5 * i) + Vector2.UnitY * 28, Color.White * transitAlpha);
            }

            for (Int32 i = _level.Players[1].Lives; i < _startingLives; i++) 
            {
                this.ScreenManager.SpriteBatch.Draw(_texture, Vector2.One * 6 + Vector2.UnitX * (1265 - 5 * i) + Vector2.UnitY * 28, Color.Black * transitAlpha);
                this.ScreenManager.SpriteBatch.Draw(_texture, Vector2.One * 5 + Vector2.UnitX * (1265 - 5 * i) + Vector2.UnitY * 28, Color.Gray * transitAlpha);
            }



            this.ScreenManager.SpriteBatch.End();
        }

    }
}
