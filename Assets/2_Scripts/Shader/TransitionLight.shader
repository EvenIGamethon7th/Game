Shader "Unlit/TransitionLight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Duration ("Duration", range(0, 1)) = 0
        _Brightness ("Brightness", range(1, 10)) = 2
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Blend One OneMinusSrcAlpha
        Cull Off
		ZWrite Off
		Lighting Off

        Pass
        {
            CGPROGRAM
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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Duration;
            float _Brightness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (i.uv.x > _Duration) clip(-1);

                fixed4 col = tex2D(_MainTex, i.uv);
                clip(col.a - 0.1);
                col.rgb *= _Brightness;
                return col;
            }
            ENDCG
        }
    }
}
