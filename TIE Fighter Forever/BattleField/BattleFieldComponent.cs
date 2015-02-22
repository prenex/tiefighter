using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TIE_Fighter_Forever.BattleField.BigShips;
using TIE_Fighter_Forever.BattleField.SmallShips;
using TIE_Fighter_Forever.BattleField.OtherObjects;
using TIE_Fighter_Forever.GameComponents.ParticleSystem;

namespace TIE_Fighter_Forever.BattleField
{
    class BattleFieldComponent : DrawableGameComponent
    {
        TIEGame game;
        ParticleSystem ps;
        int maxBigShips;
        int maxSmallShips;
        int maxLasers;
        int nextBigShipIndex;
        int nextSmallShipIndex;
        int nextLaserIndex;
        BigShip[] bigShips;
        SmallShip[] smallShips;
        Laser[] lasers;
        PlayerShip playerCamera;
        Light light;
        int score;

        /// <summary>
        /// A csatatér kirajzolható elemei által használt projekciós mátrix
        /// </summary>
        public Matrix projection;

        public int getScore()
        {
            return score;
        }

        public BattleFieldComponent(TIEGame game, int maxBigShips, int maxSmallShips, int maxLasers, ParticleSystem ps) : base(game)
        {
            this.game = game;
            this.maxBigShips = maxBigShips;
            this.maxSmallShips = maxSmallShips;
            this.maxLasers = maxLasers;
            this.ps = ps;

            bigShips = new BigShip[maxBigShips];
            smallShips = new SmallShip[maxSmallShips];
            lasers = new Laser[maxLasers];
            nextBigShipIndex = 0;
            nextSmallShipIndex = 0;
            nextLaserIndex = 0;
            score = 0;
        }

        public void addLight(Light l)
        {
            light = l;
        }

        public void addBigShip(BigShip ship)
        {
            bigShips[nextBigShipIndex] = ship;
            bigShips[nextBigShipIndex].projection = this.projection;
            nextBigShipIndex = (nextBigShipIndex + 1) % maxBigShips;
        }

        public void addSmallShip(SmallShip ship)
        {
            bool l = false;
            for (int i = 0; i < maxSmallShips; ++i)
            {
                if (smallShips[nextSmallShipIndex] == null)
                {
                    l = true;
                    break;
                }
                else if (smallShips[nextSmallShipIndex].life < 0)
                {
                    l = true;
                    break;
                }
                nextSmallShipIndex = (nextSmallShipIndex + 1) % maxSmallShips;
            }
            if (l)
            {
                smallShips[nextSmallShipIndex] = ship;
                smallShips[nextSmallShipIndex].projection = this.projection;
            }
        }

        public void addLaser(Laser laser)
        {
            lasers[nextLaserIndex] = laser;
            lasers[nextLaserIndex].projection = this.projection;
            nextLaserIndex = (nextLaserIndex + 1) % maxLasers;
        }

        public void addPlayer(PlayerShip player)
        {
            playerCamera = player;
        }

