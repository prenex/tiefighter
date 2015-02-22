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
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace MotherShipEffect1Pipeline
{
    /// <summary>
    /// A függvény valamilyen kódolással rendelkező textúrát alakít át Normalized4Byte
    /// formátumúvá. Az eredeti [0.0..1.0] közötti értékek [-1.0..1.0] közé esnek a
    /// preprocessz hatására. Ezt azért csináljuk, hogy ne a shaderben kelljen folyton
    /// megcsinálni!
    /// </summary>
    [ContentProcessor]
    [DesignTimeVisible(false)]
    class NormalMapTextureProcessor : ContentProcessor<TextureContent, TextureContent>
    {
        // Átalakítjuk a normal mapot, hogy Normalized4Byte legyen
        // és mipmapokat is csinálunk
        public override TextureContent Process(TextureContent input,
                                               ContentProcessorContext context)
        {
            // Először átkonvertáljuk az inputot vector4 típusúra
            input.ConvertBitmapType(typeof(PixelBitmapContent<Vector4>));

            // Minden face(általában csak 1 lesz) minden mipmapChain-jére
            foreach (MipmapChain mipmapChain in input.Faces)
            {
                // Minden mipmapChain minden textúráján
                foreach (PixelBitmapContent<Vector4> bitmap in mipmapChain)
                {
                    // Minden egyes texelre
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        for (int y = 0; y < bitmap.Height; y++)
                        {
                            // Kiolvasás
                            Vector4 encoded = bitmap.GetPixel(x, y);
                            // és normalizálás
                            bitmap.SetPixel(x, y, 2 * encoded - Vector4.One);
                        }
                    }
                }
            }

            // Végül az eredmény konvertálása a kimeneti formátumba, melyet majd a 
            // shader használhat
            input.ConvertBitmapType(typeof(PixelBitmapContent<NormalizedByte4>));

            // Ez nem kell, nem generáljuk újra a mipmapeket, mi magunk írtuk át a
            // régit...
            input.GenerateMipmaps(false);

            // Visszaadjuk amit kell
            return input;
        }
    }
}
