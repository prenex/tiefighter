using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TIE_Fighter_Forever.Screens.Menu;
using TIE_Fighter_Forever.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using TIE_Fighter_Forever.GameComponents.LaserGlow;
using TIE_Fighter_Forever.GameComponents.SkyBox;
using TIE_Fighter_Forever.GameComponents.ParticleSystem;
using TIE_Fighter_Forever.GameComponents.TextMenus;
using TIE_Fighter_Forever.BattleField;
using TIE_Fighter_Forever.BattleField.OtherObjects;
using TIE_Fighter_Forever.BattleField.BigShips;
using TIE_Fighter_Forever.BattleField.SmallShips;
using TIE_Fighter_Forever.AI;
using TIE_Fighter_Forever.Screens.Battle.Spawning;

namespace TIE_Fighter_Forever.Screens.Battle
{
    class BattleScreen : DrawableGameComponent, ITextMenuNotifiable
    {
        #region Fields
        #region Communication
        TIEGame game;
        IKeyboardLikeManager keyLikeMan;
        IMouseLikeManager mouseLikeMan;
        DrawableGameComponent parentScreen;
        #endregion
        #region Menu
        SpriteFont menuFont;
        ITextMenu mainMenu;
        bool menuOn;
        #endregion
        #region Graphics
        SpriteBatch spriteBatch;
        GraphicsDeviceManager graphics;
        GlowComponent glower;
        SkyBox sb;
        float aspectRatio;
        Matrix projection;
        Matrix view;
        Texture2D hud;
        Texture2D shield;
        #endregion
        #region BattleField
        BattleFieldComponent battlefield;
        Spawner spawner;
        #endregion
        #region Light
        Light light;
        #endregion
        #region Camera
        PlayerShip camera;
        #endregion
        #region Camera_control
        float speed;
        private float rotationZ = 0.0f;
        private float rotationX = 0.0f;
        #endregion
        #region ParticleSystem
        ParticleSystem ps;
        BlendState realAdditiveBlend;
        #endregion
        #region AI
        Strategist strategist;
        #endregion
        #region BulletControl
        double lastFired;
        #endregion
        Random rand = new Random();
        #endregion
        #region Initialize
        public BattleScreen(TIEGame game, DrawableGameComponent parentScreen)
            : base(game)
        {
            this.lastFired = 0;
            this.parentScreen = parentScreen;
            this.game = game;
            graphics = game.graphics;
            keyLikeMan = (IKeyboardLikeManager)game.Services.GetService(typeof(IKeyboardLikeManager));
            mouseLikeMan = (IMouseLikeManager)game.Services.GetService(typeof(IMouseLikeManager));
            menuOn = false;
            if (game.settings.laserGlow)
                glower = new GlowComponent(game);
            rand = new Random();
        }

        public override void Initialize()
        {
            if (game.settings.laserGlow)
                glower.Initialize();
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            projection =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                aspectRatio, 0.05f, 10000.0f);

            // Fény beállítása
            light = new Light();
            light.lightColor = new Vector4(1, 1, 1, 1);
            light.ambientLightColor = new Vector4(.2f, .2f, .2f, 1);
            light.lightPosition = new Vector3(100000, 100000, 100000);

            // Skybox init
            sb = new SkyBox(game, "Space", new Vector3(1000, 1000, 1000));
            sb.Initialize();
            sb.projection = projection;

            // Additív blending BlendState létrehozása
            realAdditiveBlend = new BlendState();
            realAdditiveBlend.ColorBlendFunction = BlendFunction.Add;
            realAdditiveBlend.AlphaBlendFunction = BlendFunction.Add;
            realAdditiveBlend.ColorDestinationBlend = Blend.One;
            realAdditiveBlend.AlphaDestinationBlend = Blend.One;
            realAdditiveBlend.ColorSourceBlend = Blend.One;
            realAdditiveBlend.AlphaSourceBlend = Blend.One;
            realAdditiveBlend.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;

