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

//Made from the Spectral Color list on wikipedia - https://en.wikipedia.org/w/index.php?title=Spectral_color&oldid=1375573546#mwfw
//commented out some colors to make it feel more like real rainbow (the green was too powerful & so it needed rebalancing)
static const float3 WavelengthColors[29] =
{
    float3(0.0078f, 0.0000f, 0.0000f), // 740 nm
    //float3(0.0157f, 0.0000f, 0.0000f), // 730 nm
    float3(0.0314f, 0.0000f, 0.0039f), // 720 nm
    float3(0.0588f, 0.0000f, 0.0039f), // 710 nm
    float3(0.0941f, 0.0000f, 0.0078f), // 700 nm
    float3(0.1451f, 0.0000f, 0.0118f), // 690 nm
    float3(0.2157f, 0.0000f, 0.0275f), // 680 nm
    float3(0.2980f, 0.0000f, 0.0471f), // 670 nm
    float3(0.4078f, 0.0000f, 0.0784f), // 660 nm
    float3(0.5294f, 0.0000f, 0.1059f), // 650 nm
    float3(0.6588f, 0.0000f, 0.1333f), // 640 nm
    //float3(0.7882f, 0.0000f, 0.1412f), // 630 nm
    float3(0.9216f, 0.0000f, 0.1059f), // 620 nm
    float3(1.0000f, 0.1686f, 0.0000f), // 610 nm
    float3(0.9961f, 0.3647f, 0.0000f), // 600 nm
    float3(0.9373f, 0.5098f, 0.0000f), // 590 nm
    float3(0.8353f, 0.6275f, 0.0000f), // 580 nm
    float3(0.6941f, 0.7098f, 0.0000f), // 570 nm
    float3(0.5098f, 0.7686f, 0.0000f), // 560 nm
    //float3(0.2039f, 0.8000f, 0.0000f), // 550 nm
    float3(0.0000f, 0.7725f, 0.3137f), // 540 nm
    //float3(0.0000f, 0.7216f, 0.3961f), // 530 nm
    float3(0.0000f, 0.6471f, 0.4157f), // 520 nm
    //float3(0.0000f, 0.5412f, 0.3961f), // 510 nm
    float3(0.0000f, 0.4314f, 0.3647f), // 500 nm
    float3(0.0000f, 0.3451f, 0.3373f), // 490 nm
    //float3(0.0000f, 0.2902f, 0.3333f), // 480 nm
    //float3(0.0000f, 0.2588f, 0.3765f), // 470 nm
    //float3(0.0000f, 0.1686f, 0.6078f), // 460 nm
    float3(0.1961f, 0.0000f, 0.6431f), // 450 nm
    float3(0.2431f, 0.0000f, 0.5725f), // 440 nm
    float3(0.2314f, 0.0000f, 0.4824f), // 430 nm
    float3(0.1569f, 0.0000f, 0.3255f), // 420 nm
    float3(0.0784f, 0.0000f, 0.1804f), // 410 nm
    float3(0.0314f, 0.0000f, 0.0941f), // 400 nm
    float3(0.0078f, 0.0000f, 0.0353f), // 390 nm
    float3(0.0039f, 0.0000f, 0.0118f) // 380 nm
};

float3 getRainbowColor(float progress)
{
    float scaled = progress * 28;
    int index = int(scaled);
    float fraction = frac(scaled);
    return lerp(WavelengthColors[index], WavelengthColors[index + 1], fraction);

}

float getRainbowAlpha(float2 uv)
{ 
    float x = clamp(1 - pow(2 * uv.x - 1, 4), 0, 1);
    return x ;
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
    return float4(getRainbowColor(uv.y) * getRainbowAlpha(uv),  getRainbowAlpha(uv));

}

technique Technique1
{
    pass RainbowPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
