using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIE_Fighter_Forever.BattleField
{
    interface LaserFirePointHolder
    {
        /// <summary>
        /// Gets the description of firing points
        /// </summary>
        LaserFirePointsDescriptor getLaserFirePointsDescriptor();
        /// <summary>
        /// Gets the current fire point
        /// <param name="increment">How much to change the current firepoint selection</param>
        /// </summary>
        int getAndChangeCurrentFirePointWith(int increment);
    }
}
