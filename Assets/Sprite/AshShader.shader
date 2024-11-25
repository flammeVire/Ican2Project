Shader "Custom/DynamicHoleShader"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _MaskTex("Mask Texture", 2D) = "white" {}
        _MaskPosition("Mask Position", Vector) = (0, 0, 0, 0)
        _MaskSize("Mask Size", Float) = 1.0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
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
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float2 worldPos : TEXCOORD1;
                };

                sampler2D _MainTex;
                sampler2D _MaskTex;
                float4 _MaskPosition;
                float _MaskSize;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.worldPos = v.vertex.xy; // Position dans le monde pour comparaison
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 mainTex = tex2D(_MainTex, i.uv);

                // Calcul de la distance entre le pixel et la position du masque
                float2 maskCenter = _MaskPosition.xy;
                float dist = distance(i.worldPos, maskCenter);

                // Appliquer le masque uniquement si dans le rayon défini
                if (dist < _MaskSize)
                {
                    // Utiliser la texture du masque pour définir le trou
                    float2 maskUV = (i.worldPos - maskCenter) / _MaskSize + 0.5;
                    fixed4 maskTex = tex2D(_MaskTex, maskUV);

                    if (maskTex.a > 0.5) // Alpha du masque pour déterminer le trou
                    {
                        discard; // Trou transparent
                    }
                }

                return mainTex;
            }
            ENDCG
        }
        }
            FallBack "Diffuse"
}