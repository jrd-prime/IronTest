Shader "Custom/URP/ColorReplaceShader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent" 
            "DisableBatching" = "False" // Разрешаем batching
        }

        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; // Vertex Color
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR; // Передаем Vertex Color
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.color = IN.color; // Передаем цвет вершин
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                if (col.a < 0.1)
                {
                    discard;
                }
                               
                float maxColor = max(col.r, max(col.g, col.b));
                float minColor = min(col.r, min(col.g, col.b));

                // Проверяем, является ли пиксель зеленым или его оттенком
                if (col.g > col.r && col.g > col.b)
                {
                    float intensity = maxColor;
                    
                    // Заменяем только зеленые участки на цвет вершин
                    col.rgb = IN.color.rgb * intensity;
                }

                return col;
            }

            ENDHLSL
        }
    }
}