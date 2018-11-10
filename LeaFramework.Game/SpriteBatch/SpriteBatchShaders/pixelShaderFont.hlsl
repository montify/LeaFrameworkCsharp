Texture2D tex : register(t0);
SamplerState samp;


struct PS_IN
{
    float4 Position : SV_POSITION0;
    float4 Color : COLOR0;
    float2 UVCoordinate : TEXCOORD;

};

float4 PSMain(PS_IN input) : SV_Target
{

    return tex.Sample(samp, input.UVCoordinate) * input.Color;
}