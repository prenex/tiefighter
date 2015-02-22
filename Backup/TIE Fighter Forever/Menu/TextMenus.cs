using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TIE_Fighter_Forever.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TIE_Fighter_Forever.Menu
{
    #region Interfaces for TextMenus
    public interface ITextMenuNotifiable
    {
        void textMenuCallback(ITextMenu menu, String selectedMenuText);
    }

    public interface ITextMenu
    {
        void addMenuItem(MenuItem menuItem);
        void drawMenuItems(int fadeIn, SpriteBatch spriteBatch);
        void updateMenuItems();
    }
    #endregion
    #region MenuItem Classes
    public class MenuItem
    {
        String oneText;

        /// <summary>
        /// This def. constructor can't be used
        /// </summary>
        protected MenuItem() { }

        /// <summary>
        /// Constuct a simple menu item
        /// </summary>
        public MenuItem(String text)
        {
            this.oneText = text;
        }

        /// <summary>
        /// Returns "".
        /// This method is provided for the ease of useing
        /// MenuItems and MultipleChoiceMenuItems together.
        /// </summary>
        public virtual string heading()
        {
            return "";
        }

        /// <summary>
        /// Gets the text string
        /// </summary>
        public virtual String text()
        {
            return oneText;
        }

        /// <summary>
        /// Does Nothing.
        /// This method is provided for the ease of useing
        /// MenuItems and MultipleChoiceMenuItems together.
        /// </summary>
        public virtual void nextSelection() { }

        /// <summary>
        /// Does Nothing.
        /// This method is provided for the ease of useing
        /// MenuItems and MultipleChoiceMenuItems together.
        /// </summary>
        public virtual void prevSelection() { }
    }

    public class MultipleChoiceMenuItem : MenuItem
    {
        String oneText;
        String[] texts;
        int selection;

        /// <summary>
        /// Constuct a menu item with multiple choices
        /// Heading is used to show menu items like: <heading><selectedTextFromTexts>
        /// Remark: Both of heading and texts can be null.
        /// </summary>
        public MultipleChoiceMenuItem(String heading, String[] texts)
            : base()
        {
            this.oneText = heading;
            this.texts = texts;
            this.selection = 0;
        }

        /// <summary>
        /// Gets the heading string(if there's any).
        /// Remark: It returns "" if it's the heading
        /// but returns "" too if heading == null!
        /// </summary>
        // "new" means this function will hide the inherited member function...
        public override string heading()
        {
            if (oneText == null)
                return "";
            else
                return oneText;
        }

        /// <summary>
        /// Gets the selections active text from texts[]
        /// It returns "" if texts == null.
        /// </summary>
        // "new" means this function will hide the inherited member function...
        public override String text()
        {
            if (texts == null)
                return " ";
            else
                return texts[selection];
        }

        /// <summary>
        /// Moves the subselection forward.
        /// If the current subselection is the last one,
        /// this method sets the selection to the first one...
        /// </summary>
        public override void nextSelection()
        {
            if (selection < texts.Length - 1)
                ++selection;
            else
                selection = 0;
        }

        /// <summary>
        /// Moves the subselection back.
        /// If the current subselection is the first one,
        /// this method sets the selection to the last one...
        /// </summary>
        public override void prevSelection()
        {
            if (selection > 0)
                --selection;
            else
                selection = texts.Length - 1;
        }
    }
    #endregion
    #region Implementations for TextMenus
    public class SimpleMenu : ITextMenu
    {
        #region Fields
        //Static Fields:
        public static int CenterAligned = 0;
        public static int LeftAligned = 1;

        // Used for drawing the menu items:
        SpriteFont menuFont;
        List<MenuItem> menuItems;
        int selectedMenu;
        Vector2 position;
        Color color;
        int align;
        float lineSize;

        // Used for scaling the selected menu item:
        float scale;
        float scaleSpeed;
        const float minScale = 1.0f;
        const float maxScale = 1.25f;
        bool scalingUp;

        // Used for communications and I/O:
        ITextMenuNotifiable parent;
        IKeyboardLikeManager keyLikeMan;

        // Miscelaneous:
        #endregion
        #region Methods
        /// <summary>
        /// Create a centered menu.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="keyLikeMan"></param>
        /// <param name="position"></param>
        /// <param name="menuFont"></param>
        public SimpleMenu(ITextMenuNotifiable parent,
                            IKeyboardLikeManager keyLikeMan,
                            Vector2 position,
                            SpriteFont menuFont,
                            Color color,
                            int align,
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
            spriteBatch.Begin();
            Vector2 pos = position;
            int i = 0;
            foreach (MenuItem menuItem in menuItems)
            {
                Vector2 sSize = menuFont.MeasureString(menuItem.heading() + menuItem.text());
                if(align == 1)
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
            // Scaling of the selection
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
            // Menu controls are here:
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
                parent.textMenuCallback(this, menuItems[selectedMenu].text());
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
    #endregion
}
