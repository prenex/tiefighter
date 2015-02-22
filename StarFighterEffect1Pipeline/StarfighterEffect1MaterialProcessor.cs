using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.ComponentModel;
using System.IO;

namespace StarFighterEffect1Pipeline
{
    /// <summary>
    /// Material processzor vadászokhoz és lövegekhez, environment mappinggal.
    /// </summary>
    [ContentProcessor]
    class StarfighterEffect1MaterialProcessor : MaterialProcessor
    {
        private string environmentMap = "env_green.bmp";
        [DisplayName("Environment Map")]
        [DefaultValue("env_green.bmp")]
        [Description("The environment map applied to the model.")]
        public string EnvironmentMap
        {
            get { return environmentMap; }
            set { environmentMap = value; }
        }

        /// <summary>
        /// Egy material átkonvertálására ez hívódik
        /// </summary>
        public override MaterialContent Process(MaterialContent input,
                                                ContentProcessorContext context)
        {
            EffectMaterialContent customMaterial = new EffectMaterialContent();

            // beállítjuk az anyag által használt shadert
            string effectFile = Path.GetFullPath("Shaders\\StarFighterEffect1.fx");
            customMaterial.Effect = new ExternalReference<EffectContent>(effectFile);
            // Megj.: Az external reference azt jelenti, hogy ha az adott
            // materialt több másik content(pl. több modell) használja,
            // akkor is csak egyszer másoljuk át a tetúrákat!

            // A basic materialból információkat nyerünk ki, ezért tároljuk
            BasicMaterialContent basicMaterial = (BasicMaterialContent)input;

            // Ha volt a basic materialban diffúz textúra, akkor mentjük
            // és jelezzük, hogy volt!
            if (basicMaterial.Texture != null)
            {
                customMaterial.Textures.Add("Texture", basicMaterial.Texture);
                customMaterial.OpaqueData.Add("TextureEnabled", true);
            }

            // Az environment mapot is hozzáadjuk a textúrákhoz
            string envmap = Path.GetFullPath(EnvironmentMap);
            customMaterial.Textures.Add("EnvironmentMap", new ExternalReference<TextureContent>(envmap));

            // A többit elvégzi a bázisosztály processzora
            return base.Process(customMaterial, context);
        }

        /// <summary>
        /// Ez a függvény állítja elő a textúrákat, melyeket a materialban használunk.
        /// Azért van felüldefiniálva, mert a beépített csak sima textúrával törődik
        /// nekünk viszont a textúránkat cubemappá kell alakítanunk!
        /// </summary>
        protected override ExternalReference<TextureContent> BuildTexture(
                                            string textureName,
                                            ExternalReference<TextureContent> texture,
                                            ContentProcessorContext context)
        {
            // Az environment mapot cubemappá alakítjuk, a többi mehet az eredeti módon.
            if (textureName == "EnvironmentMap")
                return context.BuildAsset<TextureContent, TextureContent>(texture, "CubemapProcessor");
            else
                return base.BuildTexture(textureName, texture, context);
        }
    }
}
