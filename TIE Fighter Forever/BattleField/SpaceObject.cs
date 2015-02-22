using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TIE_Fighter_Forever.BattleField
{
    public abstract class SpaceObject
    {
        /// <summary>
        /// A kvaternióval reprezentált forgatása az objektumban
        /// </summary>
        protected Quaternion qRotation;
        /// <summary>
        /// Az objektum pozíciója
        /// Megj.: CollidableSpaceObject esetén ennek változtatása egy hatalmas
        /// méretű ray-hez vezet, melytől az adott objektum felrobbanhat ha az új
        /// és az előző objektum között volt valami!
        /// </summary>
        protected Vector3 vPosition;

        /// <summary>
        /// Az objektum pozíciója
        /// </summary>
        public Vector3 position
        {
            get { return vPosition; }
        }

        /// <summary>
        /// Az objektum forgatási kvaterniója
        /// </summary>
        public Quaternion rotation
        {
            get { return qRotation; }
        }

        public SpaceObject()
        {
            qRotation = Quaternion.Identity;
            vPosition = Vector3.Zero;
        }

        /// <summary>
        /// Az objektumot forgatja el a jelenlegi X,Y,Z tengelyei körül a megadott szögekkel
        /// A bemeneti szögek értékeit fokban kell megadni
        /// </summary>
        /// <param name="rotationX">Objektum forgatási szöge az Y és Z tengely körüli forgatásokkal transzformált X tengely körül</param>
        /// <param name="rotationY">Objektum forgatási szöge a Z tengely körüli forgatással transzformált Y tengely körül</param>
        /// <param name="rotationZ">Objektum forgatási szöge a Z tengely körül</param>
        virtual public void rotate(float rotationX, float rotationY, float rotationZ)
        {
            qRotation = qRotation * Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(rotationX)) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(rotationY)) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(rotationZ));
        }

        /// <summary>
        /// Visszaadja az objektum világtér-béli irányvektorát
        /// </summary>
        /// <returns>Objektum előre mutató vektora</returns>
        virtual public Vector3 getForwardVector()
        {
            return Vector3.Transform(Vector3.Forward, contructRotationMatrix());
        }

        /// <summary>
        /// Visszaadja az objektum világtér-béli, felfele irányuló vektorát
        /// </summary>
        /// <returns>Objektum előre mutató vektora</returns>
        virtual public Vector3 getUpVector()
        {
            return Vector3.Transform(Vector3.Up, contructRotationMatrix());
        }

        /// <summary>
        /// Visszaadja az objektum világtér-béli, jobbra irányuló vektorát
        /// </summary>
        /// <returns>Objektum előre mutató vektora</returns>
        virtual public Vector3 getRightVector()
        {
            return Vector3.Transform(Vector3.Right, contructRotationMatrix());
        }

        /// <summary>
        /// Az objektumot eltolja a jelenlegi pozíciójához képest a világtérbeli koordinátarendszer bázisa szerint
        /// </summary>
        /// <param name="translation">eltolásvektor</param>
        virtual public void translate(Vector3 translation)
        {
            this.vPosition += translation;
        }

        /// <summary>
        /// Az objektumot eltolja a megadott vektorral a saját koordinátarenszerében vett bázis szerint
        /// Megj.: Ha az objektumot előre akarjuk csak mozgatni, használjuk a gyorsabb goForward függvényt!
        /// </summary>
        /// <param name="translation">Eltolásvektor</param>
        virtual public void translateRelative(Vector3 translation)
        {
            Vector3 f, u, r;
            f = getForwardVector(); u = getUpVector(); r = getRightVector();
            this.vPosition += (f * translation.X) + (u * translation.Y) + (r * translation.Z);
        }

        /// <summary>
        /// Elmozgatja az objektumot az előre vektorának az irányában
        /// </summary>
        /// <param name="quantity">Elmozdulásvektor hossza</param>
        virtual public void goForward(float quantity)
        {
            this.vPosition += (getForwardVector() * quantity);
        }

        /// <summary>
        /// Visszadja az objektum kvaterniójából származó forgatási mátrixot
        /// </summary>
        /// <returns>Forgatási mátrix</returns>
        virtual public Matrix contructRotationMatrix()
        {
            return Matrix.CreateFromQuaternion(qRotation);
        }
    }
}
