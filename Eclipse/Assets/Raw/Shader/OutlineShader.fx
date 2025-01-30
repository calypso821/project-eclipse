sampler2D SpriteTexture : register(s0);
float4 OutlineColor : register(c0);
float OutlineWidth : register(c1);
float4 OriginalColor : register(c2);
bool DrawOutline : register(c3);

float4 PixelShaderFunction(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 texColor = tex2D(SpriteTexture, texCoord);
    float4 finalColor = texColor * OriginalColor; // Combine texture with original color
    
    // Only process pixels that are (nearly) transparent
    if (DrawOutline && texColor.a < 0.1f)
    {
        // Check surrounding pixels for non-transparency
        float2 offsets[4] =
        {
            float2(OutlineWidth, 0), // right
            float2(-OutlineWidth, 0), // left
            float2(0, OutlineWidth), // down
            float2(0, -OutlineWidth) // up
        };
        
        // Check if any neighboring pixel is non-transparent
        for (int i = 0; i < 4; i++)
        {
            float4 neighborColor = tex2D(SpriteTexture, texCoord + offsets[i]);
            if (neighborColor.a > 0.1f)
            {
                return OutlineColor; // Draw outline here
            }
        }
    }
    
    return finalColor; // Keep original pixel
}

technique SpriteTechnique
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}