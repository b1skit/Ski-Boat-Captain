Shader "Custom/WaterShader" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_FrothTex ("Froth Texture (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
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

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float speed = 8;
			float scale = 30;

			// Water background:
			float timeScaleX = IN.uv_MainTex.x + (speed * _Time);
			float timeScaleY = IN.uv_MainTex.y + (speed * _Time);
			float newX = IN.uv_MainTex.x + (sin(timeScaleY) + sin((0.4 * timeScaleY + 46)) + sin(2.3 * timeScaleY + 36) + sin(1.12 * timeScaleY + 40.79)) / scale;
			float newY = IN.uv_MainTex.y + (sin(timeScaleX) + sin((1.3 * timeScaleX + 56)) + sin(2.6 * timeScaleX + 72) + sin(2.1 * timeScaleX + 2.79)) / scale;
			fixed4 c = tex2D(_MainTex, float2(newY, newX));

			// Generate white foam:
			float timeScaleFrothX = IN.uv_FrothTex.x + (speed * _Time);
			float timeScaleFrothY = IN.uv_FrothTex.y + (speed * _Time);
			float newFrothX = IN.uv_FrothTex.x + (sin(timeScaleFrothY) + sin((1.52 * timeScaleFrothY + 35)) + sin(1.74 * timeScaleFrothY + 6) + sin(1.13 * timeScaleFrothY + 12.20)) / scale;
			float newFrothY = IN.uv_FrothTex.y + (sin(timeScaleFrothX) + sin((1.3 * timeScaleFrothX + 22)) + sin(1.85 * timeScaleFrothX + 13) + sin(0.98 * timeScaleFrothX + 3.54)) / scale;
			fixed4 froth = tex2D(_FrothTex, float2(newFrothX, newFrothY));

			// Generate shadows lines:
			float timeScaleshadowsX = IN.uv_FrothTex.x + (speed * _Time);
			float timeScaleshadowsY = IN.uv_FrothTex.y + (speed * _Time);
			float newshadowsX = IN.uv_FrothTex.x + 0.5 + (sin(timeScaleshadowsY) + sin((1.72 * timeScaleshadowsY + 35)) + sin(1.94 * timeScaleshadowsY + 6) + sin(1.33 * timeScaleshadowsY + 12.20)) / scale;
			float newshadowsY = IN.uv_FrothTex.y + 0.5 + (sin(timeScaleshadowsX) + sin((2.3 * timeScaleshadowsX + 22)) + sin(2.85 * timeScaleshadowsX + 13) + sin(1.98 * timeScaleshadowsX + 3.54)) / scale;
			fixed4 shadows = tex2D(_FrothTex, float2(newshadowsX, newshadowsY));
			
			// Combine the maps:
			o.Albedo = c.rgb + (froth.rgb * 0.1) - (shadows * 0.055);

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			
		} // End SubShader
		ENDCG
	}
	FallBack "Diffuse"
}
