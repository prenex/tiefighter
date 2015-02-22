// Ez a pixel shader felelõs a gaussian blur-ért egy egyenes mentén

// A spritebatch az s0 regiszterbe teszi a textúrát amit használunk vele
sampler TextureSampler : register(s0);

// 15 darab sample lesz
#define SAMPLE_COUNT 15
// Ezek eltolási koordinátái és súlyai pedig itt találhatóak:
float2 SampleOffsets[SAMPLE_COUNT];
float SampleWeights[SAMPLE_COUNT];


float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 c = 0;

	// c-be az átlagolt szín kerül(A súlyok összege egy! Ezt elõre kiszámoltuk!)
	for(int i = 0; i < SAMPLE_COUNT; ++i)
	{
		c += tex2D(TextureSampler, texCoord + SampleOffsets[i]) * SampleWeights[i];
	}

	// A blurolt szín az eredményünk
    return c;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
