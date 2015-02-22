using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIE_Fighter_Forever.BattleField;
using Microsoft.Xna.Framework;

namespace TIE_Fighter_Forever.AI
{
    abstract class CollidableSpaceObjectAI
    {
        protected CollidableSpaceObjectAIState state;
        protected CollidableSpaceObject governedObject;
        protected CollidableSpaceObject target;
        protected Vector3 targetedPosition;
        protected Strategist strategist;

        public abstract int life
        {
            get;
        }

        /// <summary>
        /// Az AI által irányíott alattvaló AI-k
        /// </summary>
        public CollidableSpaceObjectAI[] governedAIs;

        /// <summary>
        /// Beállítja az AI által irányított űrobjektumot
        /// </summary>
        /// <param name="obj"></param>
        abstract public void setGovernedObject(CollidableSpaceObject obj);

        /// <summary>
        /// Frissíti az AI által irányított űrobjektumot
        /// </summary>
        abstract public void updateGovernedObject(GameTime gt);

        /// <summary>
        /// Frissíti az AI belső állapotát
        /// </summary>
        abstract public void updateState();

        /// <summary>
        /// Általános parancsot ad az AI-nak
        /// </summary>
        /// <param name="command"></param>
        abstract public void addCommand(AICommand command);

        /// <summary>
        /// Beállítja az objektum célpontját
        /// </summary>
        /// <param name="target"></param>
        abstract public void setTarget(CollidableSpaceObject target);
    }
}
