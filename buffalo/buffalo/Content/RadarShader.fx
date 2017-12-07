float4x4 Wolrd;
float4x4 View;
float4x4 Projection;

float4 AmbientColor = float4(1.f, 1.f, 1.f, 1.f);
float4 AmbientIntensity = 0.5f;

struct VertexShaderInput
{
	float4 Position : POSITION0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
};

VertexShaderOutput _VertexShader(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	return output;
}

float4 _PixelShader(VertexShaderOutput input) : COLOR0
{
	return AmbientColor * AmbientIntensity;
}

technique Ambient
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 _VertexShader();
		_PixelShader = compile ps_2_0 _PixelShader();
	}
}