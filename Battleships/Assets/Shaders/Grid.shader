Shader "Custom/Projector/Grid" {
    Subshader {
        Tags { "Queue"="Transparent+100" }
        Pass {
            ZWrite Off
            Blend OneMinusSrcAlpha SrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f {
                float4 pos: SV_POSITION;
                float4 posProj: TEXCOORD0;
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

            bool h_line(float4 worldPos, float x1, float x2, float y, float thickness) {
                return worldPos.z >= y - thickness / 2.0 && worldPos.z <= y + thickness / 2.0 &&
                       worldPos.x >= x1 && worldPos.x <= x2;
            }

            bool v_line(float4 worldPos, float y1, float y2, float x, float thickness) {
                return worldPos.x >= x - thickness / 2.0 && worldPos.x <= x + thickness / 2.0 &&
                       worldPos.z >= y1 && worldPos.z <= y2;
            }

            float4 frag (v2f i) : COLOR {
                if (i.posProj.w < 0.0) {
                    return float4(0.0, 0.0, 0.0, 1.0);
                }

                if (h_line(i.worldPos, -101.0, 101.0, -100.0, 2.0) ||
                    h_line(i.worldPos, -101.0, 101.0,  100.0, 2.0) ||
                    v_line(i.worldPos, -101.0, 101.0, -100.0, 2.0) ||
                    v_line(i.worldPos, -101.0, 101.0,  100.0, 2.0)) {

                    return float4(0.03, 0.14, 0.42, 0.5);
                }
                for (int offset = -10; offset <= 10.0; offset += 2.0) {
                    if (h_line(i.worldPos, -100.0, 100.0, offset * 10, 0.5) ||
                        v_line(i.worldPos, -100.0, 100.0, offset * 10, 0.5)) {

                        return float4(0.02, 0.11, 0.34, 0.5);
                    }
                }
                return float4(0.0, 0.0, 0.0, 1.0);
            }
            ENDCG
        }
    }
}
