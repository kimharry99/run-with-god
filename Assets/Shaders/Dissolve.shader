Shader "Unlit/Dissolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_NoiseTex("Noise_Texture", 2D) = "white" {}
		_Threshold ("Threshold" , range(0,1)) = 0
		_Glow ("Glow", Color) = (1, 0.5, 0.5, 1)
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
            // make fog work
            #pragma multi_compile_fog

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

			sampler2D _NoiseTex;
			float _Threshold;
			fixed4 _Glow;

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col = tex2D(_MainTex, i.uv);
				if (tex2D(_NoiseTex, i.uv).r < _Threshold)
					discard;
				else if (tex2D(_NoiseTex, i.uv).r < _Threshold + 0.05f)
					col = _Glow;
				return col;
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
				//return col;
            }
            ENDCG
        }
    }
	FallBack "Diffuse"
}
