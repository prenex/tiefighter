using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIE_Fighter_Forever.BattleField;
using TIE_Fighter_Forever.AI;
using TIE_Fighter_Forever.BattleField.SmallShips;
using Microsoft.Xna.Framework;

namespace TIE_Fighter_Forever.Screens.Battle.Spawning
{
    class Spawner
    {
        BattleFieldComponent battleField;
        Strategist strategist;
        TIEGame game;
        Random rand;

        public Spawner(TIEGame game, BattleFieldComponent battleField, Strategist strategist)
        {
            this.battleField = battleField;
            this.strategist = strategist;
            this.game = game;
            this.rand = new Random();
        }

        public void spawnSmallShip(SmallShip ship, CollidableSpaceObject target)
        {
            float speed = ((float)rand.NextDouble() * 0.75f) + 0.5f;
            CollidableSpaceObjectAI ai = new BruteforceStarFighterAI(1.25f, ship, target, strategist);
            strategist.addSpaceObjectAI(ai);
            battleField.addSmallShip(ship);
        }

        public void spawnQuadLaser(CollidableSpaceObject firingObject)
        {
            Quaternion qRot = firingObject.rotation;
            Vector3 vPos = firingObject.position;
            vPos = vPos + (firingObject.getUpVector() * -1.25f) + (firingObject.getRightVector() * -2.0f) + (firingObject.getForwardVector() * 17.0f);
            battleField.addLaser(new Laser(game, vPos, qRot, new Vector3(5, 10, 5), firingObject.getRay().Direction.Length() + 3.0f));
            vPos = firingObject.position;
            vPos = vPos + (firingObject.getUpVector() * 1.25f) + (firingObject.getRightVector() * -2.0f) + (firingObject.getForwardVector() * 17.0f);
            battleField.addLaser(new Laser(game, vPos, qRot, new Vector3(5, 10, 5), firingObject.getRay().Direction.Length() + 3.0f));
            vPos = firingObject.position;
            vPos = vPos + (firingObject.getUpVector() * 1.25f) + (firingObject.getRightVector() * 2.0f) + (firingObject.getForwardVector() * 17.0f);
            battleField.addLaser(new Laser(game, vPos, qRot, new Vector3(5, 10, 5), firingObject.getRay().Direction.Length() + 3.0f));
            vPos = firingObject.position;
            vPos = vPos + (firingObject.getUpVector() * -1.25f) + (firingObject.getRightVector() * 2.0f) + (firingObject.getForwardVector() * 17.0f);
            battleField.addLaser(new Laser(game, vPos, qRot, new Vector3(5, 10, 5), firingObject.getRay().Direction.Length() + 3.0f));
        }
    }
}
