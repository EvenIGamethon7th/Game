Shader "Unlit/Rhombus"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ChangeTime ("ChangeTime", range(0, 3)) = 1
        _CurrentTime ("CurrentTime", range(0, 3)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

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

            float _ChangeTime;
            float _CurrentTime;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = half4(0, 0, 0, 1);
                if (i.uv.x > _CurrentTime * 16 / _ChangeTime){
                    col = tex2D(_MainTex, i.uv);
                    if (col.r >= _CurrentTime * 24 / i.uv.x){
                        col.a = 0;
                    }
                    else
                        col = half4(0, 0, 0, 1);
                }
                    
                return col;
            }
            ENDHLSL
        }
    }
}
