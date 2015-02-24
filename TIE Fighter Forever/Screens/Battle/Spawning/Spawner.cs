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

        private void spawnLaser(Vector3 vPos, Quaternion qRot, SmallShip firingObject, int laserIndex)
        {
            laserIndex = laserIndex % firingObject.getLaserFirePointsDescriptor().firePointCount;
            vPos = vPos + (firingObject.getUpVector() * firingObject.getLaserFirePointsDescriptor().upDisplacements[laserIndex])
                + (firingObject.getRightVector() * firingObject.getLaserFirePointsDescriptor().rightDisplacements[laserIndex])
                + (firingObject.getForwardVector() * firingObject.getLaserFirePointsDescriptor().forwardDisplacements[laserIndex]);
            battleField.addLaser(new Laser(game, vPos, qRot, firingObject.getLaserFirePointsDescriptor().laserColor,
                firingObject.getRay().Direction.Length() + 3.0f,
                firingObject.getLaserFirePointsDescriptor().damageCausedOnCollision));
        }

        /// <summary>
        /// Try to spawn lasers according to the firestate of the given object in the current timeStamp, knowing when last fire happened. If the object can fire, the returned double gets the new lastFired, otherwise "lastFired" is returned!
        /// </summary>
        /// <param name="firingObject">The object that fire</param>
        /// <param name="currentTime">The current milliseconds</param>
        /// <param name="lastFired">The milliseconds when last fire happened</param>
        /// <returns>lastFired or currentmilliseconds depending on fire availability - if fire happens, currentTime is returned!</returns>
        public double spawnLaserForFireStateOf(SmallShip firingObject, double currentTime, double lastFired)
        {
            switch (firingObject.getFireState())
            {
                case FireState.SINGLE:
                    if (currentTime > lastFired + firingObject.getLaserFirePointsDescriptor().waitForSingleFireMs)
                    {
                        spawnLaser(firingObject);
                        return currentTime;
                    }
                    break;
                case FireState.IMPERIAL_DOUBLE:
                    if (currentTime > lastFired + firingObject.getLaserFirePointsDescriptor().waitForSingleFireMs * 2)
                    {
                        spawnEmpireTwinLaser(firingObject);
                        return currentTime;
                    }
                    break;
                case FireState.QUAD:
                    if (currentTime > lastFired + firingObject.getLaserFirePointsDescriptor().waitForSingleFireMs * 4)
                    {
                        spawnQuadLaser(firingObject);
                        return currentTime;
                    }
                    break;
            }
            return lastFired;
        }

        public void spawnLaser(SmallShip firingObject)
        {
            spawnLaser(firingObject.position, firingObject.rotation, firingObject, firingObject.getAndChangeCurrentFirePointWith(1));
        }

        public void spawnQuadLaser(SmallShip firingObject)
        {
            Quaternion qRot = firingObject.rotation;

            spawnLaser(firingObject.position, qRot, firingObject, 0);
            spawnLaser(firingObject.position, qRot, firingObject, 1);
            spawnLaser(firingObject.position, qRot, firingObject, 2);
            spawnLaser(firingObject.position, qRot, firingObject, 3);


            // update current firepoint
            firingObject.getAndChangeCurrentFirePointWith(4);
        }

        /// <summary>
        /// Spaws a twin laser shot that is fired in imperial fashion: top-bottom-top-bottom
        /// </summary>
        /// <param name="firingObject">The object that fires</param>
        public void spawnEmpireTwinLaser(SmallShip firingObject)
        {
            Quaternion qRot = firingObject.rotation;

            // Update current firepoint and decide about created shot
            int fireSelector = (firingObject.getAndChangeCurrentFirePointWith(2) & 2);
            if (fireSelector == 0)
            {
                // Bottom lasers
                spawnLaser(firingObject.position, qRot, firingObject, 0);
                spawnLaser(firingObject.position, qRot, firingObject, 3);
            }
            else
            {
                // Top lasers
                spawnLaser(firingObject.position, qRot, firingObject, 1);
                spawnLaser(firingObject.position, qRot, firingObject, 2);
            }
        }
    }
}
