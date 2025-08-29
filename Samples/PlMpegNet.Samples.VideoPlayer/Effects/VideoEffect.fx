#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D YTex;
sampler2D YTexSampler = sampler_state
{
    Texture = <YTex>;
};
Texture2D CbTex;
sampler2D CbTexSampler = sampler_state
{
    Texture = <CbTex>;
};
Texture2D CrTex;
sampler2D CrTexSampler = sampler_state
{
    Texture = <CrTex>;
};

float4 VideoPS(float2 texCoord : TEXCOORD0) : COLOR
{
    float y = tex2D(YTexSampler, texCoord).r;
    float cb = tex2D(CbTexSampler, texCoord).r;
    float cr = tex2D(CrTexSampler, texCoord).r;

    float4x4 rec601 = float4x4(
		1.16438, 0.00000, 1.59603, -0.87079,
		1.16438, -0.39176, -0.81297, 0.52959,
		1.16438, 2.01723, 0.00000, -1.08139,
		0, 0, 0, 1
    );

    return mul(rec601, float4(y.r, cb.r, cr.r, 1.0));
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL
            VideoPS();
    }
};