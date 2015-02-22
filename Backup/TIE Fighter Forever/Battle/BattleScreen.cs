using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TIE_Fighter_Forever.Menu;
using TIE_Fighter_Forever.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace TIE_Fighter_Forever.Battle
{
    class BattleScreen : DrawableGameComponent, ITextMenuNotifiable
    {
        #region Fields
        // Communication:
        Game game;
        IKeyboardLikeManager keyLikeMan;
        DrawableGameComponent parentScreen;
        // Menu:
        SpriteFont menuFont;
        ITextMenu mainMenu;
        bool menuOn;
        // Graphics:
        SpriteBatch spriteBatch;
        #endregion
        #region Initialize
        public BattleScreen(Game game, DrawableGameComponent parentScreen)
            : base(game)
        {
            this.parentScreen = parentScreen;
            this.game = game;
            keyLikeMan = (IKeyboardLikeManager)game.Services.GetService(typeof(IKeyboardLikeManager));
            menuOn = false;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            menuFont = content.Load<SpriteFont>("MenuContent\\MenuFont1");

            mainMenu = new SimpleMenu(this,
                                        keyLikeMan,
                                        new Vector2(GraphicsDevice.Viewport.X + GraphicsDevice.Viewport.Width / 2,
                                                    GraphicsDevice.Viewport.Y + GraphicsDevice.Viewport.Height / 2.5f),
                                        menuFont, Color.Yellow, SimpleMenu.CenterAligned, 10.0f);
            mainMenu.addMenuItem(new MenuItem("Resume game"));
            mainMenu.addMenuItem(new MenuItem("Exit battle"));
        }

        protected override void UnloadContent()
        {
            // Nothing to do for now...
        }
        #endregion
        #region Update and Draw
        public override void Update(GameTime gameTime)
        {
            if (menuOn)
                mainMenu.updateMenuItems();
            else
            {
                if (keyLikeMan.exitPressed())
                    menuOn = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (menuOn)
                mainMenu.drawMenuItems(255, spriteBatch);
            {
            }
        }

        public void textMenuCallback(ITextMenu menu, String selectedMenuText)
        {
            if (selectedMenuText.Equals("Resume game"))
                menuOn = false;
            if (selectedMenuText.Equals("Exit battle"))
            {
                game.Components.Remove(this);
                parentScreen.Initialize();
                game.Components.Add(parentScreen);
            }
        }
        #endregion
    }
}
