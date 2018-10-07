Texture2D tex : register(t0);
SamplerState samp;


struct PS_IN
{
    float4 Position : SV_POSITION0;
    float4 Color : COLOR0;
    float2 UVCoordinate : TEXCOORD;
    float2 offset : TEXCOORD1;
    float2 size : TEXCOORD2;
};

float4 PSMain(PS_IN input) : SV_Target
{
    int x = input.offset;

   

    float4 t = tex.Sample(samp, input.UVCoordinate);
    
    return t;
}