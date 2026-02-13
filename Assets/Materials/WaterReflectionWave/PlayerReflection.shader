Shader "Custom/PlayerReflection"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture (Do Not Use)", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        
        [Header(Render Texture Settings)]
        _RenderTex("Render Texture To Draw", 2D) = "white" {}
        
        [Header(Wave Settings)]
        _DistortionStrength("Sway Strength", Range(0, 0.1)) = 0.02
        _WaveSpeed("Wave Speed", Range(0, 20)) = 2.0
        _WaveFrequency("Wave Frequency", Range(1, 100)) = 15
        _DistortionDivisionFactor("Division Factor", Range(1, 100)) = 100
        
        [Toggle] _PixelSnap("Pixel Snap", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "CanUseSpriteAtlas" = "True"
            "PreviewType" = "Plane"
        }

        Blend One OneMinusSrcAlpha 
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _PIXELSNAP_ON
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 localUV : TEXCOORD1;
                float4 color : COLOR;
            };

            sampler2D _RenderTex;
            float4 _RenderTex_ST;
            half4 _Color;
            float _DistortionStrength;
            float _WaveSpeed;
            float _WaveFrequency;
            float _DistortionDivisionFactor;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(positionWS);

                #if _PIXELSNAP_ON
                OUT.positionHCS.xy = floor(OUT.positionHCS.xy * (_ScreenParams.xy * 0.5)) / (_ScreenParams.xy * 0.5);
                #endif

                OUT.uv = TRANSFORM_TEX(IN.uv, _RenderTex);
                
                OUT.localUV = IN.positionOS.xy + 0.5; 
                
                OUT.color = IN.color * _Color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float time = _Time.y * _WaveSpeed;

                float influence = 1.0 - saturate(IN.localUV.y); 

                float wave = sin(IN.localUV.y * _WaveFrequency + time) * _DistortionStrength / _DistortionDivisionFactor;
                float2 offset = float2(wave * influence, 0);

                float2 distortedUV = IN.uv + offset;
                half4 col = tex2D(_RenderTex, distortedUV);
                
                col.rgb *= col.a; 
                col *= IN.color;

                return col;
            }
            ENDHLSL
        }
    }
}