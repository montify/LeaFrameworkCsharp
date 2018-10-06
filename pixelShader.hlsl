Texture2D tex : register(t1);
SamplerState samp;


struct PS_IN
{
    float4 Position : SV_POSITION0;
    float4 Color : COLOR0;
    float2 UVCoordinate : TEXCOORD;
};

float4 PSMain(PS_IN input) : SV_Target
{
    float3 t = tex.Sample(samp, input.UVCoordinate, 0);

    return float4(t, 1);
}