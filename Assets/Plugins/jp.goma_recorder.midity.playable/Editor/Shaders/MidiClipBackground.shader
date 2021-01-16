// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "jp.goma_recorder.Midity.Playable/ClipBackground"
{
	Properties
	{
		[NoScaleOffset]
		_MainTex("Texture", 2D) = "white" {}
		_OffsetX("OffsetX", Float) = 0.0
		_RepeatX("RepeatX", Float) = 0.0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }

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

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _OffsetX;
				float _RepeatX;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					//オフセット + UV値を(0~1)でRepeat
					float2 uv = float2(abs(fmod(i.uv.x * _RepeatX + _OffsetX, 1.0)), i.uv.y);

					fixed4 col = tex2D(_MainTex, uv);
					return col;
				}
				ENDCG
			}
		}
}
