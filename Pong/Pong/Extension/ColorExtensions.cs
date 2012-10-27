using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PerfectPong.Extension
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Returns a rainbox color that oscilates through the components including black
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static Color Rainbow(GameTime gameTime, Double speed = 3)
        {
            var t = gameTime.TotalGameTime.TotalMilliseconds * speed % 1000;
            var rt = (t / 500);
            var gt = ((t - 250) / 500);
            var bt = ((t - 500) / 500);
            return new Color(
                (Single)(t < 500 ? (rt < 0.5f ? (rt * 2) : 1 - (rt - 0.5) * 2) : 0),
                (Single)(t < 750 && t > 250 ? (gt < 0.5f ? (gt * 2) : 1 - (gt - 0.5) * 2) : 0),
                (Single)(t > 500 ? (bt < 0.5f ? (bt * 2) : 1 - (bt - 0.5) * 2) : 0)
            );
        }

        /// <summary>
        /// Returns a rainbox color that oscilates through the components excluding black
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static Color RainbowContinuous(GameTime gameTime, Double speed = 0.1)
        {
            var t = gameTime.TotalGameTime.TotalMilliseconds * speed % 1000;
            var rt = (t / 666);
            var gt = ((t - 333) / 666);
            var bt = t < 333 ? t / 666 + 0.5 : (t - 666) / 666;
            var values = new Vector3(
                (Single)(t < 666 ? (rt < 0.5f ? (rt * 2) : 1 - (rt - 0.5) * 2) : 0),
                (Single)(t > 333 ? (gt < 0.5f ? (gt * 2) : 1 - (gt - 0.5) * 2) : 0),
                (Single)(t > 666 || t < 333 ? (bt < 0.5f ? (bt * 2) : 1 - (bt - 0.5) * 2) : 0)
            );
            var max = Math.Max(values.X, Math.Max(values.Y, values.Z));
            return new Color(values / max);
        }

        //public static void Rainbow(this Color color)
        //{

        //}
    }
}
