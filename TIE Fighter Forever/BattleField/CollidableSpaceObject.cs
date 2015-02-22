using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TIE_Fighter_Forever.BattleField.OtherObjects;
using Microsoft.Xna.Framework.Graphics;
using TypeReaders.CollisionPipelineRuntimeHelper;

namespace TIE_Fighter_Forever.BattleField
{
    /// <summary>
    /// Kirajzolható űrbéli objektum
    /// </summary>
    public abstract class CollidableSpaceObject : SpaceObject
    {
        /// <summary>
        /// Az előző pozíciót kell ebben tárolni!
        /// A változó, a getRay metódushoz lesz felhasználva az ütközések
        /// megvalósításához!
        /// </summary>
        protected Vector3 oldPosition;
        protected TIEGame game;
        /// <summary>
        /// Befoglaló gömb
        /// </summary>
        protected BoundingSphere bSphere;

        /// <summary>
        /// A kirajzolás során használt projekciós mátrix
        /// </summary>
        public Matrix projection;

        /// <summary>
        /// Az objektum életereje
        /// </summary>
        public int life;

        /// <summary>
        /// Az objektumot befoglaló kocka alakú dobozt adja vissza
        /// </summary>
        public BoundingBox getBoundingBox()
        {
            // Visszaadjuk a pozícióval eltolt befoglaló gömb köré írható
            // legkissebb kockát!
            BoundingSphere s = this.bSphere;
            s.Center += vPosition;
            return BoundingBox.CreateFromSphere(s);
        }

        /// <summary>
        /// Az objektumot befoglaló kocka alakú dobozt léptékelve adja vissza.
        /// Hasznos közelségi teszthez
        /// </summary>
        public BoundingBox getProximityBox(float scale)
        {
            // Visszaadjuk a pozícióval eltolt befoglaló gömb köré írható
            // legkissebb kockát!
            BoundingSphere s = this.bSphere;
            s.Center += vPosition;
            s.Radius *= scale;
            return BoundingBox.CreateFromSphere(s);
        }

        // Megvalóstjuk a spaceobject elmozdítós dolgait,
        // közben figyelve a oldPostition mentésére
        #region translating_with_oldPosition_saves
        public override void translate(Vector3 translation)
        {
            oldPosition = vPosition;
            base.translate(translation);
        }
        public override void translateRelative(Vector3 translation)
        {
            oldPosition = vPosition;
            base.translateRelative(translation);
        }
        public override void goForward(float quantity)
        {
            oldPosition = vPosition;
            base.goForward(quantity);
        }
        #endregion

        /// <summary>
        /// Visszaadja az előző pozíciótól a mostaniba mutató
        /// irányított vektort!
        /// </summary>
        /// <returns></returns>
        public Ray getRay()
        {
            Vector3 epsilon = new Vector3(0.1f, 0.0001f, 0.0001f);
            return new Ray(oldPosition, vPosition - oldPosition + epsilon);
        }

        /// <summary>
        /// Megvizsgálja, hogy ütközünk-e a megadott objektummal
        /// </summary>
        /// <param name="other">Az objektum, amivel szemben az ütközésünket vizsgáljuk</param>
        /// <returns>Ütközés esetén igaz, amúgy hamis</returns>
        abstract public bool collisionTest(CollidableSpaceObject other);

        /// <summary>
        /// Megvizsgálja, hogy túl közel vagyunk-e egy objektumhoz
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        abstract public bool proximityTest(CollidableSpaceObject other);

        /// <summary>
        /// Az objektum kirajzolása a jelenlegi állapot szerint.
        /// A kirajzolás felhasználja, a projection propertyben megadott projektáló,
        /// és a paraméterként megadott nézeti mátrixot, illetve a kvaternió és 
        /// pozíció változókból előállított értékeket.
        /// </summary>
        // megj.: Az absztrakt metódusok természetesen alapból virtuálisak is...
        abstract public void draw(Matrix view, Vector3 eye, Light light);

        /// <summary>
        /// Megvizsgálja, hogy a Ray objektum segítségével adott irányított szakasz
        /// ütközik-e a megadott COctree-vel rendelkező háromdimenziós modellel
        /// </summary>
        /// <param name="model">Modell, melynek Tag propertyje egy COCtree-t tartalmaz</param>
        /// <param name="modelWorld">A Modell világmátrixa</param>
        /// <param name="ray">A világtérbeli irányított szakasz a vizsgálathoz, Ray típusú objektumként megadva (nem félegyenes!)</param>
        /// <returns>Történik-e ütközés</returns>
        protected bool raysegmentModelCollision(Model model, Matrix modelWorld, Ray ray)
        {
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            bool collision = false;
            foreach (ModelMesh mesh in model.Meshes)
            {
                Matrix absTransform = modelTransforms[mesh.ParentBone.Index] * modelWorld;

                Matrix invMatrix = Matrix.Invert(absTransform);

                Vector3 transRayStartPoint = Vector3.Transform(ray.Position, invMatrix);
                Vector3 origRayEndPoint = ray.Position + ray.Direction;
                Vector3 transRayEndPoint = Vector3.Transform(origRayEndPoint, invMatrix);
                Ray invRay = new Ray(transRayStartPoint, transRayEndPoint - transRayStartPoint);

                // A sugárhoz(Ray) befoglaló téglatest kialakítása
                Vector3[] rayPoints = new Vector3[2];
                rayPoints[0] = invRay.Position;
                rayPoints[1] = invRay.Position + invRay.Direction;
                BoundingBox invRayBox = BoundingBox.CreateFromPoints(rayPoints);

                // COctree collision
                COctree coctree = (COctree)mesh.Tag;
                collision = coctree.collisionTest(invRay, invRayBox);
                if (collision == true)
                {
                    return true;
                }
            }
            return collision;
        }

        /// <summary>
        /// Megadja mennyi sérülést okoz az objektum az ütközésével
        /// </summary>
        /// <returns></returns>
        abstract public int damageCausedIfCollide();

        /// <summary>
        /// Addot mennyiségű kárt okoz az objektumban
        /// </summary>
        /// <param name="damageToObject"></param>
        abstract public void damage(int damageToObject);
    }
}
