using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PerfectPong.Services;

namespace PerfectPong.Level
{
    /// <summary>
    /// Stores player info
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Player lives
        /// </summary>
        public Int32 Lives { get; set; }

        /// <summary>
        /// Current Level 
        /// </summary>
        public Level Level { get; set; }

        /// <summary>
        /// Player controller
        /// </summary>
        public IPaddleController Controller { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public Player(Int32 id)
        {
            this.Lives = (Int32)GameSettings.Instance.PlayerLives; // is a single so it can dynamically be set
            this.Id = id;
        }

        /// <summary>
        /// Player Id (also for saving/loading profiles?)
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// Player paddle
        /// </summary>
        public Actors.Paddle Paddle { get; set; }
    }
}
