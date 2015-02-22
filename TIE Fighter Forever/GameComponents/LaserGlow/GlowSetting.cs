using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TIE_Fighter_Forever.Components
{
    public class GlowSetting
    {
        public GlowSetting(float GlowBonus, float BlurAmount, 
                            float GlowIntensity, float BaseIntensity
                            /*,float GlowSaturation, float BaseSaturation*/)
        {
            this.GlowBonus = GlowBonus;
            this.BlurAmount = BlurAmount;
            this.GlowIntensity = GlowIntensity;
            this.BaseIntensity = BaseIntensity;
            //this.GlowSaturation = GlowSaturation;
            //this.BaseSaturation = BaseSaturation;
        }
        #region Fields
        // Ez határozza meg mennyire fog glowolni az, aminek nagy az alfája,
        // mert ez az érték bonus glow-ként mindenhez hozzáadódik majd!
        public readonly float GlowBonus;

        // A blur effekt erőssége
        // Szokványos értékek: [1.0..10.0]
        public readonly float BlurAmount;

        // A végső kép kialakításánál játszanak szerepet. Ezek határozzák meg, hogy
        // a glow és az eredeti kép milyen arányban legyen összekeverve.
        // Szokványos értékek: [0.0..1.0]
        public readonly float GlowIntensity;
        public readonly float BaseIntensity;

        // A glowolt és az alap kép szaturációi
        //public readonly float GlowSaturation;
        //public readonly float BaseSaturation;
        #endregion
    }
}
