static const float PI = 3.14159265f;
sampler s0;
float2 RadarCenter;
float RadarRadSq;	//Radar radius Quadriert
float RadarAngle;
float4 _PixelShader(float2 coord: TEXCOORD0) : COLOR0		//standart Shader
{
	return tex2D(s0, coord);
}
float f(float x) { x = 1.f - x;  return  x*x; }	//fade out
float4 _RadarShader(float2 coord: TEXCOORD0, float2 pos : VPOS) : COLOR0
{
	float2 d = pos - RadarCenter;
	float2 dSq = d * d;
	if (dSq.x + dSq.y <= 1)
		return float4(0.f, 0.f, 1.f, 1.f);
	float angle = dSq.x / (dSq.x + dSq.y) * ( d.x > 0 ? 1.f : -1.f);
	angle = acos(angle);
	if (d.y > 0)	//wenn winkel > 180°
		angle = 2 * PI - angle;

	float delta = abs((angle > RadarAngle ? 2*PI : 0.f) - abs(angle - RadarAngle));	//entfernung, problem overflow der grad zahlen 
	float intensity = f(delta / (2.f*PI));
	return tex2D(s0, coord) * intensity;
}

technique Ambient
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 _PixelShader();
	}
	pass Radar
	{
		PixelShader = compile ps_3_0 _RadarShader();
	}
}