// Ez a pixel shader alakítja ki a végleges képet a bluroltból és az eredetibõl

// A Bloomolt kép textúráját a spritebatch teszi s0-ba
sampler GlowSampler : register(s0);
// mi pedig betettük s1-be az eredetiét!
sampler BaseSampler : register(s1);

// Ezekkel változtatjuk a színtelítettséget
// float BaseSaturation;
// float GlowSaturation;

// Ezek a paraméterek azt határozzák meg, hogy melyik kép mennyire hangsúlyos forrás
float BaseIntensity;
float GlowIntensity;

// Ez a függvény módosítja egy adott szín telitettségét
// Megj.: A "sima" saturation intristic függvény csupán annyit csinál, hogy
// 0 és 1 közé szorítja az értékeket, ez teljesen más mint az a szaturáció...
//float4 AdjustSaturation(float4 color, float saturation)
//{
	//// Megnézzük a szín mennyire tér el a megadottól(dot product)
	//float grey = dot(color, float3(0.3, 0.59, 0.11));
	//// az eredményt felhasználva lineárisan interpolálunk az ilyen szürke és az 
	//// eredeti szín között a saturation paraméter alapján
	//return lerp(grey, color, saturation);
//}

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
	// Kinyerjük a színeket
    float4 glow = tex2D(GlowSampler, texCoord);
	float4 base = tex2D(BaseSampler, texCoord);

	// Szaturáljuk és az erõsségüket állítjuk
	//glow = AdjustSaturation(glow, GlowSaturation) * GlowIntensity;
    //base = AdjustSaturation(base, BaseSaturation) * BaseIntensity;
	glow = glow * GlowIntensity;
    base = base * BaseIntensity;

	// A nagyon bloomos részeknél sötétítjük az eredeti képet, hogy kicsit
	// megmaradjanak a kontúrok azért
	//base *= (1 - saturate(glow));

	return base + glow;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
