using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TIE_Fighter_Forever.BattleField
{
    /// <summary>
    /// Holds descriptions about a laser fire point that can be used to spawn projectiles
    /// </summary>
    public class LaserFirePointsDescriptor
    {
        /// <summary>
        /// Color for the projectiles created for this firing point
        /// </summary>
        public Vector3 laserColor = new Vector3(5, 10, 5);

        /// <summary>
        /// Defines how much time the firing object waits for a single shot
        /// </summary>
        public float waitForSingleFireMs = 250;

        /// <summary>
        /// Defines how much damage the projectiles of the fire-points cause
        /// </summary>
        public int damageCausedOnCollision = 5;

        /// <summary>
        /// Defines how many firepoints displacements are held in this object.
        /// </summary>
        public readonly int firePointCount;
        /// <summary>
        /// Holds float values of the displacement of the Nth laser fire point along the firing object's up vector.
        /// </summary>
        public readonly float[] upDisplacements;
        /// <summary>
        /// Holds float values of the displacement of the Nth laser fire point along the firing object's right vector. 
        /// </summary>
        public readonly float[] rightDisplacements;
        /// <summary>
        /// Holds float values of the displacement of the Nth laser fire point  along the firing object's forward vector.
        /// </summary>
        public readonly float[] forwardDisplacements;

        public LaserFirePointsDescriptor(int firePointCount, float[] upDisplacements, float[] rightDisplacements, float[] forwardDisplacements)
        {
            this.firePointCount = firePointCount;
            this.upDisplacements = upDisplacements;
            this.rightDisplacements = rightDisplacements;
            this.forwardDisplacements = forwardDisplacements;
        }
    }
}
