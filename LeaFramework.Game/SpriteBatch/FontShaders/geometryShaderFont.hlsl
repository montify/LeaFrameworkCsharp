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
    float2 offset : TEXCOORD1;
};

struct PS_IN
{
    float4 Position : SV_POSITION0;
    float4 Color : COLOR0;
    float2 UVCoordinate : TEXCOORD;
    float2 offset : TEXCOORD1;
    float2 size : TEXCOORD2;
};

[maxvertexcount(4)]
void GSMain(point GS_IN input[1], inout TriangleStream<PS_IN> triStream)
{
    PS_IN output;

    float2 sizeN = input[0].Size/512;

    float2 v1 = float2(input[0].Size.x + scaleSizeX, input[0].Size.y + scaleSizeY) + input[0].Position;
    output.Position = mul(float4(v1, 0, 1), ProjMatrix);
    output.Color = input[0].Color;
  //  output.UVCoordinate = float2(1, 1);
    output.UVCoordinate = float2(input[0].offset.x + sizeN.x, input[0].offset.y + (input[0].Size.y / 512));
    output.offset = input[0].offset;
    output.size = input[0].Size;
    triStream.Append(output);

    v1 = float2(0, input[0].Size.y + scaleSizeY) + input[0].Position;
    output.Position = mul(float4(v1, 0, 1), ProjMatrix);
    output.Color = input[0].Color;
  //  output.UVCoordinate = float2(0, 1);
    output.UVCoordinate = float2(input[0].offset.x, input[0].offset.y + sizeN.y);
    output.offset = input[0].offset;
    output.size = input[0].Size;
    triStream.Append(output);

    v1 = float2(input[0].Size.x + scaleSizeX, 0) + input[0].Position;
    output.Position = mul(float4(v1, 0, 1), ProjMatrix);
    output.Color = input[0].Color;
   // output.UVCoordinate = float2(1, 0);
    output.UVCoordinate = float2(input[0].offset.x + sizeN.x, input[0].offset.y);
    output.offset = input[0].offset;
    output.size = input[0].Size;
    triStream.Append(output);

    v1 = float2(0, 0) + input[0].Position;
    output.Position = mul(float4(v1, 0, 1), ProjMatrix);
    output.Color = input[0].Color;
   // output.UVCoordinate = float2(0, 0);
    output.UVCoordinate = float2(input[0].offset.x, input[0].offset.y);
    output.offset = input[0].offset;
    output.size = input[0].Size;
    triStream.Append(output);

    triStream.RestartStrip();

}