using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TIE_Fighter_Forever.BattleField.OtherObjects;

namespace TIE_Fighter_Forever.BattleField.SmallShips
{
    class TieIn : SmallShip
    {
        Model[] models;
        /// <summary>
        /// Létrehoz egy TIE Interceptort
        /// </summary>
        /// <param name="game"></param>
        /// <param name="postition"></param>
        /// <param name="rotation"></param>
        public TieIn(TIEGame game, Vector3 position, Quaternion rotation)
        {
            // élet:
            this.life = 20; // 20 RU: TIE Interceptor

            // Paraméterek mentése
            this.game = game;
            this.vPosition = position;
            this.oldPosition = position - new Vector3(0, 0, 0.1f);
            this.qRotation = rotation;

            // Modellek betöltése
            models = new Model[5];
            models[0] = game.Content.Load<Model>("BattleContent\\TIE_In\\TieIntCockpit");
            models[1] = game.Content.Load<Model>("BattleContent\\TIE_In\\TieIntLeftArm");
            models[2] = game.Content.Load<Model>("BattleContent\\TIE_In\\TieIntLeftWing");
            models[3] = game.Content.Load<Model>("BattleContent\\TIE_In\\TieIntRightArm");
            models[4] = game.Content.Load<Model>("BattleContent\\TIE_In\\TieIntRightWing");

            // Előállítjuk az egész modellt befoglaló gömböt
            // ebből a gömbből állítja elő az ősosztály a boundingBox property
            // értékét. Belső reprezentációra azért használunk gömböt, mert azt 
            // könnyebb egy updata során eltolni, mint egy boxot(az eltolást is az
            // ősosztálybéli kódban végezzük)
            bSphere = new BoundingSphere();
            foreach (Model m in models)
            {
                foreach (ModelMesh mesh in m.Meshes)
                {
                    bSphere = BoundingSphere.CreateMerged(bSphere, mesh.BoundingSphere);
                }
            }
        }

        public override void draw(Matrix view, Vector3 eye, Light light)
        {
            foreach (Model m in models)
            {
                game.GraphicsDevice.SamplerStates[0] = game.settings.wrapFilter;
                game.GraphicsDevice.SamplerStates[1] = game.settings.wrapFilter;
                game.GraphicsDevice.Textures[0] = null;
                game.GraphicsDevice.Textures[1] = null;

                Matrix[] transforms = new Matrix[m.Bones.Count];
                m.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in m.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        Matrix world =  transforms[mesh.ParentBone.Index] *
                                        Matrix.CreateFromQuaternion(qRotation) *
                                        Matrix.CreateTranslation(vPosition);

                        effect.Parameters["Glow"].SetValue(1.0f);
                        effect.Parameters["eyePosition"].SetValue(eye);
                        effect.Parameters["World"].SetValue(world);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        effect.Parameters["LightDirection"].SetValue(light.lightPosition);
                        effect.Parameters["LightColor"].SetValue(light.lightColor);
                        effect.Parameters["AmbientColor"].SetValue(new Vector3(light.ambientLightColor.X, light.ambientLightColor.Y, light.ambientLightColor.Z));
                    }
                    mesh.Draw();
                }
            }
        }

        public override bool proximityTest(CollidableSpaceObject other)
        {
            return this.getProximityBox(1.5f).Intersects(other.getBoundingBox());
        }

        public override bool collisionTest(CollidableSpaceObject other)
        {
            return this.getBoundingBox().Intersects(other.getBoundingBox());
        }

        public override void damage(int damageToObject)
        {
            this.life -= damageToObject;
        }

        public override int damageCausedIfCollide()
        {
            return 20;
        }
    }
}
