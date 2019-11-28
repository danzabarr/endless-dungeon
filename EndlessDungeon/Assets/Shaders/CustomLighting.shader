Shader "Custom/CustomLighting"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Metallic("Metallic", 2D) = "black" {}
		_Smoothness("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

			CGPROGRAM
			#pragma surface surf SimpleSpecular fullforwardshadows

			half4 LightingSimpleSpecular(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
				half3 h = normalize(lightDir + viewDir);

				half diff = max(0, dot(s.Normal, lightDir));

				float nh = max(0, dot(s.Normal, h));
				float spec = pow(nh, 48.0);

				half4 c;
				c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
				c.a = s.Alpha;
				return c;
			}

			struct Input {
				float2 uv_MainTex;
			};

			sampler2D _MainTex;
			sampler2D _Metallic;

			float _Smoothness;

			void surf(Input IN, inout SurfaceOutput o) {
				o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
				fixed4 metal = tex2D(_Metallic, IN.uv_MainTex);
				o.Metallic = metal.r;
				o.Smoothness = metal.a * _Smoothness;
			}
			ENDCG
    }
    FallBack "Diffuse"
}
