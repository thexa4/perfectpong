using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Pong.Level;

namespace Pong.Services
{
    public class GameSettings
    {
        // define singleton
        private static GameSettings _instance = new GameSettings();

        /// <summary>
        /// Influence ratio of the place where the ball hits the paddle
        /// </summary>
        public Single PaddleDirectionalInfluence { get; set; }

        /// <summary>
        /// Influence ratio of movement of the paddle on the speed of the ball
        /// </summary>
        public Single PaddleSpinInfluence { get; set; }

        /// <summary>
        /// Move speed paddle in pixels per second
        /// </summary>
        public Single PaddleMoveSpeed { get; set; }

        /// <summary>
        /// Paddle size in pixels
        /// </summary>
        public Vector2 PaddleSize { get; set; }

        /// <summary>
        /// Paddle width in pixels
        /// </summary>
        public Single PaddleWidth { get { return PaddleSize.X;  } set { PaddleSize = Vector2.UnitX *value + Vector2.UnitY * PaddleSize.Y; } }

        /// <summary>
        /// Paddle height in pixels
        /// </summary>
        public Single PaddleHeight { get { return PaddleSize.Y; } set { PaddleSize = Vector2.UnitY * value + Vector2.UnitX * PaddleSize.X; } }

        /// <summary>
        /// Ball speed when launched
        /// </summary>
        public Single BallStartSpeed { get; set; }

        /// <summary>
        /// Ratio of speed increase when ball hits the paddle
        /// </summary>
        public Single BallCollisionSpeed { get; set; }

        /// <summary>
        /// Ball size in pixels
        /// </summary>
        public Vector2 BallSize { get; private set; }

        /// <summary>
        /// Ball width in pixels (uniform width/height)
        /// </summary>
        public Single BallWidth { get { return BallSize.X; } set { BallSize = Vector2.UnitX * value + Vector2.UnitY * value; } }

        /// <summary>
        /// Ball height in pixels (uniform width/height)
        /// </summary>
        public Single BallHeight { get { return BallSize.Y; } set { BallSize = Vector2.UnitY * value + Vector2.UnitX * value; } }

        /// <summary>
        /// Water displacement size in pixels
        /// </summary>
        public Single WaterDisplacementSize { get; set; }

        /// <summary>
        /// Sound volume
        /// </summary>
        public Single SoundVolume { get; set; }

        /// <summary>
        /// Level size
        /// </summary>
        public Vector2 LevelSize { get; set; }

        /// <summary>
        /// Number of lives a player has
        /// </summary>
        public Single PlayerLives { get; set; }

        /// <summary>
        /// Number of players
        /// </summary>
        public List<Player> Players { get; set; }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static GameSettings Instance
        {
            get {
                return _instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public GameSettings()
        {
            PaddleDirectionalInfluence = 50;
            PaddleSpinInfluence = 20;
            PaddleMoveSpeed = 130;
            PaddleSize = new Vector2(100, 100);

            BallStartSpeed = 110;
            BallCollisionSpeed = 10;
            BallSize = Vector2.One * 10;

            WaterDisplacementSize = 1;
            SoundVolume = 100;

            LevelSize = new Vector2(1280, 720);
            PlayerLives = 5;

            Players = new List<Player>();
        }
    }
}
