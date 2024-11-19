Shader "Custom/HoleShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _Mask ("Mask Texture", 2D) = "White" {}
        _MaskPosition ("Mask Position", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _Mask;
            float4 _MaskPosition;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float mask = tex2D(_Mask, i.uv).a; // Utiliser le canal alpha du masque
                col.a *= 1 - mask; // Rendre transparent là où le masque est blanc
                return col;
            }
            ENDCG
        }
    }
}