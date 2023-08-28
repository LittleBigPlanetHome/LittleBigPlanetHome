Shader "Custom/DissolveShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _DissolveMap ("Dissolve Map", 2D) = "white" {}
        _DissolveColor ("Dissolve Color", Color) = (1,1,1,1)
        _DissolveAmount ("Dissolve Amount", Range(0,1)) = 0
    }
 
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Opaque"}
        LOD 100
 
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
            float _DissolveAmount;
            float4 _DissolveColor;
            sampler2D _DissolveMap;
 
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                float dissolve = tex2D(_DissolveMap, i.uv).r;
                dissolve = (1 - _DissolveAmount) + (_DissolveAmount * dissolve);
                col = lerp(col, _DissolveColor, dissolve);
                return col;
            }
            ENDCG
        }
    }
 
    FallBack "Diffuse"
}