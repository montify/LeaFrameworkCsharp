cbuffer CC
{
	float3 Color;
};

struct PS_IN
{
	float4 Position : SV_POSITION0;
	float4 Color : COLOR0;
	//float2 TexCoord : TEXCOORD0;
	//float2 Normal : NORMAL0;
};

float4 PSMain(PS_IN input) : SV_Target
{
	return float4(1,0,0,1);
}