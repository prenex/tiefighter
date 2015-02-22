using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIE_Fighter_Forever.Input
{
    interface IMouseLikeManager
    {
        /// <summary>
        ///  Should be called when the manager gets initialized
        /// </summary>
        void reset();

        /// <summary>
        /// Képernyő szélességét állítja be
        /// </summary>
        void setScreenWidth(int width);
        /// Képernyő magasságát állítja be
        /// </summary>
        void setScreenHeight(int height);
        /// <summary>
        /// Kurzor X koordinátáját adja meg
        /// </summary>
        int getX();
        /// <summary>
        /// Kurzor Y koordinátáját adja meg
        /// </summary>
        int getY();
        /// <summary>
        /// Kurzor Z koordinátáját adja meg(pl. görgő)
        /// </summary>
        int getZ();
        /// <summary>
        /// Kurzor X koordinátáját adja meg [-1.0f..1.0f] intervallumra normálva
        /// </summary>
        float getNormalizedX();
        /// <summary>
        /// Kurzor Y koordinátáját adja meg [-1.0f..1.0f] intervallumra normálva
        /// </summary>
        float getNormalizedY();
        /// <summary>
        /// Kurzor Z koordinátáját adja meg [-1.0f..1.0f] intervallumra normálva
        /// </summary>
        float getNormalizedZ();
        /// Igaz, ha a bal gomb ÉPPEN le van nyomva
        /// </summary>
        bool leftButton();
        /// <summary>
        /// Igaz, ha a középső gomb ÉPPEN le van nyomva
        /// </summary>
        bool middleButton();
        /// <summary>
        /// Igaz, ha a jobb gomb ÉPPEN le van nyomva
        /// </summary>
        bool rightButton();
        /// <summary>
        /// Igaz, ha a bal gomb le volt nyomva és fel is lett engedve
        /// </summary>
        bool leftButtonWasPressed();
        /// <summary>
        /// Igaz, ha a középső gomb le volt nyomva és fel is lett engedve
        /// </summary>
        bool middleButtonWasPressed();
        /// <summary>
        /// Igaz, ha a jobb gomb le volt nyomva és fel is lett engedve
        /// </summary>
        bool rightButtonWasPressed();
    }
}
