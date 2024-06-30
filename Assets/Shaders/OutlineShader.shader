Shader "Custom/OutlineShader"
{
    Properties {
        _OutlineColor ("Outline Color", Color) = (0, 0, 1, 1) // 파란색
        _Outline ("Outline width", Range (0, 1)) = 0.1
    }

    SubShader {
        // 기본 머티리얼 렌더링 패스
        Pass {
            Name "BASE"
            Tags { "LightMode" = "ForwardBase" }
            // 기본 Unlit 셰이더의 코드를 여기에 추가합니다.
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
            };

            v2f vert (appdata_t v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = normalize(v.normal);
                return o;
            }

            half4 frag (v2f i) : SV_Target {
                return half4(1,1,1,1); // 기본 흰색으로 렌더링
            }
            ENDCG
        }

        // 외곽선 렌더링 패스
        Pass {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Front
            ZWrite On
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert_outline
            #pragma fragment frag_outline
            #include "UnityCG.cginc"

            uniform float _Outline;
            uniform float4 _OutlineColor;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert_outline(appdata v) {
                // 법선 방향으로 버텍스를 이동시킵니다.
                v.vertex.xyz += v.normal * _Outline;

                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = _OutlineColor;
                return o;
            }

            half4 frag_outline(v2f i) : SV_Target {
                return i.color;
            }
            ENDCG
        }
    }

    Fallback "Diffuse"
}
