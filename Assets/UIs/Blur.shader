Shader "UI/AdjustableBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // メインのテクスチャ
        _BlurSize ("Blur Size", Float) = 1.0 // ブラーの強さ
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float4 col = float4(0, 0, 0, 0);

                // ブラーの強さを調整
                int blurRadius = int(ceil(_BlurSize * 5)); // ブラーの半径
                for (int x = -blurRadius; x <= blurRadius; x++)
                {
                    for (int y = -blurRadius; y <= blurRadius; y++)
                    {
                        col += tex2D(_MainTex, uv + float2(x, y) * _MainTex_TexelSize.xy * _BlurSize);
                    }
                }

                // ピクセル数で正規化
                col /= (float)((blurRadius * 2 + 1) * (blurRadius * 2 + 1));

                return col;
            }
            ENDCG
        }
    }
}
