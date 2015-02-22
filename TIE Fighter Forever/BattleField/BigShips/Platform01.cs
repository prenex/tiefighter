using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TIE_Fighter_Forever.BattleField.OtherObjects;
using TypeReaders.CollisionPipelineRuntimeHelper;

namespace TIE_Fighter_Forever.BattleField.BigShips
{
    class Platform01 : BigShip
    {
        Model mainModel;
        Model collosionModel;

        /// <summary>
        /// Létrehoz egy 01-es típusú platformot
        /// </summary>
        /// <param name="game"></param>
        /// <param name="postition"></param>
        /// <param name="rotation"></param>
        public Platform01(TIEGame game, Vector3 position, Quaternion rotation)
        {
            // paramétermentés
            this.game = game;
            this.vPosition = position;
            this.oldPosition = position - position - new Vector3(0, 0, 0.1f); ;
            this.qRotation = rotation;
            this.life = 200; //1216 + 2560;   // 1216 + 2560 RU HULL(egy lövés 5-t visz le)

            // Modell betöltés
            mainModel = game.Content.Load<Model>("BattleContent\\platform01");
            collosionModel = game.Content.Load<Model>("BattleContent\\platform01_coll");
            
            //mainModel = game.Content.Load<Model>("BattleContent\\NebulonB");

            // Előállítjuk az egész modellt befoglaló gömböt
            // ebből a gömbből állítja elő az ősosztály a boundingBox property
            // értékét. Belső reprezentációra azért használunk gömböt, mert azt 
            // könnyebb egy updata során eltolni, mint egy boxot(az eltolást is az
            // ősosztálybéli kódban végezzük)
            // Ezen felül itt állítjuk be a sosem változó shaderkonstansainkat is!
            Vector4 lightColor = new Vector4(1, 1, 1, 1);
            Vector4 ambientLightColor = new Vector4(.2f, .2f, .2f, 1);
            float shininess = .8f;
            float specularPower = 3.0f;
            bSphere = new BoundingSphere();
            foreach (ModelMesh mesh in mainModel.Meshes)
            {
                bSphere = BoundingSphere.CreateMerged(bSphere, mesh.BoundingSphere);
                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["Shininess"].SetValue(shininess);
                    effect.Parameters["SpecularPower"].SetValue(specularPower);
                }
            }
        }

        public override void draw(Matrix view, Vector3 eye, Light light)
        {
            // Ezek a renderstate-ek fontosak!!!
            game.GraphicsDevice.SamplerStates[0] = game.settings.wrapFilter;
            game.GraphicsDevice.SamplerStates[1] = game.settings.wrapFilter;
            game.GraphicsDevice.Textures[0] = null;
            game.GraphicsDevice.Textures[1] = null;

            // kirajzolas
            Matrix[] transforms = new Matrix[mainModel.Bones.Count];
            mainModel.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in mainModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    Matrix world =  transforms[mesh.ParentBone.Index] *
                                    this.contructRotationMatrix() *
                                    Matrix.CreateTranslation(vPosition);

                    effect.Parameters["eyePosition"].SetValue(eye);
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["LightPosition"].SetValue(light.lightPosition);
                    effect.Parameters["LightColor"].SetValue(light.lightColor);
                    effect.Parameters["AmbientLightColor"].SetValue
                        (light.ambientLightColor);
                }
                mesh.Draw();
            }
        }

        public override bool proximityTest(CollidableSpaceObject other)
        {
            return this.getProximityBox(1.25f).Intersects(other.getBoundingBox());
        }

        /// <summary>
        /// Ray-triangle collision test
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool collisionTest(CollidableSpaceObject other)
        {
            if (this.getBoundingBox().Intersects(other.getBoundingBox()))
            {
                // Amennyiben a bounding box ütközik
                Matrix world = this.contructRotationMatrix() *
                               Matrix.CreateTranslation(vPosition);
                // Megvizsgáljuk a háromszögenkénti ütközést a másik objektum elmozdulásának az irányított szakaszával
                return raysegmentModelCollision(collosionModel, world, other.getRay());
            }
            else
            {
                return false;
            }
        }

        public override void damage(int damageToObject)
        {
            this.life -= damageToObject;
        }

        public override int damageCausedIfCollide()
        {
            return 1200;
        }
    }
}
