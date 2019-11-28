// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/CooldownOverlay"
{
	Properties
	{
		_Overlay("Overlay", Color) = (0.2, 0.2, 0.2, 1)
		_Tint("Tint", Color) = (1,1,1,1)
		_MainTex("Icon", 2D) = "clear" {}
		_BkgdTex("Background", 2D) = "clear" {}
		_Fraction("Fraction", Range(0, 1)) = 1
		[Toggle(INVERT)]
		_Invert("Invert", Float) = 0
	}
	SubShader
	{
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature INVERT
			sampler2D _BkgdTex;
			sampler2D _MainTex;
			fixed4 _Overlay;
			fixed4 _Tint;
			fixed _Fraction;
		

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			static const float PI = 3.14159265359f;
			static const float TAU = 6.28318530718f;

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;

				float angle = (PI - atan2(0.5 - uv.y, 0.5 - uv.x) + PI / 2) % TAU;

				half4 col = tex2D(_BkgdTex, uv);
				half4 icon = tex2D(_MainTex, uv) * _Tint;
				

				#ifdef INVERT
				if (angle <= _Fraction * TAU)
				#else
				if (angle > _Fraction * TAU)
				#endif
					icon *= _Overlay * _Overlay.a;


				return icon;
				col = icon * icon.a + col * (1 - icon.a) * col.a;


				col *= col.a;


				return col;
			}
			ENDCG
		}
	}
}
