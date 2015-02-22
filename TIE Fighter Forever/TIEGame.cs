using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using TIE_Fighter_Forever.Screens.Menu;
using TIE_Fighter_Forever.Input;
using TIE_Fighter_Forever.Screens.Battle;

namespace TIE_Fighter_Forever
{
    /// <summary>
    /// Ez a játék fõ osztálya, mely a belépési pontnál futtatva van
    /// </summary>
    public class TIEGame : Microsoft.Xna.Framework.Game
    {
        public Settings settings;
        public GraphicsDeviceManager graphics;

        public TIEGame()
        {
            // Alapbeállítások betöltése
            settings = new Settings();

            // és érvényesítése
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = settings.preferredScreenWidth;
            graphics.PreferredBackBufferHeight = settings.preferredScreenHeight;
            graphics.PreferredDepthStencilFormat = settings.preferredDepthStencil;
            graphics.IsFullScreen = settings.isFullScreen;
            graphics.PreferMultiSampling = settings.multiSampling;
            // ez azért kell, mert egyébként ritkább a draw hívás az update-ek javára
            // Ha false-ra teszem, akkor painkiller-es jellegû hatást vált ki, ami jobb mint az akadás!
            this.IsFixedTimeStep = true;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Itt inicializáljuk a játékot.
        /// A base.Initialize() meghívja az összes gyermek komponens inicializációját is, 
        /// persze csak azokét, amik már hozzá vannak adva komponensként!
        /// </summary>
        protected override void Initialize()
        {
            // Backface Culling:
            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone; // .CullCounterClockwise;

            KeyboardManager keyMan = new KeyboardManager(this);
            MouseManager mouseMan = new MouseManager(this);
            mouseMan.setScreenWidth(graphics.PreferredBackBufferWidth);
            mouseMan.setScreenHeight(graphics.PreferredBackBufferHeight);
            MainMenuScreen mainMenu = new MainMenuScreen(this);
            // Tesztelés:
            //BattleScreen mainMenu = new BattleScreen(this, null);

            /*keyMan.UpdateOrder = 0;
            mouseMan.UpdateOrder = 1;
            mainMenu.UpdateOrder = 10;*/

            this.Components.Add(keyMan);
            this.Components.Add(mouseMan);
            this.Components.Add(mainMenu);

            base.Initialize();
        }

        /// <summary>
        /// Itt töltjük be a grafikus contentet
        /// (az egyes képernyõk maguknak töltik ezt itt ezért üres)
        /// </summary>
        protected override void LoadContent()
        {
            // Contents are loaded in the screen classes
        }

        /// <summary>
        /// Ez játékonként egyszer hívódik meg és itt kell unloadolni mindent,
        /// amit nem content managerrel loadolunk...
        /// </summary>
        protected override void UnloadContent()
        {
            // Jelenleg mindent a content pipeline tölt be, itt nem áll semmi...
        }

        /// <summary>
        /// Itt történik az update, a játéklogika.
        /// Az összes update a komponensekben történik, 
        /// amit a base.Update() hív meg minden aktív komponensre!
        /// </summary>
        /// <param name="gameTime">Idõmérésre használható érték</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Itt történik a kirajzolás.
        /// Minden kirajzolás a komponensekben zajlik, innen a base.Draw() hívja õket.
        /// </summary>
        /// <param name="gameTime">Idõmérésre használható érték</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
