using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TIE_Fighter_Forever
{
    /// <summary>
    /// Ez az osztály tartalmaz minden beálítást a játékban, azért van egy helyen,
    /// hogy könnyű legyen lementeni a beállításokat!
    /// </summary>
    public class Settings
    {
        public Settings()
        {
            isFullScreen = true;
            preferredScreenWidth = 800;
            preferredScreenHeight = 600;
            preferredDepthStencil = DepthFormat.Depth24;
            multiSampling = false;
            laserGlow = true;
            glowBadness = 2.0f;
            clampFilter = SamplerState.LinearClamp;
            wrapFilter = SamplerState.LinearWrap;
        }

        /// <summary>
        /// Teljes képernyőn futunk-e
        /// </summary>
        public bool isFullScreen;
        /// <summary>
        /// Backbuffer x felbontása(csak elvárt érték, nem biztos hogy ennyi!)
        /// </summary>
        public int preferredScreenWidth;
        /// <summary>
        /// Backbuffer y felbontása(csak elvárt érték, nem biztos hogy ennyi!)
        /// </summary>
        public int preferredScreenHeight;
        /// <summary>
        /// Backbuffer stencil formátuma(csak elvárt érték, nem biztos hogy ez!)
        /// </summary>
        public DepthFormat preferredDepthStencil;
        /// <summary>
        /// Legyen-e multisampling(anti-aliasing)
        /// </summary>
        public bool multiSampling;
        /// <summary>
        /// Be van-e kapcsolva a laserglow effekt
        /// </summary>
        public bool laserGlow;
        /// <summary>
        /// Hányadára rontsa le a képernyő felbontását a glow a postprocess köztes 
        /// lépéseire.
        /// </summary>
        public float glowBadness;
        /// <summary>
        /// Filterezési mód wraping esetén(ez a gyakoribb)
        /// Megj.: Bizonyos esetekben nem ezt használjuk, hiába adjuk itt meg!
        /// </summary>
        public SamplerState wrapFilter;
        /// <summary>
        /// Filterezési mód clamp esetén(ez a ritkább)
        /// Megj.: Bizonyos esetekben nem ezt használjuk, hiába adjuk itt meg!
        /// </summary>
        public SamplerState clampFilter;
    }
}
