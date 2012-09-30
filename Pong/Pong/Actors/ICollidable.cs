using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Pong.Actors
{
    /// <summary>
    /// Exposes information needed for collision management
    /// </summary>
    public interface ICollidable : IGameComponent, IUpdateable, IDrawable, IDisposable
    {
        /// <summary>
        /// The top-left position of the object
        /// </summary>
        Vector2 Position { get; }
        /// <summary>
        /// The velocity of the object
        /// </summary>
        Vector2 Velocity { get; }
        /// <summary>
        /// The size of the object
        /// </summary>
        Vector2 Size { get; }
        /// <summary>
        /// Indicates wether the object is a solid
        /// </summary>
        Boolean IsSolid { get; }

        /// <summary>
        /// Allows an object to react to a collision and get itself 'unstuck'
        /// </summary>
        /// <param name="other">The collidee</param>
        void HandleCollision(ICollidable other);

        void LoadContent(ContentManager contentManager);
    }
}
