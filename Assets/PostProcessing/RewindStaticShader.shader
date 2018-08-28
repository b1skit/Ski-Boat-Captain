Shader "Unlit/RewindStaticShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			// Generates a pseudo random value in [0, 1]. Based heavily on this: https://gist.github.com/keijiro/ee7bc388272548396870
			float rounded_rand(float2 uv)
			{
				/*uv.x = (uv.x + _Time) % 1;
				uv.y = (uv.y + _Time) % 1;*/
				return round(frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453));
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				/*fixed4 col = tex2D(_MainTex, i.uv) * rounded_rand(i.uv);*/
				/*fixed4 col = tex2D(_MainTex, i.uv) * 0;*/

				fixed4 col;

				col.x = rounded_rand(i.uv);
				col.y = rounded_rand(i.uv);
				col.z = rounded_rand(i.uv);
				col.w = rounded_rand(i.uv);

				return col;
			}

			

			ENDCG
		}
	}
}
