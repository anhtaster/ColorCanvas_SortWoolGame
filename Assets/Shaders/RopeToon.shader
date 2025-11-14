Shader "Custom/RopeToon"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Gloss ("Gloss", Range(0,1)) = 0.4
        _Shine ("Shine Power", Range(1,64)) = 16
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Color;
            float _Gloss;
            float _Shine;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 normalDir : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 n = normalize(i.normalDir);
                float3 v = normalize(i.viewDir);

                // fake lighting
                float ndotv = saturate(dot(n, v));

                float highlight = pow(ndotv, _Shine) * _Gloss;

                float3 finalColor = _Color.rgb + highlight;
                return float4(finalColor, _Color.a);
            }
            ENDCG
        }
    }
}