            // FONTOS MEGJEGYZÉS! A KÖVETKEZŐ SOR XBOXON KICSIT MÁSHOGY _NÉZHET_ ESETLEG KI, MERT:
            // http://social.msdn.microsoft.com/Forums/en/xnaframework/thread/5b77cd78-e418-448c-8907-71af608a9d64
            realAdditiveBlend.ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;

            // Particlesystem init
            ps = new ParticleSystem(game, 50);
            ps.Initialize();
            ps.projection = projection;

            // Kamera objektum init
            camera = new PlayerShip(new Vector3(0, 10, -250.0f), 0, 0, 0, 7.5f);

            const int maxSmallShipNum = 30;
            const int maxBigShipNum = 7;
            const int maxLaserNum = 65;

            battlefield = new BattleFieldComponent(game, maxBigShipNum, maxSmallShipNum, maxLaserNum, ps);
            battlefield.addPlayer(camera);
            battlefield.projection = projection;

            strategist = new BruteforceStrategist(battlefield, maxSmallShipNum);
            spawner = new Spawner(game, battlefield, strategist);
            strategist.spawnerObj = spawner;

            for (int i = 0; i < maxSmallShipNum; ++i)
            {
                float sX, sY, sZ;
                sX = (float)(rand.NextDouble() - 0.5) * 2000;
                sY = (float)(rand.NextDouble() - 0.5) * 2000;
                sZ = (float)(rand.NextDouble() - 0.5) * 2000;
                spawner.spawnSmallShip(new TieIn(game, new Vector3(sX, sY, sZ), Quaternion.CreateFromYawPitchRoll(0, (float)Math.PI / 2.0f, 0)), camera);
            }

            List<BoundingBox> bbs = new List<BoundingBox>();
            bbs.Add(new BoundingBox(new Vector3(-100,-100,-100), new Vector3(100, 100, 100)));

            for (int i = 0; i < maxBigShipNum+300; ++i)
            {
                float sX, sY, sZ;
                sX = (float)(rand.NextDouble() - 0.5) * 4000;
                sY = (float)(rand.NextDouble() - 0.5) * 4000;
                sZ = (float)(rand.NextDouble() - 0.5) * 4000;
                float rX, rY, rZ;
                rX = (float)(rand.NextDouble() - 0.5);
                rY = (float)(rand.NextDouble() - 0.5);
                rZ = (float)(rand.NextDouble() - 0.5);
                Platform01 plt = new Platform01(game, new Vector3(sX, sY, sZ), Quaternion.CreateFromYawPitchRoll(rX, rY, rZ));
                bool nonColliding = true;
                foreach(BoundingBox bb in bbs)
                {
                    if(plt.getBoundingBox().Intersects(bb))
                        nonColliding = false;
                }
                if(nonColliding)
                    battlefield.addBigShip(plt);
            }
            battlefield.addLight(light);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            menuFont = content.Load<SpriteFont>("MenuContent\\MenuFont1");

            mainMenu = new SimpleMenu(this,
                                        keyLikeMan,
                                        new Vector2(GraphicsDevice.Viewport.X + GraphicsDevice.Viewport.Width / 2,
                                                    GraphicsDevice.Viewport.Y + GraphicsDevice.Viewport.Height / 2.5f),
                                        menuFont, Color.Yellow, SimpleMenuAlign.CenterAligned, 10.0f);
            mainMenu.addMenuItem(new MenuItem("Resume game"));
            mainMenu.addMenuItem(new MenuItem("Exit battle"));

            hud = content.Load<Texture2D>("BattleContent\\hud");
            shield = content.Load<Texture2D>("BattleContent\\shield");
        }

