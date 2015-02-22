using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIE_Fighter_Forever.GameComponents.TextMenus
{
    public class MultipleChoiceMenuItem : MenuItem
    {
        String oneText;
        String[] texts;
        int selection;

        /// <summary>
        /// MenuItem, melyben többféle elem közül lehet választani
        /// Heading használata: <heading><selectedTextFromTexts>
        /// Megj.: A heading és a seletedTextFromTexts is lehet null.
        /// </summary>
        public MultipleChoiceMenuItem(String heading, String[] texts)
            : base()
        {
            this.oneText = heading;
            this.texts = texts;
            this.selection = 0;
        }

        /// <summary>
        /// Visszaadja a heading stringet.
        /// Megj.: Ha a string "", vagy null, ez a függvény akkor is ""-t ad
        /// </summary>
        public override string heading()
        {
            if (oneText == null)
                return "";
            else
                return oneText;
        }

        /// <summary>
        /// Visszaadja a jelenleg megjelenített lehetőséget, ha null akkor ""-t.
        /// </summary>
        public override String text()
        {
            if (texts == null)
                return " ";
            else
                return texts[selection];
        }

        /// <summary>
        /// Jobbra léptetjük a szelekciót, ha az utolsóra lépünk, akkor elsőre fog állni.
        /// </summary>
        public override void nextSelection()
        {
            if (selection < texts.Length - 1)
                ++selection;
            else
                selection = 0;
        }

        /// <summary>
        /// Balra léptetjük a szelekciót, ha a baloldalin áll, az utolsóra fog ezután állni.
        /// </summary>
        public override void prevSelection()
        {
            if (selection > 0)
                --selection;
            else
                selection = texts.Length - 1;
        }
    }
}
