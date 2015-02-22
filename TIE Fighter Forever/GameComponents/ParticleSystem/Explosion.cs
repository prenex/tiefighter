using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TIE_Fighter_Forever.GameComponents.ParticleSystem
{
    /// <summary>
    /// Robbanások létrehozására alkalmas particle emitter
    /// </summary>
    class Explosion
    {
        Effect effect;
        Texture2D texture;
        ParticleSystem ps;
        int particleNum;
        Matrix world;
        float timeInMilliSeconds;
        VertexBuffer vertices;
        float lastTimeInMiliseconds;
        float alpha;
        float scale;
        Vector3 force;

        /// <summary>
        /// A robbanás már lezajlott akkor ez false, ha még tart, akkor true
        /// </summary>
        public bool active;

        /// <summary>
        /// Robbanás objektum létrehozása
        /// </summary>
        /// <param name="texture">Használt részecsketextúra</param>
        /// <param name="effect">Használt részecskeshader.</param>
        /// <param name="ps">Szülő részecskerendszer</param>
        public Explosion(ParticleSystem ps, Texture2D texture, Effect effect)
        {
            this.texture = texture;
            this.effect = effect;
            this.ps = ps;
            this.active = false;
        }

        /// <summary>
        /// Robbanás inicializálása
        /// </summary>
        /// <param name="gametime">Jelenlegi időpillanat</param>
        /// <param name="position">A robbanás kezdőpozíciója</param>
        /// <param name="world">World mátrix</param>
        /// <param name="size">Részecskék léptékelése ezen méret alapján</param>
        /// <param name="speed">Részecskék maximum sebessége</param>
        /// <param name="explosionParticleNum">Részecskeszám([0..8000]-be kerül az érték, többet nem érdemes megadni)</param>
        /// <param name="explosionTimeInMilisecs">Robbanás időtartama milisecundumban</param>
        /// <param name="alpha">A keletkező részecskék által beírt alpha érték(additive blending van, de ez pl. glowhoz vagy más shaderhez hasznos lehet)</param>
        public void InitExplosion(GameTime gametime,Vector3 position, Matrix world, float size, float speed, int explosionParticleNum, int explosionTimeInMilisecs, float alpha, Vector3 force)
        {
            this.timeInMilliSeconds = (float)gametime.TotalGameTime.TotalMilliseconds;
            this.lastTimeInMiliseconds = this.timeInMilliSeconds + explosionTimeInMilisecs;
            this.world = world;
            this.scale = size;
            this.particleNum = explosionParticleNum;
            this.alpha = alpha;
            this.active = true;
            this.force = force;

            // Ha az objektum már használva volt, elengedhetjük a korábbi vertex buffert
            if (vertices != null)
                vertices.Dispose();

            // Vertex Buffer feltöltése a robbanás adataival:
            ExplosionVertex[] data = new ExplosionVertex[6 * this.particleNum];
            vertices = new VertexBuffer(ps.gd, typeof(ExplosionVertex), 6 * this.particleNum, BufferUsage.WriteOnly);
            Random rand = new Random((int)gametime.TotalGameTime.Ticks);
            // Ezek arra jók, hogy a robbanás ne legyen teljesen gömb alakú, hanem legyen kicsit "elvarázsolva"
            float rx, ry, rz;
            rx = (float)rand.NextDouble(); ry = (float)rand.NextDouble(); rz = (float)rand.NextDouble();
            // Feltöltjük a vertexadatokat a részecskékhez
            for (int i = 0; i < this.particleNum; ++i)
            {
                // Néhány véletlen érték generálása részecskénként:
                // -1.0f és 1.0f közötti értékekkel generálunk random vektorokat, majd ezt felszorozzuk a robbanás sebességével.
                // Ezen kívül tárolunk egy véletlen számot is 0 és 1 között!
                Vector4 dmar = new Vector4(((Vector3.Normalize(new Vector3(((float)rand.NextDouble() - 0.5f) * rx, ((float)rand.NextDouble() - 0.5f) * ry, ((float)rand.NextDouble() - 0.5f) * rz ))) * speed), (float)rand.NextDouble());
                // A maximális kor paramétert növeljük egy véletlenszerű értékkel, ami maximum féltized másodpercet ad(lehet hogy paraméterezhetővé tesszük!)
                float maxAge = explosionTimeInMilisecs + (float)rand.NextDouble() * 50;
                // Bal felső vertex(lesz majd a texcoord alapján):
                // pozíció
                data[6 * i].position = new Vector3(position.X, position.Y, position.Z);
                // textúrapozíció, illetve idők
                data[6 * i].texAndData = new Vector4(1, 1, timeInMilliSeconds, maxAge);
                // Részecskedelta és véletlenszám
                data[6 * i].deltaMoveAndRand = dmar;

                // Jobb felső vertex(lesz majd a texcoord alapján):
                // pozíció
                data[6 * i + 1].position = new Vector3(position.X, position.Y, position.Z);
                // textúrapozíció, illetve idők
                data[6 * i + 1].texAndData = new Vector4(0, 0, timeInMilliSeconds, maxAge);
                // Részecskedelta és véletlenszám
                data[6 * i + 1].deltaMoveAndRand = dmar;

                // Jobb alsó vertex(lesz majd a texcoord alapján):
                // pozíció
                data[6 * i + 2].position = new Vector3(position.X, position.Y, position.Z);
                // textúrapozíció, illetve idők
                data[6 * i + 2].texAndData = new Vector4(1, 0, timeInMilliSeconds, maxAge);
                // Részecskedelta és véletlenszám
                data[6 * i + 2].deltaMoveAndRand = dmar;

                // Bal felső vertex(lesz majd a texcoord alapján):
                // pozíció
                data[6 * i + 3].position = new Vector3(position.X, position.Y, position.Z);
                // textúrapozíció, illetve idők
                data[6 * i + 3].texAndData = new Vector4(1, 1, timeInMilliSeconds, maxAge);
                // Részecskedelta és véletlenszám
                data[6 * i + 3].deltaMoveAndRand = dmar;

                // Jobb alsó vertex(lesz majd a texcoord alapján):
                // pozíció
                data[6 * i + 4].position = new Vector3(position.X, position.Y, position.Z);
                // textúrapozíció, illetve idők
                data[6 * i + 4].texAndData = new Vector4(0, 1, timeInMilliSeconds, maxAge);
                // Részecskedelta és véletlenszám
                data[6 * i + 4].deltaMoveAndRand = dmar;

                // Bal alsó vertex(lesz majd a texcoord alapján):
                // pozíció
                data[6 * i + 5].position = new Vector3(position.X, position.Y, position.Z);
                // textúrapozíció, illetve idők
                data[6 * i + 5].texAndData = new Vector4(0, 0, timeInMilliSeconds, maxAge);
                // Részecskedelta és véletlenszám
                data[6 * i + 5].deltaMoveAndRand = dmar;
            }
            vertices.SetData<ExplosionVertex>(data);
        }

        public void draw(GameTime gametime)
        {
            // Ez már nem ugyanaz mint a konstruktorban, közbe telik az idő!
            timeInMilliSeconds = (float)gametime.TotalGameTime.TotalMilliseconds;

            // Ha az idő már annyi, hogy a robbanás véget kellett, hogy érjen,
            // akkor a draw már nem csinál semmit és beáll false-ra az active.
            if (timeInMilliSeconds > lastTimeInMiliseconds)
            {
                active = false;
                return;
            }

            // Beállítjuk a shader paramétereket
            // Mátrixok
            effect.Parameters["world"].SetValue(world);
            effect.Parameters["view"].SetValue(ps.view);
            effect.Parameters["projection"].SetValue(ps.projection);
            // Kamera pozíció
            effect.Parameters["camPos"].SetValue(ps.cameraPos);
            effect.Parameters["camUp"].SetValue(ps.cameraUp);
            // idő beállítása
            effect.Parameters["time"].SetValue(this.timeInMilliSeconds);
            // a shader által beírt alfa érték
            effect.Parameters["alpha"].SetValue(this.alpha);
            // a robbanás részecskéinek méretezését segítendő
            effect.Parameters["scale"].SetValue(this.scale);
            effect.Parameters["force"].SetValue(force);

            effect.Parameters["baseTexture"].SetValue(texture);

            // beállítjuk a vertexbuffert a graphicsdevice-on
            ps.gd.SetVertexBuffer(vertices);
            //ps.gd.Indices = null;
            // Beállítjuk a shadert aktívnak
            effect.Techniques[0].Passes[0].Apply();

            // és kirajzoljuk a robbanást
            ps.gd.DrawPrimitives(PrimitiveType.TriangleList, 0, this.particleNum * 2);
        }

        /// <summary>
        /// Elengedi a vertex buffert
        /// </summary>
        public void Dispose()
        {
            vertices.Dispose();
        }
    }
}
