using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TIE_Fighter_Forever.GameComponents.TextMenus
{
    public interface ITextMenu
    {
        void addMenuItem(MenuItem menuItem);
        void drawMenuItems(int fadeIn, SpriteBatch spriteBatch);
        void updateMenuItems();
    }
}