        protected override void UnloadContent()
        {
        }
        #endregion
        #region Update and Draw
        public override void Update(GameTime gameTime)
        {
            if (!menuOn)
            {
                // Minden 30. másodpercben megpróbálunk 20 hajót hozzáadni a rendszerhez!
                if (((int)gameTime.TotalGameTime.TotalSeconds) % 30 == 0)
                {
                    for (int i = 0; i < 20; ++i)
                    {
                        float sX, sY, sZ;
                        sX = (float)(rand.NextDouble() - 0.5) * 2000;
                        sY = (float)(rand.NextDouble() - 0.5) * 2000;
                        sZ = (float)(rand.NextDouble() - 0.5) * 2000;
                        spawner.spawnSmallShip(new TieIn(game, new Vector3(sX, sY, sZ), Quaternion.CreateFromYawPitchRoll(0, (float)Math.PI / 2.0f, 0)), camera);
                    }
                }

            }
            HandleInput(gameTime);
            if (!menuOn)
            {
                camera.rotate(-rotationX, 0, rotationZ);
                camera.goForward(speed);

                if (camera.life < 0)
                    exitScreen();

                strategist.updateState();
                strategist.updateGovernedSpaceObjects(gameTime);
                battlefield.Update(gameTime);
            }
            base.Update(gameTime);
        }

        private void HandleInput(GameTime gt)
        {
            if (menuOn)
                mainMenu.updateMenuItems();
            else
            {
                if (keyLikeMan.exitPressed())
                    menuOn = true;

                rotationZ = mouseLikeMan.getNormalizedX();
                rotationX = mouseLikeMan.getNormalizedY();
                //speed = 0.0005f * -mouseLikeMan.getZ();
                speed = 1.25f;

                if (mouseLikeMan.leftButton())
                {
                    if (gt.TotalGameTime.TotalMilliseconds > lastFired + 1000)
                    {
                        spawner.spawnQuadLaser(camera);
                        lastFired = gt.TotalGameTime.TotalMilliseconds;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Jelezzük a laserglow komponensünknek, hogy rajzolás kezdődik és
            if (game.settings.laserGlow)
                glower.PrepareDraw();

            // Backface Culling bekapcsolása:
            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            // 3D rajzolások innentől
            view = camera.contructViewMatrix();

            GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            sb.view = view;
            sb.Draw(gameTime);
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Csatamező
            battlefield.Draw(gameTime);

            // Particlesystem:
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            graphics.GraphicsDevice.BlendState = realAdditiveBlend;
            ps.view = view;
            ps.cameraPos = camera.position;
            ps.cameraUp = camera.getUpVector();
            ps.Draw(gameTime);

            // Glow effect
            if (game.settings.laserGlow)
                glower.Draw(gameTime);

            // 2D rajzolások innentől(menü és hud)
            drawHUD();
            if (menuOn)
                mainMenu.drawMenuItems(255, spriteBatch);
            // bázisosztály rajzolás
            base.Draw(gameTime);
        }

        /// <summary>
        /// Ezzel rajzoljuk ki a szálkeresztet
        /// </summary>
        void drawHUD()
        {
            // Életerő
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            float relHp = (float)camera.life / (float)PlayerShip.maxHealth;
            spriteBatch.Draw(shield, new Rectangle(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y, (int)((GraphicsDevice.Viewport.Width / 4) * relHp), GraphicsDevice.Viewport.Height / 20), new Color(255, 255, 255));
            spriteBatch.End();
            // Szálkereszt
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            spriteBatch.Draw(hud, new Rectangle(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color(255, 255, 255));
            spriteBatch.End();
        }

        public void textMenuCallback(ITextMenu menu, MenuItem selectedMenuItem)
        {
            String selectedMenuText = selectedMenuItem.text();
            if (selectedMenuText.Equals("Resume game"))
                menuOn = false;
            if (selectedMenuText.Equals("Exit battle"))
            {
                exitScreen();
            }
        }

        public void exitScreen()
        {
            int score = battlefield.getScore();
            game.Components.Remove(this);
            //parentScreen.Initialize();
            GameOverScreen gos = new GameOverScreen(game, parentScreen, score);
            gos.Initialize();
            game.Components.Add(gos);
        }
        #endregion
    }
}
