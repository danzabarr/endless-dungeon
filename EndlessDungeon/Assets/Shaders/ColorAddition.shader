Shader "Custom/ColorAddition"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)
		_Metallic ("Base Metallic", Range(0,1)) = 0
        _Glossiness ("Base Smoothness", Range(0,1)) = 0.5
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MetallicTex ("Metallic", 2D) = "black" {}
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
        sampler2D _MetallicTex;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MetallicTex;
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

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c = _Color * _Color.a + tex * (1.0 - _Color.a);

			fixed4 met = tex2D(_MetallicTex, IN.uv_MetallicTex);
			fixed m = _Metallic * _Color.a + met.r * (1.0 - _Color.a);
			fixed s = _Glossiness * _Color.a + met.a * (1.0 - _Color.a);

            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = m;
            o.Smoothness = s;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
