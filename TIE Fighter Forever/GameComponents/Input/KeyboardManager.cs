using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TIE_Fighter_Forever.Input
{
    class KeyboardManager : GameComponent, IKeyboardLikeManager
    {
        KeyboardState oldState;
        KeyboardState newState;
        // Gombok státuszai:
        bool isEnterDown;
        bool isDownDown;
        bool isUpDown;
        bool isLeftDown;
        bool isRightDown;
        bool isEscapeDown;

        public KeyboardManager(Game game) : base(game)
        {
            // Komponensek közötti kommunikáció miatt bejelentjük, hogy ez egy szolgáltalása a játéknak
            game.Services.AddService(typeof(IKeyboardLikeManager), this);
            // oldstate legyen valami...
            oldState = Keyboard.GetState();
        }

        /// <summary>
        /// A keyboard manager állapotának frissítése
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            newState = Keyboard.GetState();
            
            // enter
            if (oldState.IsKeyDown(Keys.Enter) && newState.IsKeyUp(Keys.Enter))
                isEnterDown = true;
            else
                isEnterDown = false;
            // down arrow
            if (oldState.IsKeyDown(Keys.Down) && newState.IsKeyUp(Keys.Down))
                isDownDown = true;
            else
                isDownDown = false;
            // up arrow
            if (oldState.IsKeyDown(Keys.Up) && newState.IsKeyUp(Keys.Up))
                isUpDown = true;
            else
                isUpDown = false;
            // left arrow
            if (oldState.IsKeyDown(Keys.Left) && newState.IsKeyUp(Keys.Left))
                isLeftDown = true;
            else
                isLeftDown = false;
            // right arrow
            if (oldState.IsKeyDown(Keys.Right) && newState.IsKeyUp(Keys.Right))
                isRightDown = true;
            else
                isRightDown = false;
            // escape
            if (oldState.IsKeyDown(Keys.Escape) && newState.IsKeyUp(Keys.Escape))
                isEscapeDown = true;
            else
                isEscapeDown = false;

            oldState = newState;
        }

        public bool enterMenuItemPressed()
        {
            return isEnterDown;
        }

        public bool nextMenuItemPressed()
        {
            return isDownDown;
        }

        public bool prevMenuItemPressed()
        {
            return isUpDown;
        }

        public bool nextSubMenuItemPressed()
        {
            return isRightDown;
        }

        public bool prevSubMenuItemPressed()
        {
            return isLeftDown;
        }

        public bool exitPressed()
        {
            return isEscapeDown;
        }
    }
}
