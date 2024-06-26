Shader "Custom/AnimatedDottedBorderShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (0,0,0,1)
        _Thickness ("Line Thickness", Range(0.001, 0.1)) = 0.01
        _DotSpacing ("Dot Spacing", Range(0.01, 0.2)) = 0.05
        _DotLength ("Dot Length", Range(0.01, 0.2)) = 0.02
        _Speed ("Animation Speed", Range(0.1, 5.0)) = 1.0
        _BackgroundAlpha ("Background Alpha", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
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

            fixed4 _Color;
            float _Thickness;
            float _DotSpacing;
            float _DotLength;
            float _Speed;
            float _BackgroundAlpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 st = i.uv;
                float time = _Time.y * _Speed;

                // Parameters for the dotted line
                float lineWidth = _Thickness; 
                float dotSpacing = _DotSpacing; 
                float dotLength = _DotLength;

                // Create the animated offset
                float offset = time * dotSpacing;

                // Border conditions
                bool isLeftBorder = abs(st.x - 0.1) < lineWidth && st.y >= 0.1 && st.y <= 0.9;
                bool isRightBorder = abs(st.x - 0.9) < lineWidth && st.y >= 0.1 && st.y <= 0.9;
                bool isTopBorder = abs(st.y - 0.1) < lineWidth && st.x >= 0.1 && st.x <= 0.9;
                bool isBottomBorder = abs(st.y - 0.9) < lineWidth && st.x >= 0.1 && st.x <= 0.9;

                // Dotted line conditions
                bool isDottedLeft = isLeftBorder && fmod(st.y + offset, dotSpacing) < dotLength;
                bool isDottedRight = isRightBorder && fmod(st.y + offset, dotSpacing) < dotLength;
                bool isDottedTop = isTopBorder && fmod(st.x + offset, dotSpacing) < dotLength;
                bool isDottedBottom = isBottomBorder && fmod(st.x + offset, dotSpacing) < dotLength;

                // Combine all borders
                bool isDottedBorder = isDottedLeft || isDottedRight || isDottedTop || isDottedBottom;

                // Output color based on the border
                float borderColor = isDottedBorder ? 0.0 : 1.0;
                float alpha = isDottedBorder ? 1.0 : _BackgroundAlpha;

                return fixed4(borderColor, borderColor, borderColor, alpha) * _Color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
