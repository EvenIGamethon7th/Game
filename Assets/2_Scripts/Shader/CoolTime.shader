Shader "Unlit/CoolTime"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CoolTime ("CoolTime", range(0, 100)) = 30
        _CurrentCoolTime ("CurrentCoolTime", range(0, 100)) = 30
        _SpriteUV ("Sprite UV", Vector) = (0,0,1,1)
        _CoolTimeColor ("CoolTimeColor", Color) = (1, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector" = "True" }
        Blend One OneMinusSrcAlpha
        Cull Off
		ZWrite Off
		Lighting Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _CoolTime;
            float _CurrentCoolTime;
            half4 _CoolTimeColor;

            float4 _SpriteUV;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.uv.x = lerp(_SpriteUV.x, _SpriteUV.z, o.uv.x);
                o.uv.y = lerp(_SpriteUV.y, _SpriteUV.w, o.uv.y);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);

                col.rgb *= col.a;
                clip(col.a - 0.0005f);

                float pi = 3.14159;
                
                float2 uv = i.uv - float2(lerp(_SpriteUV.x, _SpriteUV.z, 0.5f), lerp(_SpriteUV.y, _SpriteUV.w, 0.5f));
                float polar = (atan2(uv.x, -uv.y) / pi) * 0.5f + 0.5f;
                
                if (polar < _CurrentCoolTime / _CoolTime) {
                    col.rgb = (col.rgb + _CoolTimeColor.rgb) * 0.5f;
                }
                    
                return col;
            }
            ENDHLSL
        }
    }
}
