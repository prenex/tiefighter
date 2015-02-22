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
        // Needed buttons status:
        bool isEnterDown;
        bool isDownDown;
        bool isUpDown;
        bool isLeftDown;
        bool isRightDown;
        bool isEscapeDown;

        public KeyboardManager(Game game) : base(game)
        {
            // register this object as a service, for intercomponent communications
            game.Services.AddService(typeof(IKeyboardLikeManager), this);
            // set oldState to something
            oldState = Keyboard.GetState();
        }

        /// <summary>
        /// This function updates the state of the Keyboard manager
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

        /// <summary>
        /// Returns true if the enter is pressed
        /// </summary>
        public bool enterMenuItemPressed()
        {
            return isEnterDown;
        }

        /// <summary>
        /// Returns true if the down button is pressed
        /// </summary>
        public bool nextMenuItemPressed()
        {
            return isDownDown;
        }

        /// <summary>
        /// Returns true if the up button is pressed
        /// </summary>
        public bool prevMenuItemPressed()
        {
            return isUpDown;
        }

        /// <summary>
        /// Returns true if the right button is pressed
        /// </summary>
        public bool nextSubMenuItemPressed()
        {
            return isRightDown;
        }
        /// <summary>
        /// Returns true if the left button is pressed
        /// </summary>
        public bool prevSubMenuItemPressed()
        {
            return isLeftDown;
        }

        /// <summary>
        /// Returns true if the escape button is pressed
        /// </summary>
        public bool exitPressed()
        {
            return isEscapeDown;
        }
    }
}
