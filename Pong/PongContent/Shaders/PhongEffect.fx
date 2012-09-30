extern float4x4 Projection;
extern float2 Resolution;

uniform extern float3 LightDir;
uniform extern texture ScreenTexture;    

sampler ScreenSampler = sampler_state
{
	Texture = <ScreenTexture>;
	MipFilter = None;
};

void SpriteVertexShader(inout float4 color    : COLOR0,
						inout float2 texCoord : TEXCOORD0,
						inout float4 position : POSITION0)
{
	position = mul(position, Projection);
}

float4 PixelDraw(float4 color : COLOR0, float2 texCoord: TEXCOORD0) : COLOR
{
	float2 dir = texCoord - float2(0.5,0.5);
	clip(0.5 - length(dir));

	float3 plane = float3(dir.xy, 1 - dir.x * dir.x - dir.y * dir.y);
	float3 specplane = normalize(LightDir + float3(0,0,1));
	float spec = (dot(specplane, plane) - 0.9) * 8;
	spec = spec > 0 ? spec : 0;

	float diffuse = dot(plane, LightDir) * 0.7;

	float light = diffuse + spec + 0.2;
	color *= light;

	return float4(color.xyz,1);
}

technique Draw
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 SpriteVertexShader();
		PixelShader = compile ps_2_0 PixelDraw();
	}
}