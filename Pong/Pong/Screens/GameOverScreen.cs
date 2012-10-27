using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerfectPong.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using PerfectPong.Extension;

namespace PerfectPong.Screens
{
    /// <summary>
    /// Displayed when the game is won by one of the players
    /// </summary>
    public class GameOverScreen : GameScreen
    {
        private const String TitleString = "Perfect Pong Over";
        private const String HelpString = "Player {0} won! Press enter to continue.";
        private Vector2 _positionTitle, _positionHelp;
        private Color _shadowColor;

        protected Int32 _winningPlayer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_winningPlayer">Index number of winning player</param>
        public GameOverScreen(Int32 winningPlayer)
        {
            _winningPlayer = winningPlayer;
        }

        /// <summary>
        /// Initializes the screen
        /// </summary>
        public override void Initialize()
        {
            this.TransitionOnTime = TimeSpan.FromSeconds(.5f);
            this.TransitionOffTime = TimeSpan.FromSeconds(.5f);
            this.IsPopup = false;
            base.Initialize();
        }

        /// <summary>
        /// Loads all content for this screen
        /// </summary>
        /// <param name="contentManager">ContentManager to load to</param>
        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);

            this.ScreenManager.SpriteFonts.LoadFont("Title", "Fonts/Title");
            this.ScreenManager.SpriteFonts.LoadFont("Help", "Fonts/Default");
            this.AudioManager.Load("blip", "confirm", 1f, .5f);
            this.AudioManager.Load("blip", "blip", 1f, .2f);

            var titleMeasurement = this.ScreenManager.SpriteFonts["Title"].MeasureString(TitleString);

            _positionTitle = Vector2.UnitX * (Int32)Math.Round((1280 - titleMeasurement.X) / 2) +
                Vector2.UnitY * 720 / 2;
            _positionHelp = Vector2.UnitX * (Int32)Math.Round((1280 - this.ScreenManager.SpriteFonts["Help"].MeasureString(String.Format(HelpString, _winningPlayer)).X) / 2) +
                Vector2.UnitY * (720 / 2 + (Int32)Math.Round(titleMeasurement.Y));
        }

        /// <summary>
        /// Update logic
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        /// <param name="otherScreenHasFocus">Game is blurred</param>
        /// <param name="coveredByOtherScreen">Other GameScreen is active</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            _shadowColor = ColorExtensions.Rainbow(gameTime);
        }

        /// <summary>
        /// Processes input
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void HandleInput(GameTime gameTime)
        {
            base.HandleInput(gameTime);

            if (this.InputManager.Keyboard.IsKeyReleased(Keys.Enter))
            {
                this.Next = new MenuScreen();
                this.ExitScreenAnd();
                this.AudioManager.Play("confirm");
            }
            else if (this.InputManager.Keyboard.IsKeyReleased(Keys.Escape))
            {
                this.ExitScreenAnd();
                this.AudioManager.Play("confirm");
            }

        }

        /// <summary>
        /// Draws frame
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void Draw(GameTime gameTime)
        {
            if (!this.IsTransitioning && this.ScreenState != Services.ScreenState.Active)
                return;

            base.Draw(gameTime);

            this.ScreenManager.SpriteBatch.Begin();
            this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Title"], TitleString, _positionTitle, Color.White, _shadowColor);
            this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Help"], String.Format(HelpString, _winningPlayer), _positionHelp, Color.White, _shadowColor);
            this.ScreenManager.SpriteBatch.End();

            this.ScreenManager.FadeBackBufferToBlack((Byte)(255 - this.TransitionAlpha));
        }
    }
}
