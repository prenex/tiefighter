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

namespace MotherShipEffect1Pipeline
{
    [ContentProcessor]
    class MothershipEffect1MaterialProcessor : MaterialProcessor
    {
        /// <summary>
        /// A normal map auto beállítása, vagy maga a normál map helye!
        /// </summary>
        private string normalMapTexture = "<auto>";
        [DisplayName("Normal Map Setting")]
        [DefaultValue("<auto>")]
        [Description("The normal map applied to the model. All <texture>_bump.* files became normal map for a material if you set this to <auto>!")]
        public string NormalMapTexture
        {
            get { return normalMapTexture; }
            set { normalMapTexture = value; }
        }

        /// <summary>
        /// Egy material átkonvertálására ez hívódik
        /// </summary>
        public override MaterialContent Process(MaterialContent input,
                                                ContentProcessorContext context)
        {
            EffectMaterialContent customMaterial = new EffectMaterialContent();

            // beállítjuk az anyag által használt shadert
            string effectFile = Path.GetFullPath("Shaders\\MothershipEffect1.fx");
            customMaterial.Effect = new ExternalReference<EffectContent>(effectFile);
            // Megj.: Az external reference azt jelenti, hogy ha az adott
            // materialt több másik content(pl. több modell) használja,
            // akkor is csak egyszer másoljuk át a tetúrákat!

            // A basic materialból információkat nyerünk ki, ezért tároljuk
            BasicMaterialContent basicMaterial = (BasicMaterialContent)input;

            if (basicMaterial.Texture != null)
            {
                // Ha volt a basic materialban diffúz textúra, akkor mentjük
                // és jelezzük, hogy van textúra
                customMaterial.Textures.Add("Texture", basicMaterial.Texture);
                //customMaterial.OpaqueData.Add("TextureEnabled", true);

                if (NormalMapTexture.Equals("<auto>"))
                {
                    // Ha <auto>, akkor a sima textúra átnevezésével kapunk normal-t!
                    string texfilename = basicMaterial.Texture.Filename;
                    string normap = Path.GetFullPath(texfilename.Replace(".", "_bump."));
                    customMaterial.Textures.Add("NormalMap", new ExternalReference<TextureContent>(normap));
                }
                else
                {
                    // Amúgy az adott normal mapot adjuk a textúrákhoz
                    string normap = Path.GetFullPath(NormalMapTexture);
                    customMaterial.Textures.Add("NormalMap", new ExternalReference<TextureContent>(normap));
                }
            }

            // A többit elvégzi a bázisosztály processzora
            return base.Process(customMaterial, context);
        }

        /// <summary>
        /// Ez a függvény állítja elő a textúrákat, melyeket a materialban használunk.
        /// Azért van felüldefiniálva, mert a beépített csak sima textúrával törődik
        /// nekünk viszont a textúránkat át kell alakítanunk egy kicsit...
        /// </summary>
        protected override ExternalReference<TextureContent> BuildTexture(
                                            string textureName,
                                            ExternalReference<TextureContent> texture,
                                            ContentProcessorContext context)
        {
            // A normal mapot kicsit átalakítjuk, a többi mehet az eredeti módon.
            if (textureName == "NormalMap")
                return context.BuildAsset<TextureContent, TextureContent>(texture, "NormalMapTextureProcessor");
            else
                return base.BuildTexture(textureName, texture, context);
        }
    }
}
