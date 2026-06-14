Shader "Custom/HologramBreath"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Hologram Color", Color) = (0, 1, 1, 1)
        _EmissionStrength ("Emission Strength", Range(0, 10)) = 3
        
        _BreathSpeed ("Breath Speed", Range(0.1, 10)) = 2.0
        _BreathAmount ("Breath Amount", Range(0, 1)) = 0.2
        
        // === ПОДЁРГИВАНИЕ КРАЁВ ===
        _EdgeGlitchStrength ("Edge Glitch Strength", Range(0, 0.15)) = 0.08
        _EdgeGlitchFrequency ("Edge Glitch Frequency", Range(1, 80)) = 35
        _GeneralJitter ("General Jitter", Range(0, 0.03)) = 0.006
        _HorizontalGlitch ("Horizontal Glitch", Range(0, 1)) = 0.7
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float edgeFactor : TEXCOORD4;   // для контроля краёв
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _EmissionStrength;
            float _BreathSpeed;
            float _BreathAmount;
            float _EdgeGlitchStrength;
            float _EdgeGlitchFrequency;
            float _GeneralJitter;
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

                // Fresnel — помогает определить "края"
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 viewDir = normalize(WorldSpaceViewDir(v.vertex));
                float fresnel = 1 - saturate(dot(worldNormal, viewDir));
                o.edgeFactor = pow(fresnel, 1.5); // сильнее на краях

                // === ДЁРГАНИЕ КРАЁВ ===
                float edgeGlitchTime = time * _EdgeGlitchFrequency;
                float edgeGlitch = (hash11(floor(edgeGlitchTime * 8)) - 0.5) * _EdgeGlitchStrength;
                edgeGlitch *= o.edgeFactor; // только на краях

                // Постоянное мелкое дрожание
                float constantJitter = sin(time * 42) * 0.4 + sin(time * 67) * 0.3;
                constantJitter *= _GeneralJitter;

                // Горизонтальный глитч
                float hGlitch = 0;
                if (hash11(floor(edgeGlitchTime * 12)) > 0.6)
                    hGlitch = (hash11(floor(edgeGlitchTime * 45)) - 0.5) * 0.1 * _HorizontalGlitch;

                float4 vertex = v.vertex * breath;
                
                // Основное смещение (особенно сильно по краям)
                vertex.x += (edgeGlitch * 1.8 + constantJitter + hGlitch);
                vertex.y += (edgeGlitch * 1.4 + constantJitter * 0.8);
                vertex.z += edgeGlitch * 0.6;

                // Дополнительное смещение по нормали на краях
                vertex.xyz += v.normal * edgeGlitch * 6;

                o.vertex = UnityObjectToClipPos(vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = worldNormal;
                o.viewDir = viewDir;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Лёгкое дрожание текстуры на краях
                float uvJitter = (hash11(floor(_Time.y * _EdgeGlitchFrequency * 6)) - 0.5) * 0.02 * i.edgeFactor;
                uv.x += uvJitter;

                fixed4 tex = tex2D(_MainTex, uv);
                float4 col = tex * _Color;

                float breathPulse = 0.75 + sin(_Time.y * _BreathSpeed * 1.3) * 0.25;

                float scan = sin(i.uv.y * 160 + _Time.y * 30) * 0.07 + 0.93;

                float fresnel = pow(1 - saturate(dot(i.worldNormal, i.viewDir)), 2);

                col.rgb *= _EmissionStrength * breathPulse * scan;
                col.rgb += fresnel * _Color.rgb * 3.0;

                // Прозрачность тоже пульсирует на краях
                col.a = _Color.a * breathPulse * (0.6 + fresnel * 0.5);

                return col;
            }
            ENDHLSL
        }
    }
}