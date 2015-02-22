using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TIE_Fighter_Forever.Input;

namespace TIE_Fighter_Forever.Menu
{
    class LoadingScreen: DrawableGameComponent, ITextMenuNotifiable
    {
        #region Fields
        // Menu:
        SpriteFont menuFont;
        ITextMenu loading;
        IKeyboardLikeManager keyLikeMan;
        // Graphics:
        SpriteBatch spriteBatch;
        #endregion
        #region Initialize
        public LoadingScreen(Game game)
            : base(game)
        {
            keyLikeMan = (IKeyboardLikeManager)game.Services.GetService(typeof(IKeyboardLikeManager));
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            menuFont = content.Load<SpriteFont>("MenuContent\\LoadFont");

            loading = new SimpleMenu(this,
                                        keyLikeMan,
                                        new Vector2(GraphicsDevice.Viewport.X + GraphicsDevice.Viewport.Width / 2,
                                                    GraphicsDevice.Viewport.Y + GraphicsDevice.Viewport.Height / 2.5f),
                                        menuFont, Color.Yellow, SimpleMenu.CenterAligned, 10.0f);
            loading.addMenuItem(new MenuItem("LOADING..."));
        }

        protected override void UnloadContent()
        {
            // Nothing to do for now...
        }
        #endregion
        #region Update and Draw
        public override void Update(GameTime gameTime)
        {
            loading.updateMenuItems();
        }

        public override void Draw(GameTime gameTime)
        {
            loading.drawMenuItems(255, spriteBatch);
        }

        public void textMenuCallback(ITextMenu menu, String selectedMenuText)
        {
        }
        #endregion
    }
}
