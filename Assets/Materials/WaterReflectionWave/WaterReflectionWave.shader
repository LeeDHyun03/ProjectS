Shader "Custom/WaterReflectionWave"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _DistortionStrength("Distortion Strength", Range(0, 0.1)) = 0.03
        _WaveSpeed("Wave Speed", Range(0, 5)) = 1.5
        _WaveFrequency("Wave Frequency", Range(1, 20)) = 8
        _DepthFade("Depth Influence", Range(0, 3)) = 1.5
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
                    float4 color : COLOR;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _DistortionStrength;
                float _WaveSpeed;
                float _WaveFrequency;
                float _DepthFade;

                Varyings vert(Attributes IN)
                {
                    Varyings OUT;
                    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                    OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                    OUT.color = IN.color;
                    return OUT;
                }

                half4 frag(Varyings IN) : SV_Target
                {
                    float time = _Time.y * _WaveSpeed;

                    float depthFactor = pow(IN.uv.y, _DepthFade);

                    float wave =
                        sin((IN.uv.y + time) * _WaveFrequency) *
                        cos((IN.uv.x + time * 0.7) * _WaveFrequency);

                    float2 distortion;
                    distortion.x = wave * _DistortionStrength * 0.5;
                    distortion.y = wave * _DistortionStrength * depthFactor;

                    float2 distortedUV = IN.uv + distortion;

                    half4 col = tex2D(_MainTex, distortedUV);

                    col *= IN.color;

                    return col;
                }
            ENDHLSL
            }
        }
}
