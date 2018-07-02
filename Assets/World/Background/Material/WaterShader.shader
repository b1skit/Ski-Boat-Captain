Shader "Custom/WaterShader" {
	Properties {
		//_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_FrothTex ("Froth Texture (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		//_TimeScaleFactor ("TimeScaleFactor", Range(0,1)) = 0.4
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _FrothTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_FrothTex;
		};

		half _Glossiness;
		half _Metallic;
		//fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float speed = 2.0;
			float scale = 16.0;

			float timeScaleX = speed * _Time + IN.uv_MainTex.x;
			float timeScaleY = speed * _Time + IN.uv_MainTex.y;
			float newX = IN.uv_MainTex.x + (sin(timeScaleX) + sin( (1.3 * timeScaleX + 56)) + sin(2.6 * timeScaleX + 72) + sin(2.1 * timeScaleX + 2.79) ) / scale;
			float newY = IN.uv_MainTex.y + (sin(timeScaleY) + sin((1.4 * timeScaleY + 46)) + sin(4.3 * timeScaleY + 36) + sin(3.12 * timeScaleY + 40.79)) / scale;
			fixed4 c = tex2D(_MainTex, float2(newY, newX));

			float timeScaleFrothX = speed * _Time + IN.uv_FrothTex.x;
			float timeScaleFrothY = speed * _Time + IN.uv_FrothTex.y;
			float newFrothX = IN.uv_FrothTex.x + (sin(timeScaleFrothX) + sin((1.6 * timeScaleFrothX + 36)) + sin(2.4 * timeScaleFrothX + 22) + sin(2.17 * timeScaleFrothX + 4.83)) / scale;
			float newFrothY = IN.uv_FrothTex.y + (sin(timeScaleFrothY) + sin((1.5 * timeScaleFrothX + 46)) + sin(3.4 * timeScaleFrothX + 42) + sin(2.99 * timeScaleFrothX + 30.12)) / scale;
			fixed4 froth = tex2D(_FrothTex, float2(newFrothX, newFrothY));

			float timeScaleFrothAlphaX = speed * _Time + IN.uv_FrothTex.x;
			float timeScaleFrothAlphaY = speed * _Time + IN.uv_FrothTex.y;
			float newFrothAlphaX = IN.uv_FrothTex.x + 0.5 + (sin(timeScaleFrothAlphaX) + sin((2.3 * timeScaleFrothAlphaX + 22)) + sin(2.85 * timeScaleFrothAlphaX + 13) + sin(1.98 * timeScaleFrothAlphaX + 3.54)) / scale;
			float newFrothAlphaY = IN.uv_FrothTex.y + 0.5 + (sin(timeScaleFrothAlphaY) + sin((1.72 * timeScaleFrothAlphaY + 35)) + sin(1.94 * timeScaleFrothAlphaY + 6) + sin(1.33 * timeScaleFrothAlphaY + 12.20)) / scale;
			fixed4 frothAlpha = tex2D(_FrothTex, float2(newFrothAlphaX, newFrothAlphaY));
			
			o.Albedo = c.rgb + (froth.rgb * 0.1) - (frothAlpha * 0.055);

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			
		} // End SubShader
		ENDCG
	}
	FallBack "Diffuse"
}
