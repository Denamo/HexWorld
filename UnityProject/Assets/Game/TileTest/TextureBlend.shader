Shader "Custom/TextureBlend"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SecondTex("Albedo (RGB)", 2D) = "white" {}
		_ThirdTex("Albedo (RGB)", 2D) = "white" {}
		_FourthTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _SecondTex;
		sampler2D _ThirdTex;
		sampler2D _FourthTex;

        struct Input
        {
            float2 uv_MainTex;
			//float2 uv_blend;
			float4 color: Color; // Vertex color
		};

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

		float cube(float t) {
			return t * t * t;
		}

		float tween(float t) {
			if (t < 0.5) 
				return cube(t*2)*0.5;
			else
				return 1 - cube(1-(t-0.5)*2)*0.5;
		}

		static const fixed4 BASE_COLOR = fixed4(1, 1, 1, 1);

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			
			if (IN.color.r > 0) {
				c = lerp(c, tex2D(_SecondTex, IN.uv_MainTex), tween(IN.color.r));
			}

			if (IN.color.g > 0) {
				c = lerp(c, tex2D(_ThirdTex, IN.uv_MainTex), tween(IN.color.g));
			}

			if (IN.color.b > 0) {
				c = lerp(c, tex2D(_FourthTex, IN.uv_MainTex), tween(IN.color.b));
			}
				
			o.Albedo = c * _Color;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
