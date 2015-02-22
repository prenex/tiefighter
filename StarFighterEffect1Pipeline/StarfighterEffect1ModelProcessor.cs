using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.ComponentModel;

namespace StarFighterEffect1Pipeline
{
    /// <summary>
    /// Model processzor vadászokhoz és lövegekhez, environment mapping támogatással
    /// </summary>
    [ContentProcessor]
    public class StarfighterEffect1ModelProcessor : ModelProcessor
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
        /// Az anyagok átalakításához a StarFighterEffect1MaterialProcessort
        /// kell használni, beállítjuk, hogy ezt használjuk.
        /// </summary>
        protected override MaterialContent ConvertMaterial(MaterialContent material,
                                                         ContentProcessorContext context)
        {
            // Egy dictionary a processzor paraméterezésére, itt adjuk hozzá
            // az environment mapot is...
            OpaqueDataDictionary processorParameters = new OpaqueDataDictionary();
            processorParameters["EnvironmentMap"] = EnvironmentMap;
            processorParameters["ColorKeyColor"] = ColorKeyColor;
            processorParameters["ColorKeyEnabled"] = ColorKeyEnabled;
            processorParameters["TextureFormat"] = TextureFormat;
            processorParameters["GenerateMipmaps"] = GenerateMipmaps;
            processorParameters["ResizeTexturesToPowerOfTwo"] = ResizeTexturesToPowerOfTwo;

            // Használjuk a saját processzort a fenti paraméterekkel
            return context.Convert<MaterialContent, MaterialContent>
                (material, "StarfighterEffect1MaterialProcessor", processorParameters);
        }
    }
}