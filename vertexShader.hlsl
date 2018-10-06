struct VS_IN
{
    float2 Position : POSITION0;
    float2 Size : TEXCOORD;
    float4 Color : COLOR0;
};

struct GS_IN
{
    float2 Position : POSITION;
    float2 Size : TEXCOORD;
    float4 Color : COLOR;

};

GS_IN VSMain(VS_IN input)
{
    GS_IN output;

    output.Position = input.Position;
    output.Size = input.Size;
    output.Color = input.Color;


    return output;
}

