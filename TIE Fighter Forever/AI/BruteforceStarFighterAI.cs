using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIE_Fighter_Forever.BattleField;
using Microsoft.Xna.Framework;
using TIE_Fighter_Forever.BattleField.SmallShips;

namespace TIE_Fighter_Forever.AI
{
    class BruteforceStarFighterAI : CollidableSpaceObjectAI
    {
        float speed;
        Random rand;
        double lastFired;

        public override int life
        {
            get
            {
                if (governedObject != null)
                    return governedObject.life;
                else
                    return 0;
            }
        }

        public BruteforceStarFighterAI(float maxSpeed, SmallShip governedObj, CollidableSpaceObject target, Strategist strategist)
        {
            this.speed = maxSpeed;
            this.governedAIs = null;
            this.governedObject = governedObj;
            this.target = target;
            this.state = CollidableSpaceObjectAIState.ApproachingTarget;
            this.targetedPosition = target.position;
            this.rand = new Random();
            this.strategist = strategist;
            this.lastFired = 0;
        }

        public override void updateState()
        {
            if (state == CollidableSpaceObjectAIState.ApproachingTarget)
            {
                if ((targetedPosition - governedObject.position).LengthSquared() < 400 * 400)
                    state = CollidableSpaceObjectAIState.AttackingTarget;
            }
            if (state == CollidableSpaceObjectAIState.ApproachingTarget || state == CollidableSpaceObjectAIState.AttackingTarget)
            {
                Vector3 targetMove = target.getRay().Direction * 5.0f;
                targetedPosition = target.position + targetMove;

                if ((targetedPosition - governedObject.position).LengthSquared() < 60 * 60)
                {
                    state = CollidableSpaceObjectAIState.EvadingTarget;
                    float x, y, z;
                    x = (float)((rand.NextDouble() - 0.5) * 2500);
                    y = (float)((rand.NextDouble() - 0.5) * 2500);
                    z = (float)((rand.NextDouble() - 0.5) * 2500);

                    targetedPosition = governedObject.position + new Vector3(x, y, z);
                }
            }
            
            if (state == CollidableSpaceObjectAIState.EvadingTarget)
            {
                if ((targetedPosition - governedObject.position).LengthSquared() < 60 * 60)
                {
                    state = CollidableSpaceObjectAIState.ApproachingTarget;
                }
            }
        }

        public override void updateGovernedObject(GameTime gt)
        {
            if (governedObject != null)
            {
                if (governedObject.life >= 0)
                {
                    if (state == CollidableSpaceObjectAIState.ApproachingTarget || state == CollidableSpaceObjectAIState.EvadingTarget || state == CollidableSpaceObjectAIState.AttackingTarget)
                    {
                        // Meghatározzuk a célpont felé mutató vektor normalizáltját
                        Vector3 targetVector = Vector3.Normalize(targetedPosition - governedObject.position);

                        // Meghatározzuk a célirány-vektor saját koordinátarendszerünk szerinti komponenseit
                        float x = Vector3.Dot(governedObject.getRightVector(), targetVector);
                        float y = Vector3.Dot(governedObject.getUpVector(), targetVector);
                        float z = Vector3.Dot(governedObject.getForwardVector(), targetVector);

                        // Kiszámoljuk az irányfélgömböt
                        if (z >= 0)
                        {
                            // Egy féltér a projektált irányfélgömb felét foglalja el
                            // Ha előttünk az objektum, könnyű dolgunk van
                            x /= 2.0f;
                            y /= 2.0f;
                        }
                        else
                        {
                            // Ha mögöttünk van a cél, akkor meg kell "fordítani" a koordinátákat
                            if (x >= 0)
                                x = 1.0f - x;
                            else
                                x = -1.0f - x;
                            if (y >= 0)
                                y = 1.0f - y;
                            else
                                y = -1.0f - y;
                            // és itt is felezni, mert ez az irányfélgömb egy fele csak ez is
                            x /= 2.0f;
                            y /= 2.0f;
                            // méghozzá a külső fele
                            if (x >= 0)
                                x += 0.5f;
                            else
                                x -= 0.5f;
                            if (y >= 0)
                                y += 0.5f;
                            else
                                y -= 0.5f;
                        }

                        // Kiszámoljuk a projektált irányfélgömb pontjába mutató vektor
                        // és a felfele vektor közrezárt szögét
                        float angle = (float)Math.Acos(Vector2.Dot(Vector2.UnitY, new Vector2(x, y)));
                        if (float.IsNaN(angle))
                            angle = 0.1f;   // hiba elkerülése végett
                        // és a szöget [0..1]-be normáljuk [0..PI]-ról
                        angle /= (float)Math.PI;

                        // ezzel kiszámíthatóvá válnak a végleges szögek
                        float rotAroundX = y;
                        float rotAroundZ;
                        if (x >= 0)
                            rotAroundZ = angle;
                        else
                            rotAroundZ = -angle;

                        governedObject.rotate(0, 0, rotAroundZ * 2.0f);
                        governedObject.rotate(rotAroundX * 3.5f, 0, 0);

                        governedObject.goForward(speed);
                    }

                    if (state == CollidableSpaceObjectAIState.AttackingTarget)
                    {
                        lastFired = strategist.spawnerObj.spawnLaserForFireStateOf((SmallShip)governedObject, gt.TotalGameTime.TotalMilliseconds, lastFired);
                    }
                }
            }
        }

        /// <summary>
        /// A jelen verzióban nem implementáltak a szintek és a parancsok
        /// </summary>
        /// <param name="command"></param>
        public override void addCommand(AICommand command)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Beállítja a célpontot.
        /// </summary>
        /// <param name="target"></param>
        public override void setTarget(CollidableSpaceObject target)
        {
            this.target = target;
        }

        /// <summary>
        /// Sets the governed object - this case it can be only a SmallShip instance
        /// </summary>
        /// <param name="obj"></param>
        public override void setGovernedObject(CollidableSpaceObject obj)
        {
            this.governedObject = obj;
        }
    }
}
