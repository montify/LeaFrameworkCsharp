#pragma pack_matrix( row_major )

cbuffer perFrameBuffer
{
  //  float4x4 WorldViewProj;
	float4x4 World;
	float4x4 View;
	float4x4 Projection;
	float4x4 WVP;

};


struct VS_IN
{
	float3 Position : POSITION0;
	float4 Color : COLOR0;
};

struct PS_IN
{
	float4 Position : SV_POSITION0;	
	float4 Color : COLOR0;

};

PS_IN VSMain(VS_IN input)
{
    PS_IN output = (PS_IN) 0;
	float C = 0.001f;
	float Far = 50000;
	
	float4 worldPosition = mul(float4(input.Position, 1), World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.Position.z = log(C*output.Position.z + 1) / log(C*Far + 1) * output.Position.w;
    output.Color = input.Color;
	
    return output;
}


