Shader "Unlit/CustomSpine"
{
    Properties {
        _Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
        [NoScaleOffset] _MainTex ("Main Texture", 2D) = "black" {}
        [HideInInspector] _StencilRef("Stencil Reference", Float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 8

        // Outline properties are drawn via custom editor.
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Range(0.0, 0.3)) = 0.05
    }

    SubShader {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Blend One OneMinusSrcAlpha
        Cull Off
		ZWrite Off
		Lighting Off

        Stencil {
			Ref[_StencilRef]
			Comp[_StencilComp]
			Pass Keep
		}

        Pass {

            Tags { "LightMode" = "Character" }
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma vertex vert
            #pragma fragment frag

            CBUFFER_START(UnityPerMaterial)

            sampler2D _MainTex;
            float4 _MainTex_ST;

            CBUFFER_END

            struct VertexInput {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertexColor = v.vertexColor;
                return o;
            }

            float4 frag (VertexOutput i) : SV_Target {
                float4 texColor = tex2D(_MainTex, i.uv);
                return texColor;
            }

            ENDHLSL
        }

        Pass {
            Name "Outline"
        
            Tags { "LightMode" = "Outline" }
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        
            CBUFFER_START(UnityPerMaterial)

            sampler2D _MainTex;
            float4 _MainTex_ST;
        
            float4 _OutlineColor;
            float _OutlineThickness;

            CBUFFER_END
        
            struct VertexInput {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
        
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };
        
            VertexOutput vert (VertexInput v) {
                VertexOutput o;

                o.pos = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertexColor = v.vertexColor;
                return o;
            }
        
            float4 frag (VertexOutput i) : SV_Target {
                float4 texColor = tex2D(_MainTex, i.uv);
        
                float2 offset1 = float2(_OutlineThickness, 0);
                float2 offset2 = float2(0, _OutlineThickness);

				half col1 = tex2D(_MainTex, i.uv + offset1).a;
                half col2 = tex2D(_MainTex, i.uv - offset1).a;
                half col3 = tex2D(_MainTex, i.uv + offset2).a;
                half col4 = tex2D(_MainTex, i.uv - offset2).a;

				float alpha = texColor.a;
                alpha = (col1 + col2 + col3 + col4) * 0.25f;

				float4 result = texColor;

                if (alpha > 0 && texColor.a == 0)
                {
                    result.rgb = _OutlineColor.rgb;
                    result.a = _OutlineColor.a;
                }

				result.a = max(result.a, alpha);

				return result;
            }
            ENDHLSL
        }
    }
}