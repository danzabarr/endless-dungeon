Shader "Wall/Standard" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Metallic("Metallic", 2D) = "black" {}
		_Smoothness("Smoothness", Range(0,1)) = 0.5
		_MinDistance("Minimum Distance", float) = 2
		_MaxDistance("Maximum Distance", float) = 3
		
	}
		SubShader{
			Tags {"RenderType" = "Opaque"}
			LOD 200
			//ZWrite On
			//ZTest On
			Cull Back

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _Metallic;

			struct Input {
				float2 uv_MainTex;
				float3 worldPos;
				float4 screenPos;
			};

			half _Smoothness;
			float _MinDistance;
			float _MaxDistance;
			fixed4 _Color;
			float _Cutoff;

			uniform float3 _PlayerPosition;


			bool lies_between(float3 A, float3 B, float3 C)
			{
				float a = distance(B, C);
				float b = distance(C, A);
				float c = distance(A, B);
				return a * 2 + b * 2 >= c * 2 && a * 2 + c * 2 >= b * 2;
			}



			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o) {
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				// Metallic and smoothness come from slider variables
				
				fixed4 metal = tex2D(_Metallic, IN.uv_MainTex);
				o.Metallic = metal.r;
				o.Smoothness = metal.a * _Smoothness;

				// Fade the pixels as they get close to the camera (Start fading at _MaxDistance and become fully transparent at _MinDistance)


				//float4 playerScreenPos = ComputeScreenPos(float4(_PlayerPosition, 0));


				

				//float distanceFromCamera = distance(IN.worldPos, _WorldSpaceCameraPos);




				//bool isBetween = IN.worldPos.z > _WorldSpaceCameraPos.z && _PlayerPosition.z > IN.worldPos.z;

				//float fade = isBetween ? saturate((distanceFromCamera - _MinDistance) / _MaxDistance) : 1;






				//float distanceFromPlayer = distance(_PlayerPosition, IN.worldPos);

				//float fade = 1;// saturate((distanceFromPlayer - _MinDistance) / _MaxDistance);

				//	fade = saturate(fade + IN.worldPos.z - _PlayerPosition.z);
				/*
				float fade = 1 - saturate(distanceFromPlayerZ * 0.5);

				fade *= 1 - saturate(distanceFromPlayerX );

				else
					fade *= saturate(_PlayerPosition.z - IN.worldPos.z + 1);

					//fade *= saturate( _PlayerPosition.z - IN.worldPos.z + 1);

				fade = 1 - fade;



				*/
				//fade = saturate(IN.worldPos.z - _PlayerPosition.z) + saturate(_WorldSpaceCameraPos.z - IN.worldPos.z);
				//fade = saturate(fade + distanceFromPlayer * 0.1);

				//fade *= 1 - saturate( _PlayerPosition.z - IN.worldPos.z);

				discard;

				if (_PlayerPosition.z > IN.worldPos.z + 1.4)// && _PlayerPosition.z < IN.worldPos.z + 6.4)
				{
					//float2 screenPos = IN.screenPos.xy / IN.screenPos.w * 2 - 1;
					//screenPos.x *= _ScreenParams.x / _ScreenParams.y;

					//float distanceOnScreen = distance(screenPos, float3(0, 0, 0));

					//if (distanceOnScreen < .2)
						discard;
				}

			}
			ENDCG
		}
			FallBack "Diffuse"
}