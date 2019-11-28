Shader "Outline/WhiteMaker" {
	Properties
	{
		_MainTex("Main Texture 1", 2D) = "white" {}
	}

		SubShader
	{
		Tags{ "Queue" = "Transparent" }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			half _SampleTime;

			v2f_img vert(appdata_base v)
			{
				v2f_img o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}
			float4 frag(v2f_img i) : COLOR
			{
				// Surface
				float4 main = tex2D(_MainTex, i.uv);
				if (main.a == 0)
				{
					return float4(0, 0, 0, 0);
				}
				else
				{

					return float4(1, 1, 1, 1);
				}
				return main;
			}
				ENDCG
			}
	}
}