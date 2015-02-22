using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TIE_Fighter_Forever.GameComponents.ParticleSystem
{
    /// <summary>
    /// Részecskék kezelésére használható komponens
    /// </summary>
    class ParticleSystem : DrawableGameComponent
    {
        public TIEGame game;
        public GraphicsDevice gd;
        int maxExplosions;
        int nextExplosion;
        Explosion[] explosions;
        Matrix mProjection, mView;

        Effect explosionEffect;
        int texnum;
        Texture2D[] explosionTextures;

        public Matrix projection
        {
            get { return mProjection; }
            set { mProjection = value; }
        }

        public Matrix view
        {
            get { return mView; }
            set { mView = value; }
        }

        public Vector3 cameraUp;
        public Vector3 cameraPos;

        /// <summary>
        /// Létrehoz egy részecskerendszer komponenst
        /// </summary>
        /// <param name="game">Fő játék objektum</param>
        /// <param name="numOfEmitters">Emitterek maximális száma</param>
        public ParticleSystem(TIEGame game, int maxExplosions)
            : base(game)
        {
            this.view = Matrix.Identity; this.projection = Matrix.Identity;
            this.game = game;
            this.maxExplosions = maxExplosions;
            explosions = new Explosion[maxExplosions];
            nextExplosion = 0;

            IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)
            game.Services.GetService(typeof(IGraphicsDeviceService));
            gd = graphicsService.GraphicsDevice;
        }

        /// <summary>
        /// Inicializálja a részecskerendszert és betölti a contenteket is
        /// </summary>
        public override void Initialize()
        {
            texnum = 2;
            explosionTextures = new Texture2D[texnum];
            explosionTextures[0] = game.Content.Load<Texture2D>("Particles\\explosion");
            explosionTextures[1] = game.Content.Load<Texture2D>("Particles\\explosion2");
            explosionEffect = game.Content.Load<Effect>("Shaders\\ParticleSystem\\ExplosionEffect");
            base.Initialize();
        }

        /// <summary>
        /// Létrehoz egy robbanást
        /// </summary>
        /// <param name="gametime">Jelenlegi időpillanat</param>
        /// <param name="position">A robbanás kezdőpozíciója</param>
        /// <param name="world">World mátrix</param>
        /// <param name="size">Részecskék léptékelése ezen méret alapján</param>
        /// <param name="speed">Részecskék maximum sebessége</param>
        /// <param name="explosionParticleNum">Részecskeszám([0..8000]-be kerül az érték, többet nem érdemes megadni)</param>
        /// <param name="explosionTimeInMilisecs">Robbanás időtartama milisecundumban</param>
        /// <param name="alpha">A keletkező részecskék által beírt alpha érték(additive blending van, de ez pl. glowhoz vagy más shaderhez hasznos lehet)</param>
        public void createExplosion(GameTime gametime, Vector3 position, Matrix world, float size, float speed, int explosionParticleNum, int explosionTimeInMilisecs, float alpha, Vector3 force)
        {
            if (explosions[nextExplosion] == null)
                explosions[nextExplosion] = new Explosion(this, explosionTextures[nextExplosion % texnum], explosionEffect);
            explosions[nextExplosion].InitExplosion(gametime, position, world, size, speed, explosionParticleNum, explosionTimeInMilisecs, alpha, force);
            nextExplosion = (nextExplosion + 1) % maxExplosions;
        }

        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < maxExplosions; ++i)
                if (explosions[i] != null)
                    explosions[i].draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