        /// <summary>
        ///  A csatamező kirajzolása
        /// </summary>
        /// <param name="gameTime">játékidő</param>
        public override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < maxBigShips; ++i)
            {
                if (bigShips[i] != null)
                {
                    if (bigShips[i].life >= 0)
                    {
                        bigShips[i].draw(playerCamera.contructViewMatrix(), playerCamera.position, light);
                    }
                }
            }
            for (int i = 0; i < maxSmallShips; ++i)
            {
                if (smallShips[i] != null)
                {
                    if (smallShips[i].life >= 0)
                    {
                        smallShips[i].draw(playerCamera.contructViewMatrix(), playerCamera.position, light);
                    }
                }
            }
            foreach (Laser l in lasers)
            {
                if (l != null)
                {
                    if(l.life >= 0)
                        l.draw(playerCamera.contructViewMatrix(), playerCamera.position, light);
                }
            }
        }

        /// <summary>
        /// A csatamező frissítése és ütközéseinek az ellenőrzése
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // lézerek mozgatása
            for (int i = 0; i < maxLasers; ++i)
            {
                if(lasers[i] != null)
                    if(lasers[i].life > 0)
                    lasers[i].goForward(lasers[i].speed);
            }

            // Nagyhajók ütközése lézerekkel, kishajókkal és a játékossal
            for (int i = 0; i < maxBigShips; ++i)
            {
                if (bigShips[i] != null)
                {
                    // Kishajók
                    for (int j = 0; j < maxSmallShips; ++j)
                    {
                        if (smallShips[j] != null)
                        {
                            if((bigShips[i].life >= 0) && smallShips[j].life >= 0)
                            {
                                if (bigShips[i].collisionTest(smallShips[j]))
                                {
                                    // COLLISION: BIG vs. SMALL
                                    int d1 = bigShips[i].damageCausedIfCollide();
                                    int d2 = smallShips[j].damageCausedIfCollide();
                                    smallShips[j].damage(d1);
                                    bigShips[i].damage(d2);
                                    // Robbantás:
                                    if (smallShips[j].life < 0)
                                    {
                                        ps.createExplosion(gameTime, smallShips[j].position, Matrix.Identity, 40.0f, 10.0f, 75, 3000, 0.5f, smallShips[j].getRay().Direction * 10.0f);
                                        score += 1;
                                    }
                                    if (bigShips[i].life < 0)
                                    {
                                        ps.createExplosion(gameTime, bigShips[i].position, Matrix.Identity, 1000.0f, 100.0f, 75, 5000, 0.5f, Vector3.Zero);
                                        ps.createExplosion(gameTime, bigShips[i].position, Matrix.Identity, 40.0f, 200.0f, 50, 7500, 0.5f, Vector3.Zero);
                                        score += 15;
                                    }
                                }
                            }
                        }
                    }
                    
                    // Lérerek
                    for (int j = 0; j < maxLasers; ++j)
                    {
                        if (lasers[j] != null)
                        {
                            if ((bigShips[i].life >= 0) && lasers[j].life >= 0)
                            {
                                if (bigShips[i].collisionTest(lasers[j]))
                                {
                                    // COLLISION: BIG vs. LASER
                                    int d = bigShips[i].damageCausedIfCollide();
                                    bigShips[i].damage(lasers[j].damageCausedIfCollide());
                                    lasers[j].damage(d);
                                    // Robbantás:
                                    if(lasers[j].life < 0)
                                        ps.createExplosion(gameTime, lasers[j].position, Matrix.Identity, 5.0f, 2.0f, 50, 1000, 0.5f, -1.5f * lasers[j].getForwardVector());
                                    if (bigShips[i].life < 0)
                                    {
                                        ps.createExplosion(gameTime, bigShips[i].position, Matrix.Identity, 1000.0f, 100.0f, 75, 5000, 0.5f, Vector3.Zero);
                                        ps.createExplosion(gameTime, bigShips[i].position, Matrix.Identity, 40.0f, 200.0f, 50, 7500, 0.5f, Vector3.Zero);
                                        score += 15;
                                    }
                                }
                            }
                        }
                    }

                    // Játékos
                    if ((bigShips[i].life > 0) && (playerCamera.life > 0))
                    {
                        if (bigShips[i].collisionTest(playerCamera))
                        {
                            // COLLISION: BIG vs. PLAYER
                            int d1 = playerCamera.damageCausedIfCollide();
                            int d2 = bigShips[i].damageCausedIfCollide();
                            bigShips[i].damage(d1);
                            playerCamera.damage(d2);
                            // Robbantás
                            if (bigShips[i].life < 0)
                            {
                                ps.createExplosion(gameTime, bigShips[i].position, Matrix.Identity, 1000.0f, 100.0f, 75, 5000, 0.5f, Vector3.Zero);
                                ps.createExplosion(gameTime, bigShips[i].position, Matrix.Identity, 40.0f, 200.0f, 50, 7500, 0.5f, Vector3.Zero);
                                score += 15;
                            }
                            if (playerCamera.life < 0)
                            {
                                ps.createExplosion(gameTime, playerCamera.position, Matrix.Identity, 40.0f, 10.0f, 75, 3000, 0.5f, Vector3.Zero);
                            }
                        }
                    }
                }
            }

            // Kishajók ütközése kishajókkal és a játékossal
            for (int i = 0; i < maxSmallShips; ++i)
            {
                // Kishajók
                if (smallShips[i] != null)
                {
                    for (int j = 0; j < maxSmallShips; ++j)
                    {
                        if (smallShips[j] != null)
                        {
                            if (i != j)
                            {
                                if ((smallShips[i].life) >= 0 && (smallShips[j].life >= 0))
                                {
                                    if (smallShips[i].collisionTest(smallShips[j]))
                                    {
                                        // COLLISION: SMALL vs. SMALL
                                        int d1 = smallShips[j].damageCausedIfCollide();
                                        int d2 = smallShips[i].damageCausedIfCollide();
                                        smallShips[i].damage(d1);
                                        smallShips[j].damage(d2);
                                        // Robbantás
                                        if (smallShips[i].life < 0)
                                        {
                                            ps.createExplosion(gameTime, smallShips[i].position, Matrix.Identity, 40.0f, 10.0f, 75, 3000, 0.5f, smallShips[i].getRay().Direction * 7.5f);
                                            score += 1;
                                        }
                                        if (smallShips[j].life < 0)
                                        {
                                            ps.createExplosion(gameTime, smallShips[j].position, Matrix.Identity, 40.0f, 10.0f, 75, 3000, 0.5f, smallShips[j].getRay().Direction * 7.5f);
                                            score += 1;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // COLLISION: PLAYER vs. SMALL
                    if ((smallShips[i].life >= 0) && (playerCamera.life >= 0))
                    {
                        if (playerCamera.collisionTest(smallShips[i]))
                        {
                            int d1 = playerCamera.damageCausedIfCollide();
                            int d2 = smallShips[i].damageCausedIfCollide();
                            smallShips[i].damage(d1);
                            playerCamera.damage(d2);
                        }
                        // Robbantás
                        if (smallShips[i].life < 0)
                        {
                            ps.createExplosion(gameTime, smallShips[i].position, Matrix.Identity, 40.0f, 10.0f, 75, 3000, 0.5f, smallShips[i].getRay().Direction * 7.5f);
                            score += 1;
                        }
                        if(playerCamera.life < 0)
                        {
                            ps.createExplosion(gameTime, playerCamera.position, Matrix.Identity, 40.0f, 10.0f, 75, 3000, 0.5f, Vector3.Zero);
                        }
                    }
                }
            }

            // Lézerek ütközése kishajókkal és a játékossal
            for (int i = 0; i < maxLasers; ++i)
            {
                if (lasers[i] != null)
                {
                    // Kishajók
                    for (int j = 0; j < maxSmallShips; ++j)
                    {
                        if (smallShips[j] != null)
                        {
                            if((lasers[i].life >= 0) && (smallShips[j].life >= 0))
                            {
                                if (lasers[i].collisionTest(smallShips[j]))
                                {
                                    // COLLISON: LASER vs. SMALL
                                    int d = smallShips[j].damageCausedIfCollide();
                                    smallShips[j].damage(lasers[i].damageCausedIfCollide());
                                    lasers[i].damage(d);
                                    // Robbantás
                                    if (smallShips[j].life < 0)
                                    {
                                        ps.createExplosion(gameTime, smallShips[j].position, Matrix.Identity, 40.0f, 10.0f, 75, 3000, 0.5f, smallShips[j].getRay().Direction * 7.5f);
                                        score += 1;
                                    }
                                    if (lasers[i].life < 0)
                                        ps.createExplosion(gameTime, lasers[i].position, Matrix.Identity, 5.0f, 2.0f, 50, 1000, 0.5f, -1.5f * lasers[i].getForwardVector());
                                }
                            }
                        }
                    }
                    // Játékos
                    if((lasers[i].life >= 0) && (playerCamera.life >= 0))
                    {
                        if (lasers[i].collisionTest(playerCamera))
                        {
                            // COLLISION: LASER vs. PLAYER
                            int d = playerCamera.damageCausedIfCollide();
                            playerCamera.damage(lasers[i].damageCausedIfCollide());
                            lasers[i].damage(d);
                            // Robbantás
                            if (lasers[i].life < 0)
                            {
                                ps.createExplosion(gameTime, lasers[i].position, Matrix.Identity, 5.0f, 2.0f, 50, 1000, 0.5f, -1.5f * lasers[i].getForwardVector());
                            }
                            if (playerCamera.life < 0)
                            {
                                ps.createExplosion(gameTime, playerCamera.position, Matrix.Identity, 40.0f, 10.0f, 75, 3000, 0.5f, Vector3.Zero);
                            }
                        }
                    }
                }
            }
        }

        internal PlayerShip getPlayerShip()
        {
            return playerCamera;
        }
    }
}
