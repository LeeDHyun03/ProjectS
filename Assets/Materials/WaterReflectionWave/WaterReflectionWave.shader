Shader "Custom/WaterReflectionWave"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _DistortionStrength("Sway Strength", Range(0, 0.1)) = 0.02
        _WaveSpeed("Wave Speed", Range(0, 20)) = 2.0
        _WaveFrequency("Wave Frequency", Range(1, 100)) = 15
        _DistortionDivisionFactor("Division Factor", Range(1, 100)) = 100
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "CanUseSpriteAtlas" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _DistortionStrength;
            float _WaveSpeed;
            float _WaveFrequency;
            float _DistortionDivisionFactor;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                
                OUT.localUV = IN.positionOS.xy + 0.5; 
                
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float time = _Time.y * _WaveSpeed;

                float influence = 1.0 - saturate(IN.localUV.y); 

                float wave = sin(IN.localUV.y * _WaveFrequency + time) * _DistortionStrength / _DistortionDivisionFactor;
                
                float2 offset = float2(wave * influence, 0);

                float2 distortedUV = IN.uv + offset;

                half4 col = tex2D(_MainTex, distortedUV);
                
                col *= IN.color;
                return col;
            }
            ENDHLSL
        }
    }
}