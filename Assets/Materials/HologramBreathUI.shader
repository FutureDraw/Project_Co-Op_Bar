Shader "Custom/HologramBreathUI"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Hologram Color", Color) = (0, 1, 1, 1)
        _EmissionStrength ("Emission", Range(0, 10)) = 2.5
        
        _BreathSpeed ("Breath Speed", Range(0.1, 10)) = 2.0
        _BreathAmount ("Breath Amount", Range(0, 0.3)) = 0.12
        
        _EdgeGlitchStrength ("Edge Glitch", Range(0, 0.2)) = 0.1
        _GlitchFrequency ("Glitch Frequency", Range(5, 80)) = 40
        _HorizontalGlitch ("Horizontal Glitch", Range(0, 1)) = 0.65
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "PreviewType"="Plane" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float edge : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _EmissionStrength;
            float _BreathSpeed;
            float _BreathAmount;
            float _EdgeGlitchStrength;
            float _GlitchFrequency;
            float _HorizontalGlitch;

            float hash11(float p)
            {
                p = frac(p * .1031);
                p *= p + 33.33;
                p *= p + p;
                return frac(p);
            }

            v2f vert (appdata v)
            {
                v2f o;

                float time = _Time.y;
                float breath = 1.0 + sin(time * _BreathSpeed) * _BreathAmount;

                // Edge detection (по UV)
                float2 center = abs(v.uv - 0.5) * 2;
                float edge = max(center.x, center.y);
                o.edge = pow(edge, 1.8);

                // Glitch
                float glitchTime = time * _GlitchFrequency;
                float glitch = (hash11(floor(glitchTime * 10)) - 0.5) * _EdgeGlitchStrength * o.edge;

                float hGlitch = 0;
                if (hash11(floor(glitchTime * 15)) > 0.65)
                    hGlitch = (hash11(floor(glitchTime * 60)) - 0.5) * 0.12 * _HorizontalGlitch;

                float4 vertex = v.vertex * breath;
                vertex.x += glitch * 2.5 + hGlitch;
                vertex.y += glitch * 1.8;

                o.vertex = UnityObjectToClipPos(vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // UV jitter
                float uvGlitch = (hash11(floor(_Time.y * _GlitchFrequency * 8)) - 0.5) * 0.018 * i.edge;
                uv.x += uvGlitch;

                fixed4 tex = tex2D(_MainTex, uv);
                float4 col = tex * _Color;

                float breathPulse = 0.8 + sin(_Time.y * _BreathSpeed * 1.4) * 0.2;
                float scan = sin(uv.y * 200 + _Time.y * 35) * 0.07 + 0.93;

                col.rgb *= _EmissionStrength * breathPulse * scan;
                col.a *= breathPulse * (0.7 + i.edge * 0.4);

                return col;
            }
            ENDHLSL
        }
    }
}