sampler s0;
float2 RadarCenter;
float RadarRadSq;	//Radar radius Quadriert
float RadarAngleCos;	//Cos des Winkels zu Vector (1 0)^T
float4 _PixelShader(float2 coord: TEXCOORD0) : COLOR0		//standart Shader
{
	return tex2D(s0, coord);
}

float4 _RadarShader(float2 coord: TEXCOORD0) : COLOR0
{
	float2 d;
	d.x = coord.x - RadarCenter.x;
	d.y = coord.y - RadarCenter.y;
	float2 dSq = d * d;
	if (d.x + d.y > 1 &&  RadarRadSq > 0)
		return float4(1, 0, 0, 1);
	return tex2D(s0, coord);
}

technique Ambient
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 _PixelShader();
	}
	pass Radar
	{
		PixelShader = compile ps_2_0 _RadarShader();
	}
}