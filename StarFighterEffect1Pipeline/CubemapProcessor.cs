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
    /// Ez a textúra processzor hagyományos textúrát alakít át cube mappá.
    /// A processzor létrehozza a textúra tükörképét és maga mellé teszi azt
    /// horizontálisan. Ezek után vertikálisan 3 részre bontja a keletkezett
    /// textúrát. A középső részt 4 részre bontja, melyből egy kocka 4
    /// oldala fog keletkezni, az alsó és felső egyharmadból pedig 1-1 
    /// textúrát hoz létre, mely a 4 horizontális oldalnak megfelelően 4
    /// háromszög-alakú(elfajzott trapezoid) behajtással rendelkezik.
    /// </summary>
    [ContentProcessor]
    class CubemapProcessor : ContentProcessor<TextureContent, TextureCubeContent>
    {
        // A használt cube-map textúra felbontása:
        const int cubemapSize = 256;

        /// <summary>
        /// Egy sima 2D képet alakít ez át Cubemap textúrává.
        /// </summary>
        public override TextureCubeContent Process(TextureContent input,
                                                   ContentProcessorContext context)
        {
            // Átkonvertáljuk a textúrát, hogy könnyű legyen dolgozni vele
            input.ConvertBitmapType(typeof(PixelBitmapContent<Color>));

            // Tükrözzük vízszintesen a saját függvénnyel
            PixelBitmapContent<Color> mirrored = MirrorBitmap((PixelBitmapContent<Color>)input.Faces[0][0]);

            // Csinálunk egy új, üres cubemapet
            TextureCubeContent cubemap = new TextureCubeContent();

            // és előállítjuk a 6 oldalát a kockának
            cubemap.Faces[(int)CubeMapFace.NegativeZ] = CreateSideFace(mirrored, 0);
            cubemap.Faces[(int)CubeMapFace.NegativeX] = CreateSideFace(mirrored, 1);
            cubemap.Faces[(int)CubeMapFace.PositiveZ] = CreateSideFace(mirrored, 2);
            cubemap.Faces[(int)CubeMapFace.PositiveX] = CreateSideFace(mirrored, 3);
            cubemap.Faces[(int)CubeMapFace.PositiveY] = CreateTopFace(mirrored);
            cubemap.Faces[(int)CubeMapFace.NegativeY] = CreateBottomFace(mirrored);

            // Előállíttatjuk a mipmapokat
            cubemap.GenerateMipmaps(true);

            // Tömörített formátumba konvertáljuk a bitmapet
            cubemap.ConvertBitmapType(typeof(Dxt1BitmapContent));

            // és visszaadjuk
            return cubemap;
        }

        /// <summary>
        /// Egy sima 2D képet alakít egy vízszintesen kétszer akkorává tükrözés segítségével.
        /// </summary>
        static PixelBitmapContent<Color> MirrorBitmap(PixelBitmapContent<Color> source)
        {
            // Az új kép kétszer akkora lesz
            int width = source.Width * 2;

            // Ebbe írjuk be az új képet
            PixelBitmapContent<Color> mirrored = new PixelBitmapContent<Color>(width, source.Height);

            // Egy pixel olvasása után jobbról és balról beírás a célba
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    Color color = source.GetPixel(x, y);

                    mirrored.SetPixel(x, y, color);
                    mirrored.SetPixel(width - x - 1, y, color);
                }
            }

            // A kész tükrözötten kiegészített kép visszaadása
            return mirrored;
        }

        /// <summary>
        /// A kocka egy horizontális oldalának előállítása.
        /// </summary>
        /// <param name="source">Forráskép</param>
        /// <param name="cubeSide">Kocka horizontális oldalának a száma [0..3]</param>
        /// <returns></returns>
        private MipmapChain CreateSideFace(PixelBitmapContent<Color> source, int cubeSide)
        {
            // Az eredmény ide keletkezik
            PixelBitmapContent<Color> result = new PixelBitmapContent<Color>(cubemapSize, cubemapSize);

            // Az adott horizontális oldalhoz tartozó középső terület kiválasztása
            Rectangle sourceRegion = new Rectangle(source.Width * cubeSide / 4,
                                                   source.Height / 3,
                                                   source.Width / 4,
                                                   source.Height / 3);

            // A célterület az egész textúra
            Rectangle destinationRegion = new Rectangle(0, 0, cubemapSize, cubemapSize);

            // Másolás
            BitmapContent.Copy(source, sourceRegion, result, destinationRegion);

            // Az eredmény visszaadása
            return result;
        }

        // Sok stretch-átméretezés van az alsó a felső oldalnál, így szükséges
        // multisample-t használni és nagyobb képpel számolni, hogy rendes legyen...
        const int multisampleScale = 4;

        /// <summary>
        /// A kocka felső oldalának előállítása
        /// </summary>
        /// <param name="source">Forráskép</param>
        /// <returns></returns>
        private MipmapChain CreateTopFace(PixelBitmapContent<Color> source)
        {
            // Az eredmény ide keletkezik
            PixelBitmapContent<Color> result;
            // A multisample miatt nagyobb textúrával dolgozunk
            result = new PixelBitmapContent<Color>(cubemapSize * multisampleScale,
                                                   cubemapSize * multisampleScale);
            
            // A jobb oldal koordinátája
            int right = cubemapSize * multisampleScale - 1;

            // A 4 behajtás az alsó oldal közepéig
            ScaleTrapezoid(source, 0, -1, result, right,0,     -1,  0,  0,  1);
            ScaleTrapezoid(source, 1, -1, result, 0,    0,      0,  1,  1,  0);
            ScaleTrapezoid(source, 2, -1, result, 0,    right,  1,  0,  0, -1);
            ScaleTrapezoid(source, 3, -1, result, right,right,  0, -1, -1,  0);

            // Blurrolt eredményt adunk vissza
            return BlurCubemapFace(result);
        }

        /// <summary>
        /// A kocka alsó oldalának előállítása.
        /// </summary>
        /// <param name="source">Forráskép</param>
        /// <returns></returns>
        private MipmapChain CreateBottomFace(PixelBitmapContent<Color> source)
        {
            // Az eredmény ide keletkezik
            PixelBitmapContent<Color> result;
            // A multisample miatt nagyobb textúrával dolgozunk
            result = new PixelBitmapContent<Color>(cubemapSize * multisampleScale,
                                                   cubemapSize * multisampleScale);

            // A jobb oldal koordinátája
            int right = cubemapSize * multisampleScale - 1;

            // A 4 behajtás az alsó oldal közepéig
            ScaleTrapezoid(source, 0, 1, result, right, right,-1, 0,  0,-1);
            ScaleTrapezoid(source, 1, 1, result, 0,     right, 0,-1,  1, 0);
            ScaleTrapezoid(source, 2, 1, result, 0,         0, 1, 0,  0, 1);
            ScaleTrapezoid(source, 3, 1, result, right,     0, 0, 1, -1, 0);

            // Blurrolt eredményt adunk vissza
            return BlurCubemapFace(result);
        }

        /// <summary>
        /// 
        /// </summary>
        static void ScaleTrapezoid(PixelBitmapContent<Color> source,
                                   int cubeSide, int cubeY,
                                   PixelBitmapContent<Color> destination,
                                   int destinationX, int destinationY,
                                   int xDirection1, int yDirection1,
                                   int xDirection2, int yDirection2)
        {
            int size = destination.Width;

            // Kiszámoljuk a forrás 4 vízszintes részre osztásából
            // melyik kell(vízszintes bázis meghatározása)
            int baseSourceX = cubeSide * source.Width / 4;

            // A képet soronként másoljuk, közben stretchelés
            for (int row = 0; row < size / 2; row++)
            {
                // Eldöntjük a felső, vagy az alsó harmadból olvasunk(ez a bázis)
                int sourceY;
                if (cubeY < 0)
                    sourceY = source.Height / 3;
                else
                    sourceY = source.Height * 2 / 3;

                // Az y számolt bázisát eltoljuk úgy, hogy cubeY-tól függően
                // fel vagy le haladunk soronként a forrásban, melyet
                // magasság szerint 3 részre osztunk, majd ezt is felosszuk még size/2 részre.
                // Ezen felosztás alapján lépkedünk a forráson...
                sourceY += cubeY * row * source.Height / 3 / (size / 2);

                // Ezek a destination-ök kezdetben megadják hová írunk és
                // megváltoznak soronként.
                // Az adott soron történő célba-írás x,y-al megy, ezeket itt
                // ezért be kell állítani a sor másolás stetchelés
                int x = destinationX;
                int y = destinationY;

                // A behajtott háromszögben az aktuális sor hossza:
                int rowLength = size - row * 2;

                // Sor stretchelése
                for (int i = 0; i < rowLength; i++)
                {
                    // X forrás kiszámolása:
                    // Bázishoz hozzáadjuk a sor adott pixeljének a 
                    // forrásra képezettjét úgy, hogy a szélnél az 
                    // összes pixelt használjuk a forrásból, középnél meg
                    // egyre ritkábban mintavétetelezünk:
                    int sourceX = baseSourceX + i * source.Width / 4 / rowLength;

                    // Itt történik az olvasás
                    Color color = source.GetPixel(sourceX, sourceY);

                    // Itt pedig a célba beírás
                    destination.SetPixel(x, y, color);

                    // Végül pedig a soron belüli léptetés a célon
                    x += xDirection1;
                    y += yDirection1;
                }

                // Végül a következő sor kezdőpozíciójára is rá kell lépni
                destinationX += xDirection1 + xDirection2;
                destinationY += yDirection1 + yDirection2;
            }
        }

        /// <summary>
        /// The top and bottom cubemap faces will have a nasty discontinuity
        /// in the middle where the four source image flaps meet. We can cover
        /// this up by applying a blur filter to the problematic area.
        /// </summary>
        static BitmapContent BlurCubemapFace(PixelBitmapContent<Color> source)
        {
            // Két ideiglenes bitmap(eredeti méretűek, nem multisample)
            PixelBitmapContent<Vector4> temp1, temp2;
            temp1 = new PixelBitmapContent<Vector4>(cubemapSize, cubemapSize);
            temp2 = new PixelBitmapContent<Vector4>(cubemapSize, cubemapSize);

            // Bemásoljuk az egyikbe a forrást(kicsinyítünk is itt)
            BitmapContent.Copy(source, temp1);

            // Blur-t hajtunk végre innen oda és onnan ide
            // (2 menet: vertikális és horizontális)
            ApplyBlurPass(temp1, temp2, 1, 0);
            ApplyBlurPass(temp2, temp1, 0, 1);

            // Color formátumúvá konvertáljuk Vector4-ből
            PixelBitmapContent<Color> result;

            // Majd elmentjük az eredményt és visszaadjuk
            result = new PixelBitmapContent<Color>(cubemapSize, cubemapSize);
            BitmapContent.Copy(temp1, result);
            return result;
        }

        /// <summary>
        /// Box blur filter x vagy y tengely mentén.
        /// A gaussian blur sokkal jobb, de ide ez is elég és egyszerűbb.
        /// </summary>
        static void ApplyBlurPass(PixelBitmapContent<Vector4> source,
                                  PixelBitmapContent<Vector4> destination,
                                  int dx, int dy)
        {
            // középpont koordinátája(x és y egyaránt)
            int cubemapCenter = cubemapSize / 2;

            // Blur ciklus
            for (int y = 0; y < cubemapSize; y++)
            {
                for (int x = 0; x < cubemapSize; x++)
                {
                    // Meghatározzuk milyen távol vagyunk a közepétől
                    int xDist = cubemapCenter - x;
                    int yDist = cubemapCenter - y;
                    int distance = (int)Math.Sqrt(xDist * xDist + yDist * yDist);

                    // Ez azért kell, mert középen jobban akarunk blurolni, 
                    // mint a széleken, a széle blur nélkül kell legyen
                    // az illeszkedések miatt!
                    int blurAmount = Math.Max(cubemapCenter - distance, 0) / 8;

                    // Ebbe a vektorba gyújtjük a texel színét
                    Vector4 blurredValue = Vector4.Zero;

                    // dx-től és dy-tól függ, milyen tengely mentén megy a blur
                    // a texeltől relatívan ide-oda az egyenes mentén átlagoljuk a pontokat
                    for (int i = -blurAmount; i <= blurAmount; i++)
                    {
                        blurredValue += source.GetPixel(x + dx * i, y + dy * i);
                    }
                    blurredValue /= blurAmount * 2 + 1;

                    // A célba az átlagolt/blurrolt értéket írjuk be
                    destination.SetPixel(x, y, blurredValue);
                }
            }
        }
    }
}
