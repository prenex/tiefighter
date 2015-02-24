using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TIE_Fighter_Forever.BattleField.OtherObjects;

namespace TIE_Fighter_Forever.BattleField.SmallShips
{
    public abstract class SmallShip : CollidableSpaceObject, LaserFirePointHolder
    {
        public FireState[] possibleFireStates = new FireState[1] { FireState.SINGLE };

        /// <summary>
        /// Must be one of possibleFireStates
        /// </summary>
        protected FireState fireState = FireState.SINGLE;

        protected LaserFirePointsDescriptor firePoints = new LaserFirePointsDescriptor(4, new float[4] { -1.25f, 1.25f, 1.25f, -1.25f }, new float[4] { -2.0f, -2.0f, 2.0f, 2.0f }, new float[4] { 17, 17, 17, 17 });
        protected int currFirePoint = 0;

        public LaserFirePointsDescriptor getLaserFirePointsDescriptor()
        {
            return this.firePoints;
        }

        /// <summary>
        /// Gets the current FireState.
        /// </summary>
        /// <returns>The current FireState</returns>
        public FireState getFireState()
        {
            return this.fireState;
        }

        /// <summary>
        /// Shuffle the current FireState if there are multiple possible fireStates.
        /// </summary>
        public void shuffleFireState()
        {
            int i = 0;
            while (possibleFireStates.ElementAt(i) != fireState) {
                i = (i + 1) % possibleFireStates.Count();
            }

            i = (i + 1) % possibleFireStates.Count();
            fireState = possibleFireStates.ElementAt(i);
        }

        public int getAndChangeCurrentFirePointWith(int increment)
        {
            currFirePoint = (currFirePoint + increment) % firePoints.firePointCount;
            return currFirePoint;
        }
    }
}
