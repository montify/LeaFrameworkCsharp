cbuffer perFrame
{
    float4x4 ProjMatrix;
    float scaleSizeX;
    float scaleSizeY;
};

struct GS_IN
{
    float2 Position : POSITION;
    float2 Size : TEXCOORD;
    float4 Color : COLOR;
};

struct PS_IN
{
    float4 Position : SV_POSITION0;
    float4 Color : COLOR;
    float2 UVCoordinate : TEXCOORD;
};

[maxvertexcount(4)]
void GSMain(point GS_IN input[1], inout TriangleStream<PS_IN> triStream)
{
    PS_IN output;

    float2 v1 = float2(input[0].Size.x + scaleSizeX, input[0].Size.y + scaleSizeY) + input[0].Position;
    output.Position = mul(float4(v1, 0, 1), ProjMatrix);
    output.Color = float4(1, 1, 1, 1);
    output.UVCoordinate = float2(1, 1);
    triStream.Append(output);

    v1 = float2(0, input[0].Size.y + scaleSizeY) + input[0].Position;
    output.Position = mul(float4(v1, 0, 1), ProjMatrix);
    output.Color = float4(0, 1, 0, 1);
    output.UVCoordinate = float2(0, 1);
    triStream.Append(output);

    v1 = float2(input[0].Size.x + scaleSizeX, 0) + input[0].Position;
    output.Position = mul(float4(v1, 0, 1), ProjMatrix);
    output.Color = float4(0, 0, 1, 1);
    output.UVCoordinate = float2(1, 0);
    triStream.Append(output);

    v1 = float2(0, 0) + input[0].Position;
    output.Position = mul(float4(v1, 0, 1), ProjMatrix);
    output.Color = float4(1, 0, 0, 1);
    output.UVCoordinate = float2(0, 0);
    triStream.Append(output);

    triStream.RestartStrip();


   

   
}