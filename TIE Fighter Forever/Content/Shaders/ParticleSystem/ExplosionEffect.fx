// Robbanás shader a részecskerendszerhez spherical billboard módszerrel

float4x4 world;
float4x4 view;
float4x4 projection;
float3 camPos;
float3 camUp;
float3 force;
float time;
float alpha;
float scale;

Texture baseTexture;
sampler textureSampler = sampler_state
{
	texture   = <baseTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU  = CLAMP;
	AddressV  = CLAMP;
};

struct VertexShaderInput
{
	// Pozíció
	float3 position					: POSITION0;
	// Textúrakoordináta(xy), születési idõ(z) és halál ideje(w)
	float4 texAndData				: TEXCOORD0;
	// Elmozdulásvektor(xyz) és egyediség(w)
	float4 deltaMoveAndRand			: TEXCOORD1;
};

struct VertexShaderOutput
{
	float4	position	: POSITION;
	float2	texCoord	: TEXCOORD0;
	float	a			: COLOR0;
};

// A billboard középpontjából kiteszi a vertexet a billboardnak megfelelõ helyre
// a textúrakoordinátája alapján, megfelelõ mérettel
float3 BillboardVertex(float3 billboardCenter, float2 cornerID, float size)
{
	// Kiszámoljuk a kamerától a billboard felé mutató szemvektort
	float3 eyeVector = billboardCenter - camPos;		
	
	// A billboard jobbra irányú vektora az eyeVectorra és a kamera fölfele irányuló
	// vektorára is merõleges, ezt kiszámolhatjuk keresztszorzással.
	// megj.: ez lesz a ténylegesen jobbra irány, megfelel az xna
	// koordinátarendszerének. Normalizáljuk, hogy könnyû legyen dolgozni vele
	float3 sideVector = cross(eyeVector, camUp);
	sideVector = normalize(sideVector);
	// A billboard felfele vektora a szemvektorra és az elõbb számoltra merõleges
	// és az is jobbkezes koordinátarendszert alkot, tehát direktszorzattal 
	// számolható. Ezt is normalizáljuk.
	float3 upVector = cross(sideVector,eyeVector);
	upVector = normalize(upVector);
	
	// Az adott vertexet el kell tolni a billboard közepébõl
	float3 finalPosition = billboardCenter;
	// A textúrakoordináták alapján
	finalPosition += (cornerID.x-0.5f)*sideVector*size;
	finalPosition += (0.5f-cornerID.y)*upVector*size;	
	
	// Az eredmény a vertex kitoltja lesz a megfelelõ paraméterekkel
	return finalPosition;
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;

	// Kiszámítjuk a world mátrixxal transzformált részecskepozíciót
	float3 startingPosition = mul(float4(input.position, 1), world);

	// Kiszedjük a paraméterezést a vertexchannel-bõl
	float2 tex = input.texAndData.xy;
	float3 delta = input.deltaMoveAndRand.xyz;
	float random = input.deltaMoveAndRand.w;
	float born = input.texAndData.z;
	float maxAge = input.texAndData.w;

	// kiszámoljuk a vertex részecskéjének a jelenlegi életkorát [0..1]-ben hol jár
	// az élete éppen.
	float age = time - born;	
	float relAge = age/maxAge;

	// 1-(relAge^2 / 2) alakú függvény szerint méretezzük a részecskénket, szorozva
	// a méretezõ shaderkonstanssal és a véletlenszerû értékkel ami a részecskéhez tartozik
	float sizer = saturate(1-relAge*relAge/2.0f);
	float size = scale*random*sizer;

	// A szinusz függvény elsõ negyedét(felszorozva) és a véletlent használva
	// megváltoztatjuk a részecske sebességét, hogy idõvel(távolabb) lassuljon le!
	float displacement = sin(relAge*6.28f/4.0f) * 3.0f * random;

	// Kiszámoljuk a billboard eltolását
	float3 billboardCenter = startingPosition + displacement * delta;
	// Ez pedig arra jó, hogy húzzuk a billboardunkat valamerre(gravitáció, vagy 
	// mozgó hajó, vagy felületrõl visszapattanás egyszerûbb emulálása)
	billboardCenter += age * force / 1000;

	// Kiszámoljuk a vertex tényleges pozícióját
	float3 finalPosition = BillboardVertex(billboardCenter, tex, size);
	float4 finalPosition4 = float4(finalPosition, 1);

	float4x4 viewProjection = mul (view, projection);
	output.position = mul(finalPosition4, viewProjection);

	// A PS-nek tudnia kell, hogy mennyire sötét a részecske
	// (fordított négyzetfüggvénnyel sötétítjük majd a textúrában lévõ színt)
	output.a = 1-relAge*relAge;
	// megj.: az itt kiszámolt a-nak semmi köze a majd lent kiszámolt alpha-hoz!!!

	// A textúrakoordinátákkal már nem csinálunk semmilyen transzformációt
	output.texCoord = tex;

	// VS ezennel végzett
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	// Kiszedjük a textúrát a samplerrel
    float3 rgb = tex2D(textureSampler, input.texCoord);
	// majd a távolság arányában megint csak szorozzuk a színértékeket
	rgb *= input.a;

	// végül visszaadjuk a kész pixelszínt a konstans shaderparaméter Alpha-val
	// kiegészítve!
    return float4(rgb, alpha);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
