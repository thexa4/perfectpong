﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerfectPong.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using PerfectPong.Extension;

namespace PerfectPong.Screens
{
    /// <summary>
    /// This screen displays the main menu
    /// </summary>
    public class MenuScreen : GameScreen
    {
        private const String TitleString = "Perfect Pong Menu";
        private readonly String[] Options = new String[] { "Start Game", "Options", "Exit to Title" };
        protected Vector2 _positionTitle, _positionMenu;
        protected Int32 _menuIndex;
        protected Color _shadowColor;

        /// <summary>
        /// Initializes the screen
        /// </summary>
        public override void Initialize()
        {
            this.TransitionOnTime = TimeSpan.FromSeconds(.5f);
            this.TransitionOffTime = TimeSpan.FromSeconds(.5f);

            _menuIndex = 0;

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
            this.ScreenManager.SpriteFonts.LoadFont("Menu", "Fonts/Default");
            this.AudioManager.Load("blip", "confirm", 1f, .5f);
            this.AudioManager.Load("blip", "blip", 1f, .2f);

            // Read out the total height
            var titleMeasurement = this.ScreenManager.SpriteFonts["Title"].MeasureString(TitleString);
            var menuMeasurement = Options.Sum(a => this.ScreenManager.SpriteFonts["Menu"].MeasureString(a).Y + 15) - 15;
            var height = titleMeasurement.Y + 10 + menuMeasurement;

            // Set the positions accordingly
            _positionTitle = Vector2.UnitX * (Int32)Math.Round((1280 - titleMeasurement.X) / 2) +
                Vector2.UnitY * (Single)Math.Round((720f - height) / 2);
            _positionMenu = Vector2.UnitX * (Int32)Math.Round(1280f / 2) +
                Vector2.UnitY * (Single)(Math.Round((720f - height) / 2) + 10 + Math.Round(titleMeasurement.Y));   
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
                //
                switch (_menuIndex)
                {
                    case 0:
                        this.Next = new SelectInputScreen();
                        break;

                    case 1:
                        this.Next = new OptionsScreen();
                        break;

                    case 2:
                        this.Next = new TitleScreen();
                        break;
                }
                this.AudioManager.Play("confirm");
            }
            else if (this.InputManager.Keyboard.IsKeyReleased(Keys.Escape))
            {
                this.Next = new TitleScreen();
                this.AudioManager.Play("confirm");
            }

            if (this.InputManager.Keyboard.IsKeyTriggerd(Keys.Down))
            {
                _menuIndex = (_menuIndex + 1) % Options.Length;
                this.AudioManager.Play("blip");
            }
            else if (this.InputManager.Keyboard.IsKeyTriggerd(Keys.Up))
            {
                _menuIndex = (_menuIndex == 0 ? Options.Length - 1 : _menuIndex - 1);
                this.AudioManager.Play("blip");
            }

            if (this.Next != null)
                this.ExitScreenAnd();

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

            // Draw all options
            var position = _positionMenu;
            for (Int32 i = 0; i < Options.Length; i++)
            {
                var measurement = this.ScreenManager.SpriteFonts["Menu"].MeasureString(Options[i]);
                this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Menu"], Options[i], position, 
                    Color.White, _menuIndex == i ? _shadowColor : Color.Black, 0,
                    (Single)Math.Round(measurement.X / 2) * Vector2.UnitX + (Single)Math.Round(measurement.Y / 2) * Vector2.UnitY, 
                    1, SpriteEffects.None, 0);
                position = position + Vector2.UnitY * 15;
            }
            this.ScreenManager.SpriteBatch.End();

            if (this.IsTransitioning)
                this.ScreenManager.FadeBackBufferToBlack((Byte)(255 - this.TransitionAlpha));
        }
    }
}
