// Egy kis normal mapping

float4x4 World;
float4x4 View;
float4x4 Projection;

// Fény tulajdonságai
float3 LightPosition;
float4 LightColor;
float4 AmbientLightColor;

// Anyagtulajdonságok
// Phong specular-hoz skálázó tényezõ
float Shininess;
// Exponens a phong lightinghez, a specular csillogás szélességét határozza meg
float SpecularPower;

// A kamera pozíciója(world space-ben)
float3 eyePosition;

// Normal map és a hozzá tartozó sampler
texture2D NormalMap;
sampler2D NormalMapSampler = sampler_state
{
    Texture = <NormalMap>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};
// Color map és a hozzá tartozó sampler
texture2D Texture;
sampler2D DiffuseTextureSampler = sampler_state
{
    Texture = <Texture>;
    MinFilter = linear;
    MagFilter = linear;
    MipFilter = linear;
};

// Ez a vertex shader inputja, itt pont az van, ami a vertex csatornákból megmarad!
struct VertexShaderInput
{
    float4 position		: POSITION0;
    float2 texCoord		: TEXCOORD0;
    float3 normal		: NORMAL0;    
    float3 binormal		: BINORMAL0;
    float3 tangent		: TANGENT0;

};

// A Vertex shader kimenete és a Pixel shader bemenete.
struct VertexShaderOutput
{
    float4 position			: POSITION0;
    float2 texCoord			: TEXCOORD0;
    float3 lightDirection	: TEXCOORD1;
    float3 viewDirection	: TEXCOORD2;	// utóbbi két vektor tangens térben!
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	// A pozíciót transzformáljuk a world mátrixal
    float4 worldPosition = mul(input.position, World);
    float4 viewPosition = mul(worldPosition, View);
	output.position = mul(viewPosition, Projection);

	// Fény irányának a kiszámítása("vég-kezdet")
	output.lightDirection = LightPosition - worldPosition;

	// Ezután kiszámoljuk a szemtõl a vertex felé mutató vektort
	output.viewDirection = worldPosition - eyePosition;

	// Elõállítjuk a 3x3-as mátrixot, mely a tangens térbõl a világtérbe helyez minket
	// Bázisnak a mesh tangens, binormál és normálvektorának world-space-ben vett
	// koordinátáit vesszük, ezzel szokásos ortogonális bázist kapunk, mely képes
	// a megfelelõ transzformációra...

	float3x3 tangentToWorld;
	tangentToWorld[0] = mul(input.tangent,    World);
    tangentToWorld[1] = mul(input.binormal,   World);
    tangentToWorld[2] = mul(input.normal,     World);

	// A fényt és a view-t tesszük tangens térbe, nem a PS-ben mátrixszorzunk!
	output.lightDirection = mul(tangentToWorld, output.lightDirection);
	output.viewDirection = mul(tangentToWorld, output.viewDirection);
	// Megj(magamnak).: Mint régi kódban a shadertut könyvtáramban

	// A textúrakoordinátákat nem bántjuk!
	output.texCoord = input.texCoord;

	// A számított értékek a pixel shadernek már interpolált formában
	// érkeznek meg minden pixelre!
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	// Ezek csak akkor kellenek, ha van léptékelés a mátrixokban
	input.viewDirection = normalize(input.viewDirection);
    input.lightDirection = normalize(input.lightDirection);

	// A normálvektort kiolvassuk a normal mapból
	// A pontosság javítása érdekében a samplelés után is normalizálunk!
	//float3 normalFromMap = normalize(tex2D(NormalMapSampler, input.texCoord));	// Elvileg ez nem túlzottan kell a preprocess miatt(csak lehetnek hibák, meg ugye a sample-lés)!!!
	float3 normalFromMap = tex2D(NormalMapSampler, input.texCoord);

	// A kapott normálvektorral diffúz-jellegû phong shadingot csinálunk
	float nDotL = max(dot(normalFromMap, input.lightDirection), 0);
    float4 diffuse = LightColor * nDotL;

	// specular phong shading: Visszatükrözzük a fényt a normálvektorral,
	// majd skaláris szorzással "vizsgáljuk" a view vektortól mennyire tér el...
	float3 reflectedLight = reflect(input.lightDirection, normalFromMap);
    float rDotV = max(dot(reflectedLight, input.viewDirection), 0.0001f);
	float power = pow(rDotV, SpecularPower);
	float4 specular = Shininess * LightColor * power;

	// A sima diffúz textúrát is kiolvassuk
	float4 diffuseTexture = tex2D(DiffuseTextureSampler, input.texCoord);

	// és visszaadjuk a számolt eredményszínt
	// tesztelés: return diffuseTexture;
	return (diffuse + AmbientLightColor) * diffuseTexture + specular;
	//return float4(normalFromMap,1);	// tesztelés
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}