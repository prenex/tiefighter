// Egyszerû shader a skybox megjelenítésére

float4x4 rot;

texture baseTexture;

sampler baseSampler = 
sampler_state
{
    Texture = < baseTexture >;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    ADDRESSU = CLAMP;
    ADDRESSV = CLAMP;
};

struct VS_INPUT
{
    float4 ObjectPos: POSITION;
    float2 TextureCoords: TEXCOORD0;
};

struct VS_OUTPUT 
{
   float4 ScreenPos:   POSITION;
   float2 TextureCoords: TEXCOORD0;
};

struct PS_OUTPUT 
{
   float4 Color:   COLOR;
};


VS_OUTPUT SimpleVS(VS_INPUT In)
{
   VS_OUTPUT Out;

    Out.ScreenPos = mul(In.ObjectPos, rot);
    Out.TextureCoords = In.TextureCoords;

    return Out;
}

PS_OUTPUT SimplePS(VS_OUTPUT In)
{
    PS_OUTPUT Out;

    Out.Color = tex2D(baseSampler,In.TextureCoords);

    return Out;
}

technique Simple
{
   pass Single_Pass
   {
        VertexShader = compile vs_2_0 SimpleVS();
        PixelShader = compile ps_2_0 SimplePS();
   }

}