﻿Shader "Wall/SouthSide"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader
		{
			//Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}
			LOD 200
			ZWrite Off
			//ZTest Less
			Cull Back

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			uniform float3 _PlayerPosition;

			struct Input
			{
				float2 uv_MainTex;
				float3 worldPos;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				/*
				float fade = 1;

				if (IN.worldPos.z < _PlayerPosition.z)
				{
					float distanceToPlayer = distance(_PlayerPosition.xz, IN.worldPos.xz);
					fade *= saturate(distanceToPlayer * distanceToPlayer * 0.001);
				}
				else if (IN.worldPos.z - 10 < _PlayerPosition.z)
				{
					fade = (IN.worldPos.z - _PlayerPosition.z) / 10;
				}


				o.Alpha = c.a * fade;
				*/
			}
			ENDCG
		}
			FallBack "Diffuse"
}
