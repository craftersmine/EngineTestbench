using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineTestbench
{
    internal class Helper
    {
        /// <summary>
        /// Linear interpolation between two values
        /// </summary>
        /// <param name="firstFloat">Start point value</param>
        /// <param name="secondFloat">End point value</param>
        /// <param name="by">Factor</param>
        /// <returns>Linearly interpolated value between two numbers</returns>
        public static float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat + (secondFloat - firstFloat) * by;
        }
    }
}
