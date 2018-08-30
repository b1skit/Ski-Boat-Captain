Shader "Unlit/RewindStaticShader"
{
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

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

			// Generates a pseudo random int in [0, 1]. This function is based on a modified version of this: https://gist.github.com/keijiro/ee7bc388272548396870
			float rand(float2 uv)
			{
				return frac(sin(dot(uv, float2(12.9898, 78.233)) * _Time * 1000) * 43758.5453);
			}
			
			v2f vert (appdata v)
			{
				v.vertex.y += (20 * rand(v.uv) + 20 * sin( (v.vertex.x + _Time) * 300) ); // Glitch the pixel positions
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col;
				col.x = round(rand(i.uv));
				col.y = round(rand(i.uv));
				col.z = round(rand(i.uv));
				col.a = rand(i.uv) * .5; // Soften the alpha
				return col;
			}

			ENDCG
		}
	}
}
