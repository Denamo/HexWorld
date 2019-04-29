#ifndef LIGHTWEIGHT_DUNQ_IMAGE_BASED_LIGHTING_INCLUDED
#define LIGHTWEIGHT_DUNQ_IMAGE_BASED_LIGHTING_INCLUDED

#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.lightweight/ShaderLibrary/Lighting.hlsl"

#include "DunQInput.hlsl"

// Based on GlossyEnvironmentReflection from Lighting.hlsl, but with mip level passed as a parameter
half3 SampleSpecularCubemap(half3 normal, half mip)
{
	half4 encodedIrradiance = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, normal, mip);

	if (encodedIrradiance.a == 0)
	{
		encodedIrradiance.rgb = 1.0h + dot(normal, half3(0, 1, 0)); // Show some even if the spec map is black.
	}

#if !defined(UNITY_USE_NATIVE_HDR)
	half3 irradiance = DecodeHDREnvironment(encodedIrradiance, unity_SpecCube0_HDR);
#else
	half3 irradiance = encodedIrradiance.rbg;
#endif
	return irradiance;
}

half3 FakeSubsurfaceScattering(half3 viewDirectionWS, DunQSurfaceData surfaceData)
{
	half oneMinusThickness = saturate(surfaceData.subSurfaceScatteringColor.a - surfaceData.thickness);
	half oneMinusThicknessSquared = oneMinusThickness * oneMinusThickness;

	half mip = PerceptualRoughnessToMipmapLevel(oneMinusThicknessSquared);
	half3 envBackDiffuse = SampleSpecularCubemap(-viewDirectionWS, mip);

	return envBackDiffuse * oneMinusThicknessSquared * surfaceData.subSurfaceScatteringColor.rgb;
}

half4 ImageLitPassFragment(Varyings input) : SV_Target
{
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

	DunQSurfaceData surfaceData;
	InitializeDunQSurfaceData(input.uv, surfaceData);

	InputData inputData;
	InitializeInputData(input, surfaceData.normalTS, inputData);

	half4 color = half4(0, 0, 0, _Color.a);
	
	half diffuseMip = UNITY_SPECCUBE_LOD_STEPS - 2; // Minus two to remove the bleed to back side from blurring and fix artifacts from using 512px cube map
	half specularMip = PerceptualRoughnessToMipmapLevel(saturate(1.0h - surfaceData.smoothness));

	half3 envDiffuse = SampleSpecularCubemap(inputData.normalWS, diffuseMip);
	
	half3 reflectionVector = reflect(-inputData.viewDirectionWS, inputData.normalWS);
	half3 envReflection = SampleSpecularCubemap(reflectionVector, specularMip);

	half nDotV = dot(inputData.normalWS, inputData.viewDirectionWS);
	half fresnelTerm = Pow4(saturate(1-nDotV));
	half oneMinusReflectivity = OneMinusReflectivityMetallic(surfaceData.metallic);

	half3 diffuse = surfaceData.albedo * oneMinusReflectivity;
	half3 specular = lerp(kDieletricSpec.rgb, surfaceData.albedo, surfaceData.metallic);
	
	half reflectivity = saturate(1.0 - oneMinusReflectivity + surfaceData.smoothness + fresnelTerm);

	color.rgb = diffuse * envDiffuse * surfaceData.occlusion;
	color.rgb += reflectivity * specular * envReflection * surfaceData.occlusion;

#ifdef _SUBSURFACESCATTERING
	color.rgb += FakeSubsurfaceScattering(inputData.viewDirectionWS, surfaceData);
#endif

	//color.rgb = MixFog(color.rgb, inputData.fogCoord);
	return color;
}

#endif // LIGHTWEIGHT_DUNQ_IMAGE_BASED_LIGHTING_INCLUDED
