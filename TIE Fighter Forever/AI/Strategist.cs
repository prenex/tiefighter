using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIE_Fighter_Forever.BattleField;
using TIE_Fighter_Forever.Screens.Battle.Spawning;
using Microsoft.Xna.Framework;

namespace TIE_Fighter_Forever.AI
{
    abstract class Strategist
    {
        protected BattleFieldComponent battleField;
        protected Spawner spawner;

        public Spawner spawnerObj
        {
            get { return spawner; }
            set { spawner = value; }
        }

        /// <summary>
        /// Frissíti a stratégiai AI belső állapotát
        /// </summary>
        abstract public void updateState();

        /// <summary>
        /// Frissíti a stratégiai AI által uralt űrobjektumok állapotát
        /// </summary>
        abstract public void updateGovernedSpaceObjects(GameTime gt);

        /// <summary>
        /// Új AI-t ad hozzá a stratégiai AI-hoz
        /// </summary>
        /// <param name="ai"></param>
        abstract public void addSpaceObjectAI(CollidableSpaceObjectAI ai);
    }
}
