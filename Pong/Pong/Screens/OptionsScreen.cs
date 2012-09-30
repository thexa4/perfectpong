using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pong.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Pong.Extension;

namespace Pong.Screens
{
    /// <summary>
    /// This screens allows setting the game options
    /// </summary>
    public class OptionsScreen : GameScreen
    {
        private const String TitleString = "Perfect Pong Options";

        private readonly String[] Options = new String[] { 
            "Paddle speed", 
            "Paddle width", 
            "Paddle height", 
            "Paddle spin influence %", 
            "Paddle directional influence %", 
            "Ball start speed", 
            "Ball size", 
            "Ball acceleration % on bounce", 
            "Water displacement size",
            "Player lives",
            "Return" ,
        };

        private readonly String[] OptionNames = new String[] {
            "PaddleMoveSpeed",
            "PaddleWidth",
            "PaddleHeight",
            "PaddleSpinInfluence",
            "PaddleDirectionalInfluence",
            "BallStartSpeed",
            "BallWidth",
            "BallCollisionSpeed",
            "WaterDisplacementSize",
            "PlayerLives",
        };

        private readonly Single[][] Presets = new Single[][] { 
            new Single[] { 50, 75, 100, 125, 150, 175, 200, 225, 250, 275, 300 },
            new Single[] { 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110 },
            new Single[] { 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150 },
            new Single[] { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75 },
            new Single[] { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75 },
            new Single[] { 50, 75, 100, 125, 150, 175, 200, 225, 250, 275, 300 },
            new Single[] { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 },
            new Single[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
            new Single[] { 0, 1, 2, 3 },
            new Single[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 20, 25, 30, 35, 40, 45, 50, 75, 100 },
        };

        private Int32[] _presetIndex;

        //private readonly String[] Help

        protected Int32 _menuIndex;
        protected Color _shadowColor;
        protected Vector2 _positionTitle, _positionMenu;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isPopup"></param>
        public OptionsScreen(Boolean isPopup = false)
        {
            this.IsPopup = isPopup;
        }

        /// <summary>
        /// Initializes the screen
        /// </summary>
        public override void Initialize()
        {
            this.TransitionOnTime = TimeSpan.FromSeconds(.5f);
            this.TransitionOffTime = TimeSpan.FromSeconds(.5f);

            _menuIndex = 0;
            _presetIndex = new Int32[this.Presets.Length];
            for (Int32 i = 0; i < _presetIndex.Length; i++) {
                var currentValue = GetCurrentOptionValue(i);
                _presetIndex[i] = this.Presets[i].TakeWhile(a => a < currentValue).Count();
            }

            base.Initialize();
        }

        /// <summary>
        /// Gets an option value from the game settings instance
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected Single GetCurrentOptionValue(Int32 index)
        {
            return (Single)(typeof(GameSettings).GetProperty(OptionNames[index]).GetValue(GameSettings.Instance, null));
        }

        /// <summary>
        /// Gets the new option value
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected Single GetNewOptionValue(Int32 index)
        {
            return this.Presets[index][_presetIndex[index]];
        }

        /// <summary>
        /// Saves all the options
        /// </summary>
        protected void SaveOptions()
        {
            var settingsType = typeof(GameSettings);
            for (Int32 i = 0; i < _presetIndex.Length; i++)
                settingsType.GetProperty(OptionNames[i]).SetValue(GameSettings.Instance, this.Presets[i][_presetIndex[i]], null);
        }

        /// <summary>
        /// Loads all content for this screen
        /// </summary>
        /// <param name="contentManager">ContentManager to load to</param>
        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);

            var titleMeasurement = this.ScreenManager.SpriteFonts["Title"].MeasureString(TitleString);
            var menuMeasurement = Options.Sum(a => this.ScreenManager.SpriteFonts["Menu"].MeasureString(a).Y + 15) - 15;
            var height = titleMeasurement.Y + 10 + menuMeasurement;
            this.AudioManager.Load("blip", "confirm", 1f, .5f);
            this.AudioManager.Load("blip", "blip", 1f, .2f);

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

