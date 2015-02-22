using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TIE_Fighter_Forever.BattleField.OtherObjects;

namespace TIE_Fighter_Forever.BattleField.SmallShips
{
    class Laser : SmallShip
    {
        Model model;
        Vector3 em;
        public float speed;
        /// <summary>
        /// Létrehoz egy TIE Interceptort
        /// </summary>
        /// <param name="game"></param>
        /// <param name="postition"></param>
        /// <param name="rotation"></param>
        public Laser(TIEGame game, Vector3 position, Quaternion rotation, Vector3 emissiveColor, float speed)
        {
            // Életpont pozítívra hozása(nem használjuk)
            this.life = 1;

            // Paraméterek mentése
            this.game = game;
            this.vPosition = position;
            this.oldPosition = position - new Vector3(0, 0, 0.1f);
            this.qRotation = rotation;
            this.em = emissiveColor;
            this.speed = speed;

            // Modellek betöltése
            model = game.Content.Load<Model>("BattleContent\\Bullets\\Bullet3");

            // Előállítjuk az egész modellt befoglaló gömböt
            // ebből a gömbből állítja elő az ősosztály a boundingBox property
            // értékét. Belső reprezentációra azért használunk gömböt, mert azt 
            // könnyebb egy updata során eltolni, mint egy boxot(az eltolást is az
            // ősosztálybéli kódban végezzük)
            bSphere = new BoundingSphere();
            foreach (ModelMesh mesh in model.Meshes)
            {
                bSphere = BoundingSphere.CreateMerged(bSphere, mesh.BoundingSphere);
            }
        }

        public override void draw(Matrix view, Vector3 eye, Light light)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Alpha = 0.1f;
                    effect.EmissiveColor = em;
                    effect.World = transforms[mesh.ParentBone.Index] *
                                   Matrix.CreateFromQuaternion(qRotation) *
                                   Matrix.CreateTranslation(vPosition);
                    effect.Projection = projection;
                    effect.View = view;
                }
                mesh.Draw();
            }
        }

        public override bool proximityTest(CollidableSpaceObject other)
        {
            return this.getProximityBox(1.5f).Intersects(other.getBoundingBox());
        }

        /// <summary>
        /// Megvizsgálja, hogy a lövedék közepe belemászott-e az adott kishajó objektumba
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool collisionTest(CollidableSpaceObject other)
        {
            Vector3[] poss = new Vector3[2];
            poss[0] = this.getRay().Position;
            poss[1] = this.getRay().Position + this.getRay().Direction;
            BoundingBox bb = BoundingBox.CreateFromPoints(poss);

            // Ha nem ütköznek ezek a bounding boxok, akkor ütközés nem lehet!
            if (!bb.Intersects(other.getBoundingBox()))
                return false;
            // Ha ütköznek, akkor érdemes megvizsgálni az esetet
            else
            {
                // Ha a végpont benne van a célobjektum bounding boxában
                // akkor természetesen ütközés van...
                if (other.getBoundingBox().Contains(poss[1]) == ContainmentType.Contains)
                    return true;
                // Ha nincs, akkor tovább vizsgálódunk
                else
                {
                    // dist tartalmazza a sugár és a befoglaló doboz legközelebbi
                    // metszéspontjának távolságát a sugár kezdőpozíciójától
                    // vagy null-t, ha a sugár nem metszi a boxot.
                    float? dist = this.getRay().Intersects(other.getBoundingBox());
                    if (dist != null)
                    {
                        float d = (float)dist;
                        // "vég - kezdet": sugár vektora
                        Vector3 v = (this.getRay().Position +
                                    this.getRay().Direction) -
                                    (this.getRay().Position);
                        // Ha a metszéspont közelebb van, mint v hossza, akkor
                        // ütközés történt
                        if ((d * d) < v.LengthSquared())
                            return true;
                    }
                    // Minden egyéb esetben nem történt ütközés
                    return false;
                }
            }
        }

        public override void damage(int damageToObject)
        {
            life -= damageToObject;
        }

        public override int damageCausedIfCollide()
        {
            return 5;
        }
    }
}
