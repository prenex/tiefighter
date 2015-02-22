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
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content;

namespace MotherShipEffect1Pipeline
{
    /// <summary>
    /// Model processzor fõhajókhoz és ûrállomásokhoz, normal mapping támogatással
    /// </summary>
    [ContentProcessor]
    public class MothershipEffect1ModelProcessor : ModelProcessor
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
        /// Felüldefiniáljuk ezt a property-t, mert nekünk MINDIG szükségünk
        /// van a generált tangensekre!
        /// </summary>
        [Browsable(false)]
        public override bool GenerateTangentFrames
        {
            get { return true; }
            set { }
        }

        public override ModelContent Process(NodeContent input,
            ContentProcessorContext context)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            //directory = Path.GetDirectoryName(input.Identity.SourceFilename);

            if (!(NormalMapTexture.Equals("<auto>")) && (NormalMapTexture == null))
            {
                throw new InvalidContentException("Error finding normal map!");
            }

            return base.Process(input, context);
        }

        /// <summary>
        /// Ezek a számunkra is fontos adatok a vertex csatornából.
        /// A többit eldobjuk lejjebb!
        /// </summary>
        static IList<string> acceptableVertexChannelNames =
            new string[]
            {
                VertexChannelNames.TextureCoordinate(0),
                VertexChannelNames.Normal(0),
                VertexChannelNames.Binormal(0),
                VertexChannelNames.Tangent(0)
            };

        /// <summary>
        /// Ezt felüldefiniáltuk, hogy csak azok a vertex csatornák 
        /// maradjanak ami feltétlenül szükséges
        /// </summary>
        /// <param name="geometry">A geometria objektum ami tartalmaza
        /// a csatornát</param>
        /// <param name="vertexChannelIndex">A vertex csatorna indexe</param>
        /// <param name="context">A környezet amiben dolgozunk
        /// (pl. lehetne jelezni, hogy eltávolításra kerülnek dolgok)</param>
        protected override void ProcessVertexChannel(GeometryContent geometry,
                    int vertexChannelIndex, ContentProcessorContext context)
        {
            String vertexChannelName = geometry.Vertices.Channels[vertexChannelIndex].Name;

            // Ha a fenti felsorolásban megtalálható a csatornanév, akkor mehet
            if (acceptableVertexChannelNames.Contains(vertexChannelName))
            {
                base.ProcessVertexChannel(geometry, vertexChannelIndex, context);
            }
            // egyébként távolítsuk el, csupán felesleges adat van ott ilyenkor!
            else
            {
                geometry.Vertices.Channels.Remove(vertexChannelName);
            }
        }

        /// <summary>
        /// Az anyagok átalakításához a MothershipEffect1MaterialProcessort
        /// kell használni, beállítjuk, hogy ezt használjuk.
        /// </summary>
        protected override MaterialContent ConvertMaterial(MaterialContent material,
                                                         ContentProcessorContext context)
        {
            // Egy dictionary a processzor paraméterezésére, itt adjuk hozzá
            // a normalmapot is...
            OpaqueDataDictionary processorParameters = new OpaqueDataDictionary();
            // Megj.: Ezeknek public propertynek kell lenni a processzorban!
            // (convertáláskor íródnak be innen!)
            processorParameters["NormalMapTexture"] = NormalMapTexture;
            processorParameters["ColorKeyColor"] = ColorKeyColor;
            processorParameters["ColorKeyEnabled"] = ColorKeyEnabled;
            processorParameters["TextureFormat"] = TextureFormat;
            processorParameters["GenerateMipmaps"] = GenerateMipmaps;
            processorParameters["ResizeTexturesToPowerOfTwo"] = ResizeTexturesToPowerOfTwo;

            // Használjuk a saját processzort a fenti paraméterekkel
            return context.Convert<MaterialContent, MaterialContent>
                (material, "MothershipEffect1MaterialProcessor", processorParameters);
        }
    }
}