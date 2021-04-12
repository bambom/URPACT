// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "GOD/BossSkin"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("MainColor", Color) = (0.0, 0.0, 0.0, 1.0)
		_PassThroughColor ("PassThrough Color", Color) = (0.38,0.86,0.74,0)
		_Emission ("Emmisive Color", Color) = (0.0, 0.0, 0.0, 1.0)
		_TintColor ("TintColor", Color) = (1.0, 0.0, 0.0, 1.0)
		_TintExponent ("TintExponent", Range(0,10)) = 3.5
		_AlphaCutOff ("AlphaCutOff", Range(0,1)) = 0.5
	}
	SubShader
		{
		Tags { "RenderType"="Opaque" }
		Pass
		{
			Lighting Off
			ZTest Greater
			ZWrite Off
			Color [_PassThroughColor]
		}
		
		Pass
		{
			Tags { "RenderType"="Opaque" }
			LOD 200
			AlphaTest Greater [_AlphaCutOff]
			Cull Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it does not contain a surface program or both vertex and fragment programs.
			//#pragma exclude_renderers gles
			#include "UnityCG.cginc"
			struct appdata
		{
				float4 position : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : POSITION;
				float4 color : COLOR;
				float3 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			fixed4 _Color;
			fixed4 _Emission;
			fixed4 _TintColor;
			half _TintExponent;

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.position);
				o.texcoord.xy = v.texcoord;
				o.texcoord.z = pow(1 - dot(normalize(ObjSpaceViewDir(v.position)), v.normal), _TintExponent);
				return o;
			}

			float4 frag(v2f IN) : COLOR {
				half4 texle = tex2D(_MainTex, IN.texcoord.xy);
				half3 color = texle.rgb * _Emission.rgb;
				color += _Color.rgb + _TintColor.rgb * IN.texcoord.z * abs(_SinTime.w);
				return half4(color, texle.a);
			}
			ENDCG
		}
	}
}
