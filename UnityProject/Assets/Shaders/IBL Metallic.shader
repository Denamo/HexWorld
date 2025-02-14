Shader "DunQ/IBL Metallic"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Albedo", 2D) = "white" {}

		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0

		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
		_MetallicSmoothnessThicknessMap("Metallic", 2D) = "white" {}

		_SpecGlossMap("Specular", 2D) = "white" {}

		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

		_BumpMap("Normal Map", 2D) = "bump" {}

		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}

		_SubSurfaceScattringColor("Sub Surface Color (RGB), Cut off (A)", Color) = (0, 0, 0, 0) // color

		// Blending state
		[HideInInspector] _Surface("__surface", Float) = 0.0
		[HideInInspector] _Blend("__blend", Float) = 0.0
		[HideInInspector] _AlphaClip("__clip", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
		[HideInInspector] _Cull("__cull", Float) = 2.0

		_ReceiveShadows("Receive Shadows", Float) = 1.0
	}

		SubShader
		{
			// Lightweight Pipeline tag is required. If Lightweight render pipeline is not set in the graphics settings
			// this Subshader will fail. One can add a subshader below or fallback to Standard built-in to make this
			// material work with both Lightweight Render Pipeline and Builtin Unity Pipeline
			Tags{"RenderType" = "Opaque" "RenderPipeline" = "LightweightPipeline" "IgnoreProjector" = "True"}
			LOD 300

			// ------------------------------------------------------------------
			//  Forward pass. Shades all light in a single pass. GI + emission + Fog
			Pass
			{
				// Lightmode matches the ShaderPassName set in LightweightRenderPipeline.cs. SRPDefaultUnlit and passes with
				// no LightMode tag are also rendered by Lightweight Render Pipeline
				Name "ForwardLit"
				Tags{"LightMode" = "LightweightForward"}

				Blend[_SrcBlend][_DstBlend]
				ZWrite On
				Cull[_Cull]

				//ZWrite[_ZWrite]

				HLSLPROGRAM
				// Required to compile gles 2.0 with standard SRP library
				// All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
				#pragma prefer_hlslcc gles
				#pragma exclude_renderers d3d11_9x
				#pragma target 3.5

				// -------------------------------------
				// Material Keywords
				#pragma shader_feature _ALBEDOOCCLUSIONMAP
				#pragma shader_feature _NORMALMAP
				#pragma shader_feature _ALPHATEST_ON
				//#pragma shader_feature _ALPHAPREMULTIPLY_ON
				//#pragma shader_feature _EMISSION
				#pragma shader_feature _METALLICSMOOTHNESSTHICKNESS
				#pragma shader_feature _SUBSURFACESCATTERING

				//#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
				//#pragma shader_feature _GLOSSYREFLECTIONS_OFF
				#pragma shader_feature _SPECULAR_SETUP
				#pragma shader_feature _RECEIVE_SHADOWS_OFF

				// -------------------------------------
				// Lightweight Pipeline keywords
				//#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
				//#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
				//#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
				//#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
				//#pragma multi_compile _ _SHADOWS_SOFT
				//#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

				// -------------------------------------
				// Unity defined keywords
				//#pragma multi_compile _ DIRLIGHTMAP_COMBINED
				//#pragma multi_compile _ LIGHTMAP_ON
				//#pragma multi_compile_fog

				//--------------------------------------
				// GPU Instancing
				#pragma multi_compile_instancing

				#pragma vertex LitPassVertex
				#pragma fragment ImageLitPassFragment

				#include "DunQInput.hlsl"
				#include "DunQImageBaseLighting.hlsl"

				ENDHLSL
			}
		}

		FallBack "Hidden/InternalErrorShader"
		//CustomEditor "Game.Editor.ImageBasedLightingShaderGUI"
}
