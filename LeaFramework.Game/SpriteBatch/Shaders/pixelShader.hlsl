Texture2D tex : register(t0);
SamplerState samp;


struct PS_IN
{
    float4 Position : SV_POSITION0;
    float4 Color : COLOR0;
    float2 UVCoordinate : TEXCOORD;
    int TextureID : BLENDINDICES;
};

float4 PSMain(PS_IN input) : SV_Target
{
    int x = input.TextureID;
    float3 t = tex.Sample(samp, input.UVCoordinate, 0);

    return float4(t,1);
}