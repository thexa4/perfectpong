extern float4x4 Projection;
extern float2 Resolution;
uniform extern texture HeightTexture;
uniform extern texture ScreenTexture;    

sampler ScreenSampler = sampler_state
{
	Texture = <ScreenTexture>;
	MipFilter = None;
};

sampler HeightSampler = sampler_state
{
	Texture = <HeightTexture>;
	MipFilter = None;
	Filter = Point;
};

void SpriteVertexShader(inout float4 color    : COLOR0,
						inout float2 texCoord : TEXCOORD0,
						inout float4 position : POSITION0)
{
	position = mul(position, Projection);
}

float4 PixelUpdateHeight(float2 texCoord: TEXCOORD0) : COLOR
{
	float velocity = tex2D(ScreenSampler, texCoord);
	float prevheight = tex2D(HeightSampler, texCoord);

	return float4(prevheight + velocity,0,0,1);
}

float4 PixelUpdateVelocity(float2 texCoord: TEXCOORD0) : COLOR
{
	float avg = tex2D(ScreenSampler, texCoord + Resolution * float2(0,1)) +
		tex2D(ScreenSampler, texCoord + Resolution * float2(0,-1)) +
		tex2D(ScreenSampler, texCoord + Resolution * float2(1,0)) +
		tex2D(ScreenSampler, texCoord + Resolution * float2(-1,0));
	avg /= 4;

	float height = tex2D(ScreenSampler, texCoord);

	return float4(avg - height,0,0,1);
}

float4 PixelChange(float2 texCoord: TEXCOORD0, float4 color : COLOR0) : COLOR
{
	return float4(color.x - color.y,0,0,1);
}

float4 PixelDraw(float2 texCoord: TEXCOORD0) : COLOR
{
	float self = tex2D(HeightSampler, texCoord);
	float dx = tex2D(HeightSampler, texCoord + Resolution * float2(1,0)) - self - tex2D(HeightSampler, texCoord + Resolution * float2(-1,0));
	float dy = tex2D(HeightSampler, texCoord + Resolution * float2(0,1)) - self - tex2D(HeightSampler, texCoord + Resolution * float2(0,-1));

	float4 bg = tex2D(ScreenSampler, texCoord + float2(dx, dy));

	return float4(bg.xyz,1);
}

technique UpdateHeight
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 SpriteVertexShader();
		PixelShader = compile ps_2_0 PixelUpdateHeight();
	}
}

technique UpdateVelocity
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 SpriteVertexShader();
		PixelShader = compile ps_2_0 PixelUpdateVelocity();
	}
}

technique Change
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 SpriteVertexShader();
		PixelShader = compile ps_2_0 PixelChange();
	}
}

technique Draw
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 SpriteVertexShader();
		PixelShader = compile ps_2_0 PixelDraw();
	}
}