Shader "Kitten/Unlit/UnlitKitten"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    _AlphaTex("Alpha", 2D) = "white" {}
    }
        SubShader
    {
Tags { "Queue"="Transparent" "RenderType"="Transparent" }
Blend SrcAlpha OneMinusSrcAlpha
ZWrite Off
 
        Pass
    {
        CGPROGRAM
#pragma vertex vert
#pragma fragment frag
        // make fog work
#pragma multi_compile_fog
 
#include "UnityCG.cginc"
 
    struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
        float2 uv2 : TEXCOORD1;
    };
 
    struct v2f
    {
        float2 uv : TEXCOORD0;
        float2 uv2 : TEXCOORD1;
        UNITY_FOG_COORDS(1)
            float4 vertex : SV_POSITION;
    };
 
    sampler2D _MainTex;
    sampler2D _AlphaTex;
    float4 _MainTex_ST;
    float4 _AlphaTex_ST;
 
 
    v2f vert(appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        o.uv2 = TRANSFORM_TEX(v.uv2, _AlphaTex);
        UNITY_TRANSFER_FOG(o,o.vertex);
        return o;
    }
 
    fixed4 frag(v2f i) : SV_Target
    {
    // sample the texture
fixed4 col = tex2D(_MainTex, i.uv);
fixed4 alpha = tex2D(_AlphaTex, i.uv2);
col.a *= alpha.g;
return col;

 
 
    }
        ENDCG
    }
    }
}
 