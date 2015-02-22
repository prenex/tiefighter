using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIE_Fighter_Forever.BattleField;
using TIE_Fighter_Forever.Screens.Battle.Spawning;
using Microsoft.Xna.Framework;

namespace TIE_Fighter_Forever.AI
{
    class BruteforceStrategist : Strategist
    {
        protected CollidableSpaceObjectAI[] spaceObjectAIs;
        protected int nextAIIndex;
        protected int maxSpaceObjAINum;

        public BruteforceStrategist(BattleFieldComponent battleField, int maxSpaceObjAINum)
        {
            this.battleField = battleField;
            this.spaceObjectAIs = new CollidableSpaceObjectAI[maxSpaceObjAINum];
            this.maxSpaceObjAINum = maxSpaceObjAINum;
            this.nextAIIndex = 0;
        }

        public override void updateState()
        {
            for(int i = 0; i < maxSpaceObjAINum; ++i)
            {
                if (spaceObjectAIs[i] != null)
                {
                    spaceObjectAIs[i].updateState();
                }
            }
        }

        public override void updateGovernedSpaceObjects(GameTime gt)
        {
            foreach (CollidableSpaceObjectAI ai in spaceObjectAIs)
            {
                if(ai != null)
                    ai.updateGovernedObject(gt);
            }
        }

        public override void addSpaceObjectAI(CollidableSpaceObjectAI ai)
        {
            bool l = false;
            for (int i = 0; i < maxSpaceObjAINum; ++i)
            {
                if (spaceObjectAIs[nextAIIndex] == null)
                {
                    l = true;
                    break;
                }
                else if (spaceObjectAIs[nextAIIndex].life < 0)
                {
                    l = true;
                    break;
                }
                nextAIIndex = (nextAIIndex + 1) % maxSpaceObjAINum;
            }
            if (l)
            {
                spaceObjectAIs[nextAIIndex] = ai;
            }
        }
    }
}
