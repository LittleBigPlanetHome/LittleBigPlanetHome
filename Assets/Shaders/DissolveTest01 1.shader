Shader "Custom/DissolveTexture" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaTex ("Alpha Texture", 2D) = "white" {}
        _Cutoff ("Dissolve Amount", Range(0.0, 1.0)) = 0.5
        _DissolveColor ("Dissolve Color", Color) = (1,1,1,1)
        _Speed ("Dissolve Speed", Range(0.1, 10.0)) = 1.0
    }
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float _Cutoff;
            float4 _DissolveColor;
            float _Speed;
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            fixed4 frag (v2f i) : SV_Target {
                float dissolve = _Cutoff + (_Speed * _Time.y);
                dissolve = clamp(dissolve, 0.0, 1.0);
                fixed4 col = tex2D(_MainTex, i.uv);
                float alpha = tex2D(_AlphaTex, i.uv).r;
                if (i.uv.y > dissolve) {
                    col = lerp(col, _DissolveColor, 1.0 - dissolve);
                    col.a = 1.0 - dissolve;
                } else {
                    col.a = alpha;
                }
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
