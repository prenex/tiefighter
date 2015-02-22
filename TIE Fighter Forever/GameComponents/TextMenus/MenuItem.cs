using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIE_Fighter_Forever.GameComponents.TextMenus
{
    public class MenuItem
    {
        String oneText;

        /// <summary>
        /// Csak kell, de nem látható...
        /// </summary>
        protected MenuItem() { }

        /// <summary>
        /// Egyszerű MenuItem
        /// </summary>
        public MenuItem(String text)
        {
            this.oneText = text;
        }

        /// <summary>
        /// Return "".
        /// Ez csak azért van itt, hogy könnyen lehessen használni mindenféle
        /// menuitemeket, bonyolultat és simát együtt
        /// </summary>
        public virtual string heading()
        {
            return "";
        }

        /// <summary>
        /// Text stringet adja vissza
        /// </summary>
        public virtual String text()
        {
            return oneText;
        }

        /// <summary>
        /// Nem csinál semmit, lsd. heading
        /// </summary>
        public virtual void nextSelection() { }

        /// <summary>
        /// Nem csinál semmit, lsd. heading
        /// </summary>
        public virtual void prevSelection() { }
    }
}
