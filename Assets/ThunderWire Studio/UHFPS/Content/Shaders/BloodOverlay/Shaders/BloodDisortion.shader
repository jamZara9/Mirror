Shader "UHFPS/BloodDisortion"
{
    Properties
    {
        _BlendColor("Blend Color", Color) = (1,1,1,1)
        _OverlayColor("Overlay Color", Color) = (1,1,1,1)
        _BlendTex("Image", 2D) = "white" {}
        _BumpMap("Normal", 2D) = "bump" {}
        _BlendAmount("Blend Amount", Range(0,1)) = 0.5
        _EdgeSharpness("Edge Sharpness", Range(0,1)) = 0.5
        _Distortion("Distortion", Range(0,1)) = 0.5
    }

    SubShader
    {
        ZWrite Off
        ZTest Always
        Blend Off
        Cull Off

        Pass
        {
            Name "BloodDisortion"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            TEXTURE2D(_CameraColorTexture);
            SAMPLER(sampler_CameraColorTexture);

            TEXTURE2D(_BlendTex);
            SAMPLER(sampler_BlendTex);

            TEXTURE2D(_BumpMap);
            SAMPLER(sampler_BumpMap);

            float4 _BlendColor;
            float4 _OverlayColor;

            float _BloodAmount;
            float _BlendAmount;
            float _EdgeSharpness;
            float _Distortion;

            half4 frag(Varyings input) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, input.texcoord);
                float4 blendColor = SAMPLE_TEXTURE2D(_BlendTex, sampler_CameraColorTexture, input.texcoord);
                float4 bumpColor = SAMPLE_TEXTURE2D(_BumpMap, sampler_CameraColorTexture, input.texcoord);

                blendColor.a = blendColor.a + (_BlendAmount * 2 - 1);
                blendColor.a = saturate(blendColor.a * _EdgeSharpness - (_EdgeSharpness - 1) * 0.5);

                half2 bump = UnpackNormal(bumpColor).rg;
                float2 distortedUV = input.texcoord.xy + bump * blendColor.a * _Distortion;
                float4 mainColor = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, distortedUV);
                float4 overlayColor = blendColor;

                overlayColor.rgb = mainColor.rgb * (blendColor.rgb + 0.5) * 1.2;
                blendColor = lerp(blendColor, overlayColor, 0.3);
                mainColor.rgb *= 1 - blendColor.a * 0.5;

                float4 overlay = lerp(float4(1, 1, 1, 1), _OverlayColor, _BloodAmount);
                return lerp(mainColor, blendColor * _BlendColor, blendColor.a) * overlay;
            }

            ENDHLSL
        }
    }
}