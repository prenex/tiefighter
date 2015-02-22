// Ez a pixel shader végzi a glowoló részek kiválogatását!

// A spritebatch az s0 regiszterbe teszi a textúrát amit használunk vele
sampler TextureSampler : register(s0);

float GlowBonus;

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // kiszedjük a pixel színét(RGBA)
	float4 c = tex2D(TextureSampler, texCoord);

	c.a -= GlowBonus;

	// Minél kissebb az alfa érték, annál erõsebb lesz a színérték
	float selector = (1.0 - c.a);

	return c * selector;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
