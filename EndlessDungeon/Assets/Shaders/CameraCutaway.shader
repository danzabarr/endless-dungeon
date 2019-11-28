// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

Shader "Custom/Opaque"
{
	Properties
	{
		_MainTex("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Metallic("Metallic", 2D) = "black" {}
		_TranslucentGain("Translucent Gain", float) = 0
	}

	CGINCLUDE

	#include "UnityCG.cginc"
	#include "Lighting.cginc"
	#include "AutoLight.cginc"
	#include "UnityPBSLighting.cginc"

	sampler2D _MainTex;
	sampler2D _Normal;
	sampler2D _Metallic;

	float4 _MainTex_ST;
	float4 _Normal_ST;
	float4 _Metallic_ST;
	float _TranslucentGain;
	/*
	UnityLight CreateLight (float3 worldPos, half3 normal) {
		UnityLight light;
		//light.dir = normalize(_WorldSpaceLightPos0.xyz - worldPos);
		light.dir = normalize(UnityWorldSpaceLightDir(worldPos.xyz));
		UNITY_LIGHT_ATTENUATION(attenuation, 0, worldPos);
		light.color = _LightColor0.rgb * attenuation;
		light.ndotl = DotClamped(normal, light.dir);
		return light;
	}
	*/

	struct vertexOutput {
		float4 pos : SV_POSITION;
		float3 worldPos : TEXCOORD0;
		half3 worldNormal : TEXCOORD1;
		float2 uv : TEXCOORD2;

		half3 tspace0 : TEXCOORD4; // tangent.x, bitangent.x, normal.x
		half3 tspace1 : TEXCOORD5; // tangent.y, bitangent.y, normal.y
		half3 tspace2 : TEXCOORD6; // tangent.z, bitangent.z, normal.z

		half3 viewDir : TEXCOORD7;
		half3 normal : TEXCOORD8;

		SHADOW_COORDS(9)
		UNITY_FOG_COORDS(10)
	};

	vertexOutput vert(appdata_full input) {
		vertexOutput output;

		output.pos = UnityObjectToClipPos(input.vertex);
		output.uv = input.texcoord;
		output.viewDir = normalize(ObjSpaceViewDir(input.vertex));
		output.normal = input.normal;


		half3 wNormal = UnityObjectToWorldNormal(input.normal);
		half3 wTangent = UnityObjectToWorldDir(input.tangent.xyz);
		// compute bitangent from cross product of normal and tangent
		half tangentSign = input.tangent.w * unity_WorldTransformParams.w;
		half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
		// output the tangent space matrix
		output.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
		output.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
		output.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);

		output.worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
		output.worldNormal = wNormal;


		TRANSFER_SHADOW(output);
		UNITY_TRANSFER_FOG(output, output.pos);

		return output;
	}

	half4 frag(vertexOutput input) : SV_Target{
		//Calculate light direction

		UNITY_SETUP_INSTANCE_ID(input);

		half3 lightDir = normalize(UnityWorldSpaceLightDir(input.worldPos.xyz));
		half3 worldViewDir = normalize(UnityWorldSpaceViewDir(input.worldPos));

		half4 metallicMap = tex2D(_Metallic, TRANSFORM_TEX(input.uv, _Metallic));

		half metallic = metallicMap.r;
		half smoothness = metallicMap.a;

		half specular = pow(max(0.0, dot(reflect(-lightDir, input.worldNormal), input.viewDir)), max(0.01, smoothness * 50)) * metallic;

		half3 albedo = tex2D(_MainTex, TRANSFORM_TEX(input.uv, _MainTex)).rgb;

		half3 normal = UnpackNormal(tex2D(_Normal, TRANSFORM_TEX(input.uv, _Normal)));

		half3 bumpedNormal = half3(dot(input.tspace0, normal), dot(input.tspace1, normal), dot(input.tspace2, normal));

		half3 bumpedDiffuse = saturate(dot(lightDir.xyz, bumpedNormal) + _TranslucentGain);

		half bumpedSpecular = saturate(pow(max(0.0, dot(reflect(-lightDir, bumpedNormal), input.viewDir)), max(0.01, smoothness * 50)) * metallic);


		//Get shadow attenuation
		float atten = SHADOW_ATTENUATION(input);

		float3 col = albedo;

		//col *= max((bumpedDiffuse + bumpedSpecular) * atten * _LightColor0, UNITY_LIGHTMODEL_AMBIENT) * 2;

		col *= (bumpedDiffuse + bumpedSpecular) * atten * _LightColor0;

		UNITY_APPLY_FOG(input.fogCoord, col);

		col = clamp(col, 0, 5);

		return half4(col, 1);
	}

	ENDCG

	SubShader
	{

		Tags {"RenderType" = "Opaque"}

		Pass
		{
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma target 3.0

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog

			ENDCG
		}

		Pass
		{
			Tags { "LightMode" = "ForwardAdd" }

			Blend One One
			ZWrite Off

			CGPROGRAM

			#pragma target 3.0

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

			#define POINT

			#include "AutoLight.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"

			struct VertexData {
				float4 position : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float4 tangent : TANGENT;
				
			};

			struct Interpolators {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;

				half3 tspace0 : TEXCOORD4; // tangent.x, bitangent.x, normal.x
				half3 tspace1 : TEXCOORD5; // tangent.y, bitangent.y, normal.y
				half3 tspace2 : TEXCOORD6; // tangent.z, bitangent.z, normal.z

				float3 viewDir : TEXCOORD7;

				SHADOW_COORDS(9)

			};

			Interpolators MyVertexProgram(VertexData v) {
				Interpolators i;
				i.position = UnityObjectToClipPos(v.position);
				i.worldPos = mul(unity_ObjectToWorld, v.position);
				i.normal = UnityObjectToWorldNormal(v.normal);
				i.uv = TRANSFORM_TEX(v.uv, _MainTex);


				half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
				// compute bitangent from cross product of normal and tangent
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 wBitangent = cross(i.normal, wTangent) * tangentSign;
				// output the tangent space matrix
				i.tspace0 = half3(wTangent.x, wBitangent.x, i.normal.x);
				i.tspace1 = half3(wTangent.y, wBitangent.y, i.normal.y);
				i.tspace2 = half3(wTangent.z, wBitangent.z, i.normal.z);
				i.viewDir = normalize(ObjSpaceViewDir(v.position));
				TRANSFER_SHADOW(i);
				return i;
			}

			UnityLight CreateLight(Interpolators input, float3 worldPos, float3 normal) {
				UnityLight light;
				light.dir = normalize(_WorldSpaceLightPos0.xyz - worldPos);

				float d = length(_WorldSpaceLightPos0.xyz - worldPos);
				float range = 5;
				float normalizedDist = d / range;
				//float attenuation = saturate(1.0 / (1.0 + 25.0 * normalizedDist * normalizedDist) * saturate((1 - normalizedDist) * 5.0));

				float DistanceSquared = normalizedDist * normalizedDist;
				float QuadraticAttenuation = 25.0;

				float attenuation = saturate(1.0 / (1.0 + (DistanceSquared * QuadraticAttenuation)) * (1 - (DistanceSquared / range)));

				light.color = _LightColor0.rgb * attenuation;
				return light;
			}

			float4 MyFragmentProgram(Interpolators i) : SV_TARGET {
				i.normal = normalize(i.normal);

				float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

				float3 albedo = tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex)).rgb;

				float4 metallicMap = tex2D(_Metallic, TRANSFORM_TEX(i.uv, _Metallic));
				float metallic = metallicMap.r;
				float smoothness = metallicMap.a;

				half3 normal = UnpackNormal(tex2D(_Normal, TRANSFORM_TEX(i.uv, _Normal)));

				half3 bumpedNormal = half3(dot(i.tspace0, normal), dot(i.tspace1, normal), dot(i.tspace2, normal));
						 


				float3 specularTint;
				float oneMinusReflectivity;
				albedo = DiffuseAndSpecularFromMetallic(
				albedo, metallic, specularTint, oneMinusReflectivity
				);

				UnityIndirect indirectLight;
				indirectLight.diffuse = 0;
				indirectLight.specular = 0;

				float4 result = UNITY_BRDF_PBS(
					albedo, specularTint,
					oneMinusReflectivity, smoothness,
					bumpedNormal, viewDir,
					CreateLight(i, i.worldPos, bumpedNormal), indirectLight
				);

				float atten = SHADOW_ATTENUATION(i);

				result = clamp(result, 0, 3);

				result *= atten;

				return result;
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}

