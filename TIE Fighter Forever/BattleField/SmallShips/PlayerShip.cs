using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TIE_Fighter_Forever.BattleField.SmallShips
{
    public class PlayerShip : SmallShip, LaserFirePointHolder
    {
        // Current default maximum heath,, TODO: make this changable!
        public const int maxHealth = 120;

        // Current default maximum speed, TODO: make this changable!
        public float maxSpeed = 2.0f;

        /// <summary>
        /// Létrehozza a játékos belőnézetes kameráját a megadott kezdőpozícióval és elforgatással
        /// </summary>
        /// <param name="position">Kezdőpozíció</param>
        /// <param name="rotationX">Játékos forgatási szöge az Y és Z tengely körüli forgatásokkal transzformált X tengely körül</param>
        /// <param name="rotationY">Játékos forgatási szöge a Z tengely körüli forgatással transzformált Y tengely körül</param>
        /// <param name="rotationZ">Játékos forgatási szöge a Z tengely körül</param>
        /// <param name="radius">Játékos hajójának mérete</param>
        public PlayerShip(Vector3 position, float rotationX, float rotationY, float rotationZ, float radius)
        {
            this.possibleFireStates = new FireState[3] { FireState.SINGLE, FireState.IMPERIAL_DOUBLE, FireState.QUAD };
            this.firePoints.laserColor = new Vector3(10, 7, 0);

            this.life = maxHealth;    // 100+20 RU: Tie Advanced
            this.vPosition = position;
            this.oldPosition = position - new Vector3(0,0,0.1f);
            this.qRotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(rotationX)) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(rotationY)) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(rotationZ));
            this.bSphere = new BoundingSphere(Vector3.Zero, radius);
        }

        /// <summary>
        /// A kamerát forgatja el a jelenlegi X,Y,Z tengelyei körül a megadott szögekkel
        /// A bemeneti szögek értékeit fokban kell megadni
        /// </summary>
        /// <param name="rotationX">Kamera forgatási szöge az Y és Z tengely körüli forgatásokkal transzformált X tengely körül</param>
        /// <param name="rotationY">Kamera forgatási szöge a Z tengely körüli forgatással transzformált Y tengely körül</param>
        /// <param name="rotationZ">Kamera forgatási szöge a Z tengely körül</param>
        // Megj.: Ezt azért kell felüldefiniálni, mert a kamera(a többi spaceobjecttől eltérően)
        // nem a világtérben mozog, hanem a nézeti térben, ahol a transzformációk ellentettje jelenti a kamera helyes transzformálását!
        public override void rotate(float rotationX, float rotationY, float rotationZ)
        {
            qRotation = qRotation * Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathHelper.ToRadians(-rotationX)) * Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(-rotationY)) * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(-rotationZ));
        }

        /// <summary>
        /// Visszaadja a kamera jelenlegi állapotához tartozó nézeti mátrixot
        /// </summary>
        /// <returns>View mátrix</returns>
        public Matrix contructViewMatrix()
        {
            return Matrix.CreateTranslation(-vPosition) * Matrix.Transpose(contructRotationMatrix());
        }

        public override bool proximityTest(CollidableSpaceObject other)
        {
            return this.getProximityBox(2.0f).Intersects(other.getBoundingBox());
        }

        public override bool collisionTest(CollidableSpaceObject other)
        {
            BoundingBox b1, b2;
            b1 = this.getBoundingBox();
            b2 = other.getBoundingBox();
            return this.getBoundingBox().Intersects(other.getProximityBox(0.15f));
        }

        public override void draw(Matrix view, Vector3 eye, OtherObjects.Light light)
        {
            // A player semmit se rajzol ki most(később lehetne itt modell pl.)
        }

        public override void damage(int damageToObject)
        {
            this.life -= damageToObject;
        }

        public override int damageCausedIfCollide()
        {
            return 120;
        }
    }
}
