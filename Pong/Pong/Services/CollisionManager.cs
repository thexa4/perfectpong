using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PerfectPong.Actors;

namespace PerfectPong.Services
{
    public class CollisionManager : GameComponent
    {
        private List<ICollidable> _collidables;

        /// <summary>
        /// Creates a new CollisionManager, detects collisions and moves actors around based on velocity
        /// </summary>
        /// <param name="game"></param>
        public CollisionManager(Game game) : base(game)
        {
            // Add this as a service
            this.Game.Services.AddService(typeof(CollisionManager), this);
        }

        /// <summary>
        /// Initializes Manager
        /// </summary>
        public override void Initialize()
        {
            _collidables = new List<ICollidable>();

            base.Initialize();
        }


        /// <summary>
        /// Registers a new collidable
        /// </summary>
        /// <param name="collidable"></param>
        public void Register(ICollidable collidable) 
        {
            _collidables.Add(collidable);
        }

        /// <summary>
        /// Frame Renewal, move actors and detect collisions
        /// </summary>
        /// <param name="gameTime">Snapshot of Timing values</param>
        public override void  Update(GameTime gameTime)
        {
 	        base.Update(gameTime);

            ICollidable[] activeCollidables = new ICollidable[_collidables.Count];
            _collidables.CopyTo(activeCollidables);

            foreach (ICollidable collider in activeCollidables)
            {
                foreach (ICollidable collidee in activeCollidables)
                {
                    if (collider.Equals(collidee))
                        continue;

                    // Boundingbox intersection using demorgen to see if two collidables overlap
                    if (collider.Position.X < (collidee.Position.X + collidee.Size.X) &&
                        (collider.Position.X + collider.Size.X) > collidee.Position.X &&
                        collider.Position.Y < (collidee.Position.Y + collidee.Size.Y) &&
                        (collider.Position.Y + collider.Size.Y) > collidee.Position.Y)
                        {
                            collider.HandleCollision(collidee);
                        }
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            _collidables.Clear();
        }
    }
}
