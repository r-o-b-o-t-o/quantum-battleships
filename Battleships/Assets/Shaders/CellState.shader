Shader "Custom/Projector/CellState" {
    Properties {
        _Texture ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }

    Subshader {
        Tags { "Queue"="Transparent+100" }
        Pass {
            ZWrite Off
            Blend OneMinusSrcAlpha SrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _Texture;
            float4 _Color;

            struct v2f {
                float4 pos: SV_POSITION;
                float4 posProj : TEXCOORD0;
                float4 worldPos: TEXCOORD1;
            };

            uniform float4x4 unity_Projector;

            v2f vert(appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.posProj = mul(unity_Projector, v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float4 frag (v2f i) : COLOR {
                if (i.posProj.w < 0.0) {
                    return float4(0.0, 0.0, 0.0, 1.0);
                }

                float4 tex = tex2D(_Texture, i.posProj.xy / i.posProj.w);
                tex.a = 1.0 - tex.a;
                tex *= _Color;
                return tex;
            }
            ENDCG
        }
    }
}
