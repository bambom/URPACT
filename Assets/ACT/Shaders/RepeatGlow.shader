Shader "GOD/RepeatGlow" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_GlowColor ("Glow Color", Color) = (0.0, 0.0, 0.0, 1.0)
	}
	SubShader {
		CGPROGRAM
		#pragma surface surf Lambert
		sampler2D _MainTex;
		fixed4 _GlowColor;
		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Emission = c + abs(_SinTime.w) * _GlowColor;
		}
		ENDCG
	}
}
