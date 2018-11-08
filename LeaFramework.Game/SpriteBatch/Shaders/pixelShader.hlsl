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
    float4 t = tex.Sample(samp, input.UVCoordinate);
    
    return float4(input.Color.xyz, 1);
}