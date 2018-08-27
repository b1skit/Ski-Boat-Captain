Shader "Hidden/RewindPostProcessingShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			static const float SPEED = 200.0f;
			static const float AMPLITUDE = 0.01f;
			static const float FREQUENCY = 20.0f;
			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 newUV;
				i.uv.x = (i.uv.x + (AMPLITUDE * sin( (FREQUENCY * i.uv.y + (SPEED * _Time) ) )) ) % 1;
				fixed4 col = tex2D(_MainTex, i.uv);

				return col;
			}
			ENDCG
		}
	}
}
