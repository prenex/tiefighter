using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TIE_Fighter_Forever.Components;
using System.ComponentModel;


namespace TIE_Fighter_Forever.GameComponents.LaserGlow
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class GlowComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Fields

        // Kommunikációhoz a fõ-játék-osztállyal
        TIEGame game;

        // Teljesképernyõs rajzoláshoz
        SpriteBatch spriteBatch;

        // Effect file a glowoló részek megkülönböztetésére
        Effect selectorEffect;
        // Effect file a gaussian blur végrehajtására
        Effect gaussianBlurEffect;
        // Effect file a végleges kép kialakítására
        Effect finalEffect;

        // A jelenetet ide rendereljük ki
        RenderTarget2D scene;
        // Ide jön a megkülönböztetett részek renderje és a második blur eredménye
        RenderTarget2D render0;
        // A megkülönböztetettet ide bluroljuk elõször, majd ezt felhasználva 
        // másodszor blurrolunk a másik irányba
        RenderTarget2D render1;

        // A glow postprocess effect beállítása
        GlowSetting setting = new GlowSetting(0.0f, 2.75f, 6.0f, 1.0f/*, 0.7f, 1.0f*/);

        public GlowSetting Setting
        {
            get { return setting; }
            set { setting = value; }
        }
        #endregion
        #region Initialization
        public GlowComponent(TIEGame game)
            : base(game)
        {
            if (game == null)
                throw new ArgumentNullException("game");
            else
                this.game = game;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Kell egy spritebatch a 2d rajzoláshoz
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Kellenek az effect fileok a shaderkódokkal
            selectorEffect = Game.Content.Load<Effect>("Shaders\\PostProcess\\SelectGlowEffect");
            gaussianBlurEffect = Game.Content.Load<Effect>("Shaders\\PostProcess\\GaussianBlurEffect");
            finalEffect = Game.Content.Load<Effect>("Shaders\\PostProcess\\FinalGlowEffect");

            // Lekérjük a backbuffer tulajdonságait
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            int width = pp.BackBufferWidth;
            int height = pp.BackBufferHeight;
            SurfaceFormat format = pp.BackBufferFormat;

            // Csinálunk egy ugyanakkora puffert, mint a backbuffer(ebbe megy az eredeti renderkép)
            scene = new RenderTarget2D(GraphicsDevice, width, height, false, format, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);

            width = (int)(width / game.settings.glowBadness);
            height = (int)(height / game.settings.glowBadness);

            // Majd csinálunk két kissebb köztes buffert is a blurhoz
            render0 = new RenderTarget2D(GraphicsDevice, width, height, false, format, DepthFormat.None);
            render1 = new RenderTarget2D(GraphicsDevice, width, height, false, format, DepthFormat.None);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            scene.Dispose();
            render0.Dispose();
            render1.Dispose();
            base.UnloadContent();
        }
        #endregion
        #region Draw
        /// <summary>
        /// Ezt a függvényt kell meghívni minden frameben még a kirajzolások elõtt!
        /// </summary>
        public void PrepareDraw()
        {
            // A visible tulajdonság meghatározza lefut-e a draw hívás ha komponensként
            // hozzá vagyunk adva, ezzel az if-el itt könnyen kezelhetõvé válik a glow
            // ki és bekapcsolása, mert hiszen elég a Visible-t állítani!
            // Megj.: Lehet hogy az ifet kiszedem majd, de annyit nem lassít egy ilyen
            // apróság!
            if (Visible)
            {
                // Beállítjuk, hogy a mi rendertargetünkbe rendereljünk...
                GraphicsDevice.SetRenderTarget(scene);
            }
        }
        /// <summary>
        /// Ez a függvény hívja meg a glow postprocessing tényleges meneteit és 
        /// rajzolja ki a backbufferbe a tényleges kész képet is.
        /// </summary>
        /// <param name="gameTime">Játékbeli idõ</param>
        public override void Draw(GameTime gameTime)
        {
            // ELSÕ MENET: Szelekció.
            selectorEffect.Parameters["GlowBonus"].SetValue(setting.GlowBonus);
            // A korábban a scene-be renderelt kép selektálása és átvitele render0-ba
            DrawFullscreenQuadToTarget(scene, render0, selectorEffect);

            // MÁSODIK MENET: Vízszintes Gaussian Blur.
            SetBlurEffectParameters(1.0f / (float)render0.Width, 0);
            // A szelektált képet render1-be rendereljük és vízszintesen bluroljuk
            DrawFullscreenQuadToTarget(render0, render1, gaussianBlurEffect);

            // HARMADIK MENET: Függõleges Gaussian Blur.
            SetBlurEffectParameters(0, 1.0f / (float)render0.Width);
            // A vízszintesen blurolt képet függõlegesen is bluroljuk, cél: render0
            DrawFullscreenQuadToTarget(render1, render0, gaussianBlurEffect);

            // VÉGSÕ MENET: Az eredeti scene és a blurolt kép összehozása.
            // Beállítjuk a backbuffert mint rendertargetet
            GraphicsDevice.SetRenderTarget(null);
            // Felparaméterezzük a glow-t
            EffectParameterCollection parameters = finalEffect.Parameters;
            parameters["BaseIntensity"].SetValue(setting.BaseIntensity);
            parameters["GlowIntensity"].SetValue(setting.GlowIntensity);
            //parameters["BaseSaturation"].SetValue(setting.BaseSaturation);
            //parameters["GlowSaturation"].SetValue(setting.GlowSaturation);

            // Beállítjuk a device-on a második textúrát(az nullást a spritebatch fogja, egyest a normal map miatt nem cseszegetjük!)
            // és a hozzá tartozó samplert (linear clamp azért kell, mert nem
            // kettõhatvány textúráknál reach esetén csak ez használható!)
            //SamplerState tmps0 = GraphicsDevice.SamplerStates[0];
            //SamplerState tmps1 = GraphicsDevice.SamplerStates[1];
            GraphicsDevice.SamplerStates[1] = game.settings.clampFilter;
            GraphicsDevice.Textures[1] = scene;

            // Kirendereljük a végleges, összetett képet
            Viewport viewport = GraphicsDevice.Viewport;
            DrawFullscreenQuad(render0, viewport.Width, viewport.Height, finalEffect);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Adott textúra adott méretben a képernyõre renderelése az adott effect-el!
        /// </summary>
        /// <param name="texture">Textúra</param>
        /// <param name="width">Szélesség</param>
        /// <param name="height">Magasság</param>
        /// <param name="effect">Használt effect</param>
        void DrawFullscreenQuad(Texture2D texture, int width, int height, Effect effect)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, null, null, null, effect);
            spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            spriteBatch.End();
        }

        /// <summary>
        /// Adott textúra adott rendertargetbe renderelése
        /// </summary>
        /// <param name="texture">Forrástextúra</param>
        /// <param name="target">Célpont rendertarget</param>
        /// <param name="effect">Használt effect</param>
        void DrawFullscreenQuadToTarget(Texture2D texture, RenderTarget2D target,
                                        Effect effect)
        {
            GraphicsDevice.SetRenderTarget(target);
            DrawFullscreenQuad(texture, target.Width, target.Height, effect);
        }

        /// <summary>
        ///  A gaussgörbe egy pontjának magasságát adja meg a bemeneti paraméter
        ///  függvényében!
        /// </summary>
        /// <param name="n">abszcissza</param>
        /// <returns>oordináta</returns>
        float Gauss(float x)
        {
            // Megj.: A haranggörbe csúcsának magassága 1 / sqrt(2*PI*theta^2)
            // Megj.: A haranggörbe "szélessége" theta függvénye
            // A blurAmount tehát a görbét paraméterezi a fenti két módon is!
            float theta = setting.BlurAmount;
            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta * theta)) *
                        Math.Exp(-(x * x) / (2 * theta * theta)));
        }

        void SetBlurEffectParameters(float dx, float dy)
        {
            // Az effect paramétereit az egyszerûség kedvéért összeállítjuk és egyben
            // beállítjuk!
            EffectParameter weightsParameter, offsetsParameter;
            weightsParameter = gaussianBlurEffect.Parameters["SampleWeights"];
            offsetsParameter = gaussianBlurEffect.Parameters["SampleOffsets"];
            
            // Illetve az is érdekel minket, hogy hány elemet támogatunk a shaderünkben
            // samplelési célokra...
            int sampleCount = weightsParameter.Elements.Count;

            // Ezekbe a tömbökbe kerülnek a súlyok és az eltolások
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // Az elsõ értékek mindig ezek(Gauss(0), nincs eltolás)
            sampleWeights[0] = Gauss(0);
            sampleOffsets[0] = new Vector2(0);

            // A súlyok összegének tárolására, hogy a végén normalizálhassunk
            float weightSum = Gauss(0);

            // A sampleCount páratlan: egy középsõ nulladik pozíció és egy egyenes 
            // mentén párosával ide-oda lépégetés van. A középsõt kiszámoltunk fent
            // most a többi jön!
            for (int i = 0; i < sampleCount / 2; ++i)
            {
                // Súlyok kiszámítása
                float weight = Gauss(i + 1);
                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                // súlyszumma növelése
                weightSum += 2 * weight;

                // Eltolások kiszámítása:
                // kihasználjuk, hogy a sampler hardware a két szomszédos sample 
                // átlagát adja meg akkor, ha pont a két sample közül olvasunk a 
                // shaderbõl. Ez a bónusz átlagolás sokat javít a blur-unk felbontásán!
                float offset = i * 2 + 1.5f;
                // Az elmozdulás vektort úgy kreáljuk, hogy a blur offseteinek
                // számolását a dx,dy paraméterek alapján állítsuk be(konkrétan mi 
                // majd függõleges és vízszintes elmozdulásokat állítunk be 1 és 0-val)
                Vector2 delta = new Vector2(dx, dy) * offset;
                // Offsetek beállítása a súlyoknak megfelelõen
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalizáljuk az eredményt, azaz leosztjuk a súlyokat, hogy a 
            // súlyösszeg 1 legyen!
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= weightSum;
            }

            // Végül mentjük a kiszámolt paramétereket a helyükre...
            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }

        #endregion
    }
}
