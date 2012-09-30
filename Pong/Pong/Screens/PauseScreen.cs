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
    /// 
    /// </summary>
    public class PauseScreen : GameScreen
    {
        private const String TitleString = "Perfect Pong Pause";
        private readonly String[] Options = new String[] { "Resume Game", "Options", "End Match" };
        protected Vector2 _positionTitle, _positionMenu;
        protected Int32 _menuIndex;
        protected Color _shadowColor;
        protected Level.Level _level;
        protected Texture2D _texture;

        protected GameScreen _popup;
        protected Boolean _popupEnabled;

        public PauseScreen(Level.Level level)
        {
            _level = level;
            _level.Screen.Exiting += new EventHandler(Screen_Exiting);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Screen_Exiting(object sender, EventArgs e)
        {
            this.ExitScreen();
        }


        /// <summary>
        /// Initializes the screen
        /// </summary>
        public override void Initialize()
        {
            this.TransitionOnTime = TimeSpan.FromSeconds(.5f);
            this.TransitionOffTime = TimeSpan.FromSeconds(.5f);

            _menuIndex = 0;

            this.IsPopup = true;
            this.IsCapturingInput = true;
            base.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void PostProcessing()
        {
            base.PostProcessing();

            _popup = new OptionsScreen(true);
            _popup.Exited += new EventHandler(_popup_Exited);
            _popupEnabled = true;

            this.Exiting += new EventHandler(PauseScreen_Exiting);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PauseScreen_Exiting(object sender, EventArgs e)
        {
            _popup.ExitScreen();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _popup_Exited(object sender, EventArgs e)
        {
            _popupEnabled = true;
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
            _texture = this.ContentManager.Load<Texture2D>("Graphics/Blank");
            this.AudioManager.Load("blip", "confirm", 1f, .5f);
            this.AudioManager.Load("blip", "blip", 1f, .2f);

            var titleMeasurement = this.ScreenManager.SpriteFonts["Title"].MeasureString(TitleString);
            var menuMeasurement = Options.Sum(a => this.ScreenManager.SpriteFonts["Menu"].MeasureString(a).Y + 15) - 15;
            var height = titleMeasurement.Y + 10 + menuMeasurement;

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

            if (!_popupEnabled)
                return;

            if (this.InputManager.Keyboard.IsKeyReleased(Keys.Enter))
            {
                //
                switch (_menuIndex)
                {
                    case 0:
                        this.ExitScreen();
                        break;

                    case 1:
                        this.ScreenManager.AddScreen(_popup);
                        _popupEnabled = false;
                        break;

                    case 2:
                        _level.Screen.Next = new MenuScreen();
                        _level.Screen.ExitScreenAnd();
                        break;
                }

                this.AudioManager.Play("confirm");
            }
            else if (this.InputManager.Keyboard.IsKeyReleased(Keys.Escape))
            {
                this.ExitScreen();
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
        }

        /// <summary>
        /// Draws frame
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public override void Draw(GameTime gameTime)
        {
            if (!this.IsTransitioning && this.ScreenState != Services.ScreenState.Active)
                return;

            var transitalpha = (1 - this.TransitionPosition - (1 - _popup.TransitionPosition));

            base.Draw(gameTime);

            this.ScreenManager.SpriteBatch.Begin();
            this.ScreenManager.SpriteBatch.Draw(_texture, new Rectangle(0, 0, this.ScreenManager.ScreenWidth, this.ScreenManager.ScreenHeight), Color.Black * 0.5f * (_popupEnabled ? transitalpha : 1));
            this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Title"], TitleString, _positionTitle, Color.White * transitalpha, _shadowColor * transitalpha);
            var position = _positionMenu;
            for (Int32 i = 0; i < Options.Length; i++)
            {
                var measurement = this.ScreenManager.SpriteFonts["Menu"].MeasureString(Options[i]);
                this.ScreenManager.SpriteBatch.DrawShadowedString(this.ScreenManager.SpriteFonts["Menu"], Options[i], position,
                    Color.White * transitalpha, (_menuIndex == i ? _shadowColor : Color.Black) * transitalpha, 0,
                    (Single)Math.Round(measurement.X / 2) * Vector2.UnitX + (Single)Math.Round(measurement.Y / 2) * Vector2.UnitY,
                    1, SpriteEffects.None, 0);
                position = position + Vector2.UnitY * 15;
            }
            this.ScreenManager.SpriteBatch.End();
        }
    }
}
