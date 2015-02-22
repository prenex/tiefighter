using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIE_Fighter_Forever.Input
{
    public interface IKeyboardLikeManager
    {
        /// <summary>
        /// Igaz, "következő menuItem" gomb lenyomása majd felengedése esetén.
        /// </summary>
        bool nextMenuItemPressed();
        /// <summary>
        /// Igaz, "előző menuItem" gomb lenyomása majd felengedése esetén.
        /// </summary>
        bool prevMenuItemPressed();
        /// <summary>
        /// Igaz, ha a felhasználó a menüt kiválasztó gombot nyomja le és engedi fel.
        /// </summary>
        bool enterMenuItemPressed();
        /// <summary>
        /// Igaz, "következő submenuItem" gomb lenyomása majd felengedése esetén.
        /// </summary>
        bool nextSubMenuItemPressed();
        /// <summary>
        /// Igaz, "előző submenuItem" gomb lenyomása majd felengedése esetén.
        /// </summary>
        bool prevSubMenuItemPressed();
        /// <summary>
        /// Igaz, ha vissza gombot nyomtak
        /// </summary>
        bool exitPressed();
    }
}
