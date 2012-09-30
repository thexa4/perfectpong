using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Pong.Services
{
    public interface IPaddleController
    {
        /// <summary>
        /// Direction of the paddle controller
        /// </summary>
        PaddleDirection Direction { get; }

        void Initialize();
        void Update(GameTime gameTime);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IPaddleAIController : IPaddleController
    {
        Level.Level Level { set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum PaddleDirection {
        None = 0,

        Up = 1,
        Down = 2,
    }
}
