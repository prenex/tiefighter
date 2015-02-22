using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIE_Fighter_Forever.Input
{
    public interface IKeyboardLikeManager
    {
        bool nextMenuItemPressed();
        bool prevMenuItemPressed();
        bool enterMenuItemPressed();
        bool nextSubMenuItemPressed();
        bool prevSubMenuItemPressed();
        bool exitPressed();
    }
}
