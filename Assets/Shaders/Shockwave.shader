Shader "Custom/Shockwave"
{
	HLSLINCLUDE
#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	float _Radius;
	float _Thickness;
	float _CenterX;
	float _CenterY;

	float4 Frag(VaryingsDefault i) : SV_Target
	{
		float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
		float dist = length(float2(i.texcoord.x - _CenterX, i.texcoord.y - _CenterY));
		float displ = (dist - _Radius) / _Thickness;

		//displ *= _DisplacementAmount;
		displ /= 100;

		float4 displ_col = col;
		if (dist >= _Radius && dist <= _Radius + _Thickness)
		{
			displ_col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + float2(displ, displ));
			displ_col.rgb += float4(displ * 2, 0, 0, 1);
			//displ_col.rgb = 1 - displ_col.rgb;
		}
		return displ_col;
	}

	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass{
			HLSLPROGRAM

				#pragma vertex VertDefault
				#pragma fragment Frag

			ENDHLSL
		}
	}
}
