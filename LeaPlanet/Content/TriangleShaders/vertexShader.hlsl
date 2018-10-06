//#pragma pack_matrix( row_major )

cbuffer TestX
{
    float4x4 ViewProj;
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

    float4 worldPos = mul(float4(input.Position, 1), ViewProj);
   

    output.Position = worldPos;
    output.Color = input.Color;
   
    return output;
}


