// Environment mapping effect

float4x4 World;
float4x4 View;
float4x4 Projection;

float3 LightDirection;
float4 LightColor;
float3 AmbientColor;

float Glow;

// A kamera pozíciója(world space-ben)
float3 eyePosition;

bool TextureEnabled;

texture Texture;
texture EnvironmentMap;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float3 Reflection : TEXCOORD1;
    float3 Fresnel : COLOR0;
    float3 Lighting : COLOR1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	// A pozíciót transzformáljuk a world mátrixal
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	// A textúrakoordinátákat nem bántjuk
	output.TexCoord = input.TexCoord;

	// Kiszámítjuk a vertex worldspace-beli normálvektorát
	// Megj.: Feltesszük, hogy nincs léptékezés(scale), különben
	// float3 worldNormal = normalize(mul(input.Normal, World));
	// kellene ahelyett, hogy:
	float3 worldNormal = mul(input.Normal, World);

	// A szem koordinátáját számoljuk ki a világtérben. Ezt úgy tesszük, 
	// hogy vesszük a View transzformáció eltolás részét ésmegszorozzuk
	// a View mátrix balfelsõ 3x3asának(forgatási rész) inverzével
	// Mivel a mátrix ortonormális, az inverz itt transzponálást jelent.
	// Megj.: Ezt inkább CPU-val számoljuk ki és nem minden vertexre!!!
    //float3 eyePosition = mul(-View._m30_m31_m32, transpose(View));
	// Ezután kiszámoljuk a szemtõl a vertex felé mutató vektort
	float3 viewVector = worldPosition - eyePosition;
	// Egy intristic függvénnyel kiszámoljuk a elõzõleg számolt vektor
	// a világtérbeli normálvektorra tükrözöttjét
	output.Reflection = reflect(viewVector, worldNormal);
	// A fresnel együtthatót közelítjük azzal, hogy skalárszorozzuk
	// a nézeti és a nomálvektort, majd [0..1]-belivé tesszük az eredményt
    output.Fresnel = saturate(1 + dot(normalize(viewVector), worldNormal));
	// A fény számítása is hasonlóan zajlik
    float lightAmount = max(dot(worldNormal, normalize(LightDirection)), 0);
	output.Lighting = AmbientColor + lightAmount * LightColor;

	// A számított értékek a pixel shadernek már interpolált formában
	// érkeznek meg minden pixelre!
    return output;
}

// Sampler a "sima" textúrák eléréséhez
sampler TextureSampler = sampler_state
{
    Texture = (Texture);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    
    AddressU = Wrap;
    AddressV = Wrap;
};
// Sampler az environment textúrák eléréséhez
sampler EnvironmentMapSampler = sampler_state
{
    Texture = (EnvironmentMap);

    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    
    AddressU = Clamp;
    AddressV = Clamp;
};

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float3 color;
	// Lehet, hogy nincs a modelhez diffúz textúra, ekkor legyen fekete
	// egyébként a tex2D-vel olvassunk a sima samplerbõl.
	if (TextureEnabled)
        color = tex2D(TextureSampler, input.TexCoord);
    else
        color = float3(0, 0, 0);
	// Az envmap színértékét a texCUBE függvénnyel olvassuk
	// a texcube fv. valósítja meg a cubemapokból történõ helyes olvasást!
	float3 envmap = texCUBE(EnvironmentMapSampler, -input.Reflection);
	// Az interpolált és közelítõ fresnel értékkel végezzünk lineáris
	// interpolációt(lerp) a szín és az environment szín között!
    color = lerp(color, envmap, input.Fresnel);

    // A színeket utókezeljük a fény hatására
    color *= input.Lighting * 2;

	// alpha csatorna 1-re írásával térünk vissza
	return float4(color, Glow);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
