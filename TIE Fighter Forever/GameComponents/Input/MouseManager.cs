using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TIE_Fighter_Forever.Input
{
    class MouseManager : GameComponent, IMouseLikeManager
    {
        Game parent;
        MouseState newState, oldState;
        bool left, middle, right;
        int screenWidth, screenHeight;

        public MouseManager(Game game) : base(game)
        {
            game.Services.AddService(typeof(IMouseLikeManager), this);
            parent = game;
            oldState = Mouse.GetState();
        }

        public MouseManager(Game game, int screenWidth, int screenHeight)
            : base(game)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            game.Services.AddService(typeof(IMouseLikeManager), this);
            parent = game;
            oldState = Mouse.GetState();
        }

        /// <summary>
        /// A mouse manager állapotának frissítése
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            newState = Mouse.GetState();

            if ((oldState.LeftButton == ButtonState.Pressed) && (newState.LeftButton == ButtonState.Released))
                left = true;
            else
                left = false;
            if ((oldState.RightButton == ButtonState.Pressed) && (newState.RightButton == ButtonState.Released))
                right = true;
            else
                right = false;
            if ((oldState.MiddleButton == ButtonState.Pressed) && (newState.MiddleButton == ButtonState.Released))
                middle = true;
            else
                middle = false;

            oldState = newState;
        }

        public int getX()
        {
            return Mouse.GetState().X;
        }

        public int getY()
        {
            return Mouse.GetState().Y;
        }

        public int getZ()
        {
            return Mouse.GetState().ScrollWheelValue;
        }

        public bool leftButton()
        {
            return (Mouse.GetState().LeftButton == ButtonState.Pressed);
        }

        public bool middleButton()
        {
            return (Mouse.GetState().MiddleButton == ButtonState.Pressed);
        }

        public bool rightButton()
        {
            return (Mouse.GetState().RightButton == ButtonState.Pressed);
        }

        public bool leftButtonWasPressed()
        {
            return left;
        }

        public bool middleButtonWasPressed()
        {
            return middle;
        }

        public bool rightButtonWasPressed()
        {
            return right;
        }

        public float getNormalizedX()
        {
            int x = Mouse.GetState().X;
            return ((float)x / ((float)screenWidth / 2)) - 1.0f;
        }

        public float getNormalizedY()
        {
            int y = Mouse.GetState().Y;
            return ((float)y / ((float)screenHeight / 2)) - 1.0f;
        }

        public void setScreenWidth(int width)
        {
            this.screenWidth = width;
        }

        public void setScreenHeight(int height)
        {
            this.screenHeight = height;
        }
    }
}
