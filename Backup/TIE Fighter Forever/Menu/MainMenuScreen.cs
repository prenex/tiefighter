using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TIE_Fighter_Forever.Input;
using TIE_Fighter_Forever.Battle;

namespace TIE_Fighter_Forever.Menu
{
    public class MainMenuScreen : DrawableGameComponent, ITextMenuNotifiable
    {
        #region Fields
        // General fields:
        Game game;
        IKeyboardLikeManager keyLikeMan;
        SpriteBatch spriteBatch;

        // Used for drawing the background:
        Texture2D background;

        // The true Main Menu:
        ITextMenu mainMenu;
        // Used for the menu items:
        SpriteFont menuFont;

        // Used for scaling the selected menu item:
        const float minScale = 1.0f;
        const float maxScale = 1.25f;

        // Used for fading in&out:
        int fadeIn;
        int fadeSpeed;
        #endregion

        #region Initializaton
        public MainMenuScreen(Game game) : base(game)
        {
            this.game = game;
            keyLikeMan = (IKeyboardLikeManager)game.Services.GetService(typeof(IKeyboardLikeManager));
        }

        /// <summary>
        /// Initializes the main menu of the game
        /// </summary>
        public override void Initialize()
        {
            fadeSpeed = 3;
            fadeIn = fadeSpeed;
            base.Initialize();
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            ContentManager content = Game.Content;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = content.Load<Texture2D>("MenuContent\\MainMenu");
            menuFont = content.Load<SpriteFont>("MenuContent\\MenuFont1");

            mainMenu = new SimpleMenu(this,
                                        keyLikeMan,
                                        new Vector2(GraphicsDevice.Viewport.X + GraphicsDevice.Viewport.Width / 2,
                                                    GraphicsDevice.Viewport.Y + GraphicsDevice.Viewport.Height / 2.5f),
                                        menuFont, Color.Yellow, SimpleMenu.CenterAligned, 10.0f);
            mainMenu.addMenuItem(new MenuItem("New Campaign"));
            mainMenu.addMenuItem(new MenuItem("Load Campaign"));
            mainMenu.addMenuItem(new MenuItem("Battle"));
            mainMenu.addMenuItem(new MenuItem("Options"));
            mainMenu.addMenuItem(new MenuItem("Credits"));
            mainMenu.addMenuItem(new MenuItem("Exit Game"));
            /*String[] sa = { "ITEM1", "ITEM2", "ITEM3"};
            mainMenu.addMenuItem(new MultipleChoiceMenuItem("HEAD: ", sa));*/
        }

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Nothing to do for now...
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// This function updates the menu
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Doing the fade thing
            if (fadeIn < (255 - fadeSpeed) && (0 <= (fadeIn + fadeSpeed))) fadeIn += fadeSpeed;
            // and updating menu controls
            mainMenu.updateMenuItems();
            if (fadeSpeed < 0 && 0 > (fadeIn + fadeSpeed))
            {
                fadeSpeed = -fadeSpeed;
                fadeIn = 0;
                game.Components.Remove(this);
                LoadingScreen ls = new LoadingScreen(game);
                ls.Initialize();
                game.Components.Add(ls);
                BattleScreen bs = new BattleScreen(game, this);
                bs.Initialize();
                game.Components.Remove(ls);
                game.Components.Add(bs);
            }
        }

        /// <summary>
        /// This will be called when the user selects a menupoint.
        /// </summary>
        public void textMenuCallback(ITextMenu menu, String selectedMenuText)
        {
            if (fadeSpeed != -1)
            {
                if (selectedMenuText.Equals("Exit Game"))
                    game.Exit();
                if (selectedMenuText.Equals("Battle"))
                {
                    fadeSpeed = -fadeSpeed;
                }
            }
        }

        /// <summary>
        /// This function draws the menu
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
           drawBackground();
           mainMenu.drawMenuItems(fadeIn, spriteBatch);
        }

        /// <summary>
        /// This function is called just before menu items are drawn.
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
