using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TIE_Fighter_Forever.Input;

namespace TIE_Fighter_Forever.GameComponents.TextMenus
{
    public enum SimpleMenuAlign
    {
        CenterAligned,
        LeftAligned
    }
    public class SimpleMenu : ITextMenu
    {
        #region Fields
        //Statikus mezők:

        // Rajzoláshoz használt mezők:
        SpriteFont menuFont;
        List<MenuItem> menuItems;
        int selectedMenu;
        Vector2 position;
        Color color;
        SimpleMenuAlign align;
        float lineSize;

        // Léptékeléshez használt mezők:
        float scale;
        float scaleSpeed;
        const float minScale = 1.0f;
        const float maxScale = 1.25f;
        bool scalingUp;

        // Kommunikáció és I/O:
        ITextMenuNotifiable parent;
        IKeyboardLikeManager keyLikeMan;

        // egyebek:
        #endregion
        #region Methods
        /// <summary>
        /// Előállít egy egyszerű, igazított elemekből álló menüt.
        /// </summary>
        /// <param name="parent">Callback függvényt biztosító objektum</param>
        /// <param name="keyLikeMan">Menüpontok közötti lépkedéshez használt irányító objektum</param>
        /// <param name="position">Koordináta</param>
        /// <param name="menuFont">Használt font</param>
        /// <param name="color">Menü színe</param>
        /// <param name="align">Menü igazítása</param>
        /// <param name="lineSize">Sorok mérete</param>
        public SimpleMenu(  ITextMenuNotifiable parent,
                            IKeyboardLikeManager keyLikeMan,
                            Vector2 position,
                            SpriteFont menuFont,
                            Color color,
                            SimpleMenuAlign align,
                            float lineSize)
        {
            this.lineSize = lineSize;
            this.align = align;
            this.color = color;
            this.position = position;
            menuItems = new List<MenuItem>();
            selectedMenu = 0;
            scale = 1.0f;
            scaleSpeed = 0.02f;
            scalingUp = true;
            this.keyLikeMan = keyLikeMan;
            this.parent = parent;
            this.menuFont = menuFont;
        }

        #endregion
        #region Interface-inherited methods
        public void addMenuItem(MenuItem menuItem)
        {
            menuItems.Add(menuItem);
        }
        public void drawMenuItems(int fadeIn, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);
            Vector2 pos = position;
            int i = 0;
            foreach (MenuItem menuItem in menuItems)
            {
                Vector2 sSize = menuFont.MeasureString(menuItem.heading() + menuItem.text());
                if (align == SimpleMenuAlign.LeftAligned)
                    sSize.X = 0;
                if (i != selectedMenu)
                {
                    pos.X -= sSize.X / 2;
                    spriteBatch.DrawString(menuFont, menuItem.heading() + menuItem.text(), pos, new Color(Vector3.Multiply(color.ToVector3(), (float)fadeIn / (float)255)), 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                    pos.X += sSize.X / 2;
                }
                else
                {
                    pos.X -= (sSize.X * scale) / 2;
                    spriteBatch.DrawString(menuFont, menuItem.heading() + menuItem.text(), pos, new Color(Vector3.Multiply(color.ToVector3(), (float)fadeIn / (float)255)), 0.0f, Vector2.Zero, scale, SpriteEffects.None, 1.0f);
                    pos.X += (sSize.X * scale) / 2;
                }
                ++i;
                pos.Y += (sSize.Y + lineSize);
            }
            spriteBatch.End();
        }
        public void updateMenuItems()
        {
            // A kiválasztott léptékelése, kiemelés céljából
            if (scalingUp)
            {
                if (scale > maxScale)
                {
                    scaleSpeed = -scaleSpeed;
                    scalingUp = false;
                }
                else
                {
                    scale += scaleSpeed;
                }
            }
            else
            {
                if (scale < minScale)
                {
                    scaleSpeed = -scaleSpeed;
                    scalingUp = true;
                }
                else
                {
                    scale += scaleSpeed;
                }
            }
            // Menüben való lépkedés megvalósítása
            if (keyLikeMan.nextMenuItemPressed())
            {
                ++selectedMenu;
                if (selectedMenu > menuItems.Count - 1)
                    selectedMenu = 0;
            }
            if (keyLikeMan.prevMenuItemPressed())
            {
                --selectedMenu;
                if (selectedMenu < 0)
                    selectedMenu = menuItems.Count - 1;
            }
            if (keyLikeMan.enterMenuItemPressed())
            {
                parent.textMenuCallback(this, menuItems[selectedMenu]);
            }
            if (keyLikeMan.nextSubMenuItemPressed())
            {
                menuItems[selectedMenu].nextSelection();
            }
            if (keyLikeMan.prevSubMenuItemPressed())
            {
                menuItems[selectedMenu].prevSelection();
            }
        }
        #endregion
    }
}
