﻿Shader "Unlit/Dissolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_NoiseTex("Noise_Texture", 2D) = "white" {}
		_Threshold ("Threshold" , range(0,1)) = 0
		_Glow ("Glow", Color) = (1, 0.5, 0.5, 1)
		_EmissionAmount("Emission amount", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Off
		CGPROGRAM
		#pragma surface surf Lambert
		#pragma target 3.0


		struct Input
		{
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		sampler2D _NoiseTex;
		float _Threshold;
		fixed4 _Glow;
		float _EmissionAmount;

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
			if (col.a < 0.01f)
				discard;
			if (tex2D(_NoiseTex, IN.uv_MainTex).r < _Threshold)
				discard;
			else if (tex2D(_NoiseTex, IN.uv_MainTex).r < _Threshold + 0.1f)
				o.Emission = _Glow * _EmissionAmount;
            o.Albedo = col.rgb;
            o.Alpha = col.a;
        }
        ENDCG
    }
	FallBack "Diffuse"
}
