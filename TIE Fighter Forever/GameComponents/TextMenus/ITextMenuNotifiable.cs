using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIE_Fighter_Forever.GameComponents.TextMenus
{
    public interface ITextMenuNotifiable
    {
        void textMenuCallback(ITextMenu menu, MenuItem selectedMenuItem);
    }
}
