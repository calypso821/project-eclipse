// Define variables
sampler2D SpriteTexture : register(s0);

// Custom color parameter we'll pass from the game
float4 CustomColor : register(c0);

float4 PixelShaderFunction(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 tex = tex2D(SpriteTexture, texCoord);
    
    // Keep original alpha for transparency, but use custom color for visible pixels
    if (tex.a > 0)
    {
        return float4(CustomColor.rgb, tex.a);
    }
    return tex;
}

technique SpriteTechnique
{
    pass Pass1
    {
        // Changed from ps_2_0 to ps_4_0
        PixelShader = compile ps_3_0 PixelShaderFunction(); 
    }
}