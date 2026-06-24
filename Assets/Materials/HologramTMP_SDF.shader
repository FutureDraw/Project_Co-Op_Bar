Shader "Custom/HologramTMP_SDF"
{
    Properties
    {
        _FaceColor ("Face Color", Color) = (1,1,1,1)
        _MainTex ("Alpha Texture", 2D) = "white" {}

        _BreathSpeed ("Breath Speed", Range(0.1, 10)) = 2.0
        _BreathAmount ("Breath Amount", Range(0, 0.3)) = 0.12

        _EdgeGlitchStrength ("Edge Glitch Strength", Range(0, 0.25)) = 0.1
        _GlitchFrequency ("Glitch Frequency", Range(5, 100)) = 45
        _HorizontalGlitch ("Horizontal Glitch", Range(0, 1)) = 0.7
        _EmissionStrength ("Emission Strength", Range(0, 10)) = 2.8
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex VertShader
            #pragma fragment PixShader
            #pragma multi_compile __ UNITY_UI_CLIP_RECT

            #include "UnityCG.cginc"
            #include "Assets/TextMesh Pro/Shaders/TMPro_Properties.cginc"  // Важно!

            struct vertex_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv0 : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv0 : TEXCOORD0;
                float edge : TEXCOORD1;
            };

            float _BreathSpeed;
            float _BreathAmount;
            float _EdgeGlitchStrength;
            float _GlitchFrequency;
            float _HorizontalGlitch;
            float _EmissionStrength;

            float hash11(float p)
            {
                p = frac(p * .1031);
                p *= p + 33.33;
                p *= p + p;
                return frac(p);
            }

            v2f VertShader(vertex_t input)
            {
                v2f output;

                float time = _Time.y;
                float breath = 1.0 + sin(time * _BreathSpeed) * _BreathAmount;

                // Определение краёв буквы (по UV)
                float2 center = abs(input.uv0 - 0.5) * 2.0;
                float edge = max(center.x, center.y);
                output.edge = pow(edge, 2.0);

                // Glitch только на краях
                float glitchTime = time * _GlitchFrequency;
                float glitch = (hash11(floor(glitchTime * 12)) - 0.5) * _EdgeGlitchStrength * output.edge;

                float hGlitch = 0;
                if (hash11(floor(glitchTime * 18)) > 0.65)
                    hGlitch = (hash11(floor(glitchTime * 70)) - 0.5) * 0.15 * _HorizontalGlitch;

                float4 vertex = input.vertex * breath;
                vertex.x += glitch * 2.8 + hGlitch;
                vertex.y += glitch * 2.0;

                output.vertex = UnityObjectToClipPos(vertex);
                output.uv0 = TRANSFORM_TEX(input.uv0, _MainTex);
                output.color = input.color;

                return output;
            }

            fixed4 PixShader(v2f input) : SV_Target
            {
                float2 uv = input.uv0;

                // Дрожание UV на краях
                float uvGlitch = (hash11(floor(_Time.y * _GlitchFrequency * 9)) - 0.5) * 0.025 * input.edge;
                uv.x += uvGlitch;

                half4 c = tex2D(_MainTex, uv) * input.color * _FaceColor;

                float breathPulse = 0.75 + sin(_Time.y * _BreathSpeed * 1.35) * 0.25;
                float scan = sin(uv.y * 180 + _Time.y * 40) * 0.08 + 0.92;

                c.rgb *= _EmissionStrength * breathPulse * scan;
                c.a *= breathPulse * (0.75 + input.edge * 0.5);

                #ifdef UNITY_UI_CLIP_RECT
                float2 factor = saturate((_ClipRect.zw - _ClipRect.xy) * input.vertex.xy);
                c.a *= factor.x * factor.y;
                #endif

                return c;
            }
            ENDHLSL
        }
    }
}