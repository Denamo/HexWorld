#ifndef LIGHTWEIGHT_DUNQ_LIT_INPUT_INCLUDED
#define LIGHTWEIGHT_DUNQ_LIT_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/SurfaceInput.hlsl"
//#include "Packages/com.unity.render-pipelines.lightweight/Shaders/LitInput.hlsl"

CBUFFER_START(UnityPerMaterial)
float4 _MainTex_ST;
half4 _Color;
half4 _SpecColor;
half4 _EmissionColor;
half _Cutoff;
half _Glossiness;
half _GlossMapScale;
half _Metallic;
half _BumpScale;
half _OcclusionStrength;
half4 _SubSurfaceScattringColor;
CBUFFER_END

TEXTURE2D(_MetallicSmoothnessThicknessMap);		SAMPLER(sampler_MetallicSmoothnessThicknessMap);

// From LitInput.hlsl, Not used for our shader but required for the LitForwardPass.hlsl include

/*
half3 albedo;
half3 specular;
half  metallic;
half  smoothness;
half3 normalTS;
half3 emission;
half  occlusion;
half  alpha;
*/

inline void InitializeStandardLitSurfaceData(float2 uv, out SurfaceData outSurfaceData)
{
	outSurfaceData.alpha = 1;
	outSurfaceData.albedo = half3(1.0h, 1.0h, 1.0h);
	outSurfaceData.metallic = 1.0h;
	outSurfaceData.specular = half3(1.0h, 1.0h, 1.0h);
	outSurfaceData.smoothness = 1.0h;
	outSurfaceData.normalTS = half3(1.0h, 1.0h, 1.0h);
	outSurfaceData.occlusion = 1.0h;
	outSurfaceData.emission = half3(1.0h, 1.0h, 1.0h);
}

#include "Packages/com.unity.render-pipelines.lightweight/Shaders/LitForwardPass.hlsl"

struct DunQSurfaceData : SurfaceData
{
	half	thickness;
	half4	subSurfaceScatteringColor;
};

void InitializeDunQSurfaceData(float2 uv, out DunQSurfaceData outSurfaceData)
{
#ifdef _ALBEDOOCCLUSIONMAP
	half4 albedoOcclusion = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
	outSurfaceData.albedo = albedoOcclusion.rgb;
	outSurfaceData.occlusion = albedoOcclusion.a;
#else
	outSurfaceData.albedo = _Color.rgb;
	outSurfaceData.occlusion = _Color.a;
#endif

#ifdef _METALLICSMOOTHNESSTHICKNESS
	half4 metallicSmoothnessThickness = SAMPLE_TEXTURE2D(_MetallicSmoothnessThicknessMap, sampler_MetallicSmoothnessThicknessMap, uv);

	outSurfaceData.metallic = metallicSmoothnessThickness.r;
	outSurfaceData.smoothness = metallicSmoothnessThickness.g;
	outSurfaceData.thickness = metallicSmoothnessThickness.b;
#else
	outSurfaceData.metallic = _Metallic;
	outSurfaceData.smoothness = _Glossiness;
	outSurfaceData.thickness = 1.0h;
#endif

	outSurfaceData.specular = half3(0.0h, 0.0h, 0.0h);

	half4 n = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, uv);
	outSurfaceData.normalTS = UnpackNormal(n);

	outSurfaceData.emission = 0;
	outSurfaceData.alpha = 1;
	outSurfaceData.subSurfaceScatteringColor = _SubSurfaceScattringColor;
}

#endif // LIGHTWEIGHT_DUNQ_LIT_INPUT_INCLUDED
