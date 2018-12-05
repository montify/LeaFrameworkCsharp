struct VS_IN
{
    float2 Position : POSITION0;
    float2 Size : TEXCOORD;
    float4 Color : COLOR0;
    float2 offset : TEXCOORD1;
};

struct GS_IN
{
    float2 Position : POSITION;
    float2 Size : TEXCOORD;
    float4 Color : COLOR;
    float2 offset : TEXCOORD1;
};

GS_IN VSMain(VS_IN input)
{
    GS_IN output;

    output.Position = input.Position;
    output.Size = input.Size;
    output.Color = input.Color;
    output.offset = input.offset;

    return output;
}

