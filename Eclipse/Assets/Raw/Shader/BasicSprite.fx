#if OPENGL
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Matrix for transformation
matrix WorldViewProjection;

// Texture and sampler
texture2D MainTexture;
sampler2D MainTextureSampler = sampler_state
{
    Texture = <MainTexture>;
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

// Vertex shader input
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
    float4 Color : COLOR0;
};

// Vertex shader output
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
    float4 Color : COLOR0;
};

// Vertex shader
VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
    output.Position = mul(input.Position, WorldViewProjection);
    output.TexCoord = input.TexCoord;
    output.Color = input.Color;
    return output;
}

// Pixel shader
float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 texColor = tex2D(MainTextureSampler, input.TexCoord);
    return texColor * input.Color;
}

// Technique
technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}