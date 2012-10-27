using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PerfectPong.Level;
using Microsoft.Xna.Framework.Content;
using PerfectPong.Services;

namespace PerfectPong.Actors
{
    /// <summary>
    /// Kills a player if touched by a ball
    /// </summary>
    public class DeadZone : DrawableGameComponent, ICollidable
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get { return Vector2.Zero; } }
        public Vector2 Size { get; set; }
        public Boolean IsSolid { get { return false; } }

        /// <summary>
        /// The player it belongs to
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// Level reference
        /// </summary>
        protected Level.Level Level { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="player"></param>
        public DeadZone(Game game, Level.Level level, Player player) : base(game)
        {
            this.Player = player;
            this.Level = level;
            this.Visible = false;

            ((CollisionManager)game.Services.GetService(typeof(CollisionManager))).Register(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentManager"></param>
        public void LoadContent(ContentManager contentManager)
        {
            // Support so you could make these visible 
        }

        /// <summary>
        /// Handles a collision with a ball, removes a live from a player
        /// </summary>
        /// <param name="other"></param>
        public void HandleCollision(ICollidable other)
        {
            if (other.GetType() != typeof(Ball))
                return;

            this.Player.Lives--;
            this.Level.Reset();
        }
    }
}