            // Change
            if (this.InputManager.Keyboard.IsKeyTriggerd(Keys.Right))
            {
                if (_menuIndex < this.Presets.Length)
                    _presetIndex[_menuIndex] = (++_presetIndex[_menuIndex]) % this.Presets[_menuIndex].Length;
                this.AudioManager.Play("blip");
            }
            else if (this.InputManager.Keyboard.IsKeyTriggerd(Keys.Left))
            {
                if (_menuIndex < this.Presets.Length)
                    _presetIndex[_menuIndex] = _presetIndex[_menuIndex] > 0 ? (--_presetIndex[_menuIndex]) : this.Presets[_menuIndex].Length - 1;
                this.AudioManager.Play("blip"); 
            }
            

            // Advance
            if (this.InputManager.Keyboard.IsKeyTriggerd(Keys.Down) || this.InputManager.Keyboard.IsKeyTriggerd(Keys.Enter))
            {
                if (_menuIndex < this.Presets.Length)
                {
                    _menuIndex = (_menuIndex + 1) % Options.Length;
                    this.AudioManager.Play("blip");
                }
                else
                {
                    if (this.IsPopup)
                        this.ExitScreen();
                    else
                        this.Next = new MenuScreen();
                    SaveOptions();
                    this.AudioManager.Play("confirm");
                }
            }
            else if (this.InputManager.Keyboard.IsKeyTriggerd(Keys.Up))
            {
                _menuIndex = (_menuIndex == 0 ? Options.Length - 1 : _menuIndex - 1);
                this.AudioManager.Play("blip");
            }
            else if (this.InputManager.Keyboard.IsKeyReleased(Keys.Escape))
            {
                if (this.IsPopup)
                {
                    this.ExitScreen();
                    SaveOptions();
                }
                else
                    this.Next = new MenuScreen();
                this.AudioManager.Play("confirm");

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

            var transitalpha = this.IsPopup ? (1 - this.TransitionPosition) : 1f;

            this.ScreenManager.SpriteBatch.Begin();
            this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Title"], TitleString, _positionTitle, Color.White * transitalpha, _shadowColor * transitalpha);
            var position = _positionMenu;
            for (Int32 i = 0; i < Options.Length; i++)
            {
                var formated = Options[i];
                if (i < this.Presets.Length)
                {
                    var next = GetNewOptionValue(i);
                    formated =  String.Format("{0}: {1}", Options[i], _menuIndex == i ? String.Join(" ", this.Presets[i]) : next.ToString());
                }
                
                var measurement = this.ScreenManager.SpriteFonts["Menu"].MeasureString(formated);

                // Draw the option
                this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Menu"], formated, position,
                    (_menuIndex == i ? Color.White : Color.Gray) * transitalpha, (_menuIndex == i ? _shadowColor : Color.Black) * transitalpha, 0,
                    (Single)Math.Round(measurement.X / 2) * Vector2.UnitX + (Single)Math.Round(measurement.Y / 2) * Vector2.UnitY,
                    1, SpriteEffects.None, 0);

                // Draw all values when option is selected
                if (i < this.Presets.Length && i == _menuIndex)
                {
                    var offsetMeasurement = this.ScreenManager.SpriteFonts["Menu"].MeasureString(Options[i] + ": ");
                    var totalMeasurement = Vector2.Zero;
                    for (Int32 j = 0; j < this.Presets[i].Length; j++)
                    {
                        var subMeasurement = this.ScreenManager.SpriteFonts["Menu"].MeasureString(" " + this.Presets[i][j]);
                        this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Menu"], this.Presets[i][j].ToString(),
                            position + offsetMeasurement * Vector2.UnitX + totalMeasurement,
                            (j == _presetIndex[i] ? Color.White : Color.Gray) * transitalpha, (j == _presetIndex[i] ? _shadowColor : Color.Black) * transitalpha, 0,
                            (Single)Math.Round(measurement.X / 2) * Vector2.UnitX + (Single)Math.Round(measurement.Y / 2) * Vector2.UnitY,
                        1, SpriteEffects.None, 0);
                        totalMeasurement += Vector2.UnitX * subMeasurement;
                    }
                }
                 

                position = position + Vector2.UnitY * 15;
            }
            this.ScreenManager.SpriteBatch.End();

            if (this.IsTransitioning && !this.IsPopup)
                this.ScreenManager.FadeBackBufferToBlack((Byte)(255 - this.TransitionAlpha));
        }
    }
}
