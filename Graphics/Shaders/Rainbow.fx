matrix uTransformMatrix;
float uTime;

float3 uColor;
float uBrightness;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

static const float3 rainbowColors[8] =
{
    float3(1.00, 0.00, 0.00), // Red
    float3(1.00, 0.35, 0.00), // Orange
    float3(1.00, 1.00, 0.00), // Yellow
    float3(0.00, 1.00, 0.00), // Green
    float3(0.00, 0.55, 1.00), // Cyan-Blue
    float3(0.00, 0.00, 1.00), // Blue
    float3(0.29, 0.00, 0.51), // Indigo
    float3(0.56, 0.00, 1.00) // Violet
};

float3 getRainbowColor(float progress)
{
    float scaled = smoothstep(0,1,progress) * 7;
    int index = int(scaled);
    float fraction = frac(scaled);
    return lerp(rainbowColors[index], rainbowColors[index + 1], fraction);

}

float getRainbowAlpha(float2 uv)
{ 
    float y = clamp(1-pow(2*uv.y-1,4),0,1);
    float x = clamp(1 - pow(2 * uv.x - 1, 4), 0, 1);
    return y * x;
}

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, uTransformMatrix);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 uv = input.TextureCoordinates;
    return float4(getRainbowColor(uv.y) * getRainbowAlpha(uv), getRainbowAlpha(uv));

}

technique Technique1
{
    pass RainbowPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
