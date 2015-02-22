using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TIE_Fighter_Forever.Input;
using TIE_Fighter_Forever.Screens.Battle;
using TIE_Fighter_Forever.GameComponents.TextMenus;

namespace TIE_Fighter_Forever.Screens.Menu
{
    public class GameOverScreen : DrawableGameComponent, ITextMenuNotifiable
    {
        #region Fields
        // Általános mezők
        TIEGame game;
        IKeyboardLikeManager keyLikeMan;
        SpriteBatch spriteBatch;

        // Háttér kirajzolásához
        Texture2D background;

        // A főmenü képernyője
        DrawableGameComponent parentScreen;

        // Menük
        ITextMenu endGameMenu2;
        ITextMenu endGameMenu1;
        // Menük spriteja
        SpriteFont menuFont;

        // Játékos pontszáma
        int score;

        // Kiválasztott menüpont léptékelésének szélsőértékei
        const float minScale = 1.0f;
        const float maxScale = 1.25f;

        // Induláskor és továbblépéskor történő elsötétöléshez
        int fadeIn;
        int fadeSpeed;

        // Ezek arra kellenek, hogy eldöntsük mi történjen elsötétülés után
        bool goMainMenu;
        #endregion
        #region Initializaton
        public GameOverScreen(TIEGame game, DrawableGameComponent parentScreen, int score) : base(game)
        {
            this.game = game;
            this.parentScreen = parentScreen;
            this.score = score;
            keyLikeMan = (IKeyboardLikeManager)game.Services.GetService(typeof(IKeyboardLikeManager));
        }

        /// <summary>
        /// Főmenüképernyőt inicializáló fv.
        /// </summary>
        public override void Initialize()
        {
            goMainMenu = false;

            fadeSpeed = 3;
            fadeIn = fadeSpeed;
            base.Initialize();
        }

        /// <summary>
        /// Content betöltő fv.
        /// </summary>
        protected override void LoadContent()
        {
            ContentManager content = Game.Content;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = content.Load<Texture2D>("GameOverContent\\GameOver");
            menuFont = content.Load<SpriteFont>("MenuContent\\MenuFont1");

            endGameMenu1 = new SimpleMenu(this,
                            keyLikeMan,
                            new Vector2(GraphicsDevice.Viewport.X + GraphicsDevice.Viewport.Width / 2,
                                        GraphicsDevice.Viewport.Y + GraphicsDevice.Viewport.Height / 2.5f),
                            menuFont, Color.Yellow, SimpleMenuAlign.CenterAligned, 10.0f);
            endGameMenu1.addMenuItem(new MenuItem("The Game is over."));
            endGameMenu1.addMenuItem(new MenuItem("Your score: " + Convert.ToString(score)));
            endGameMenu2 = new SimpleMenu(this,
                                        keyLikeMan,
                                        new Vector2(GraphicsDevice.Viewport.X + GraphicsDevice.Viewport.Width / 2,
                                                    GraphicsDevice.Viewport.Y + GraphicsDevice.Viewport.Height / 1.5f),
                                        menuFont, Color.Yellow, SimpleMenuAlign.CenterAligned, 10.0f);
            endGameMenu2.addMenuItem(new MenuItem("Return to Menu"));
        }

        /// <summary>
        /// Content kitöltő fv.
        /// </summary>
        protected override void UnloadContent()
        {
            // Nem kell csinálni semmit, megoldja az XNA
        }
        #endregion
        #region Update and Draw
        /// <summary>
        /// Vezérlő logika
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // fade
            if (fadeIn < (255 - fadeSpeed) && (0 <= (fadeIn + fadeSpeed))) fadeIn += fadeSpeed;
            // menu update
            endGameMenu2.updateMenuItems();
            if (fadeSpeed < 0 && 0 > (fadeIn + fadeSpeed) && goMainMenu)
            {
                fadeSpeed = -fadeSpeed;
                fadeIn = 0;
                game.Components.Remove(this);
                parentScreen.Initialize();
                game.Components.Add(parentScreen);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// Ezt a textmenü hívja, ha kiválasztásra kerül egy menuitem
        /// </summary>
        public void textMenuCallback(ITextMenu menu, MenuItem selectedMenuItem)
        {
            string selectedMenuText = selectedMenuItem.text();
            if (fadeSpeed != -1)
            {
                if (selectedMenuText.Equals("Return to Menu"))
                {
                    goMainMenu = true;
                    fadeSpeed = -fadeSpeed;
                }
            }
        }

        /// <summary>
        /// Ebben a függvényben történik a kiralyzolás
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            drawBackground();
            endGameMenu1.drawMenuItems(fadeIn, spriteBatch);
            endGameMenu2.drawMenuItems(fadeIn, spriteBatch);
            base.Draw(gameTime);
        }

        /// <summary>
        /// Ezzel rajzoljuk ki a hátteret(egyelőre csak egy kép, de más is lehet)
        /// </summary>
        void drawBackground()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Rectangle(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), new Color((byte)fadeIn, (byte)fadeIn, (byte)fadeIn));
            spriteBatch.End();
        }
        #endregion
    }
}
