using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pong.Services;
using Microsoft.Xna.Framework;
using Pong.Level;

namespace Pong.Screens
{
    public class PlayingScreen : GameScreen
    {
        private Level.Level _level;
        public WaterEffect Water { get; protected set; }
        protected HeadsUpDisplayPopup _hud;
        protected PauseScreen _pause;
        protected Boolean _pauseEnabled;

        /// <summary>
        /// Initializes Screen
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            _level = new Level.Level(this.Game, this);
            _level.Initialize();

            this.Water = new WaterEffect(Game, this.ScreenManager.ScreenWidth, this.ScreenManager.ScreenHeight);
            this.Water.Initialize();

            this.TransitionOnTime = TimeSpan.FromSeconds(.5f);
            this.TransitionOffTime = TimeSpan.FromSeconds(.5f);
        }

        /// <summary>
        /// After screen is added to the screenmanager
        /// </summary>
        public override void PostProcessing()
        {
            base.PostProcessing();

            _hud = new HeadsUpDisplayPopup(_level);
            this.ScreenManager.AddScreen(_hud);
            this.Exiting += new EventHandler(PlayingScreen_Exiting);

            _pause = new PauseScreen(_level);
            _pauseEnabled = true;
        }

        /// <summary>
        /// Screen Exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PlayingScreen_Exiting(object sender, EventArgs e)
        {
            _hud.ExitScreen();
        }

        /// <summary>
        /// Loads all content
        /// </summary>
        /// <param name="contentManager"></param>
        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager contentManager)
        {
            base.LoadContent(contentManager);
            this.AudioManager.Load("blip", "confirm", 1f, .5f);
            this.AudioManager.Load("blip", "blip", 1f, .2f);

            this.Water.LoadContent(contentManager);
            _level.LoadContent(contentManager);
        }

        /// <summary>
        /// Frame Renewal
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing Values</param>
        /// <param name="otherScreenHasFocus">!Game.IsActive</param>
        /// <param name="coveredByOtherScreen"></param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Don't update level if in transit.
            if (!this.IsTransitioning && this.ScreenState == Services.ScreenState.Active && !_pause.IsActive && !otherScreenHasFocus)
                _level.Update(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!this.IsTransitioning && this.ScreenState != Services.ScreenState.Active)
                return;

            base.Draw(gameTime);

            this.Water.Draw(gameTime);

            foreach (var actor in _level.Actors)
            {
                DrawableGameComponent drawable = actor as DrawableGameComponent;
                if (drawable != null)
                    drawable.Draw(gameTime);
            }

            foreach (var ball in _level.Balls)
                ball.Draw(gameTime);

            this.ScreenManager.FadeBackBufferToBlack((Byte)(255 - this.TransitionAlpha));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void HandleInput(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.HandleInput(gameTime);
            foreach (var player in _level.Players)
                player.Controller.Update(gameTime);

            if (_pauseEnabled && this.InputManager.Keyboard.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                _pause = new PauseScreen(_level);
                _pause.Exited += new EventHandler(_pause_Exited);
                _pauseEnabled = false;

                this.ScreenManager.AddScreen(_pause);
                this.AudioManager.Play("confirm"); 
            }
        }

        void _pause_Exited(object sender, EventArgs e)
        {
            _pauseEnabled = true;
        }
    }
}
