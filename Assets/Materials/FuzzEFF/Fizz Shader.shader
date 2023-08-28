Shader "Sackboy/Fur"
{
    //re-wrote the script from the blog -vzp (TO-DO: ADD STANDARD CODE FOR SACKSKIN TEX)
    Properties
    {        
        [Header(Sackboy Skin Settings)] 
        [Space(13)]
        //texture def-
       _Color("Color Tint", Color) = (1,1,1,1)
       _MainTexture("Diffuse Texture", 2D) = "white" {}
       _DirtTexture("Dirt Texture", 2D) = "white" {}
       _DefTexture("Additional Texture", 2D) = "white" {}
       _SpecColor("Specular Color", Color) = (1,1,1,1)
       _Shininess("Shininess", Float) = 10
       _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimPower("Rim Power", Range(0.1,10)) = 3.0

        [Header(Sackboy Fuzz Settings)]
        [Space(13)]
        //added missin properties
       _MainTex ("Main Texture", 2D) = "white" {}
       _FinColor("Fuzz Color", Color) = (1,1,1,1)
       _SideFurTex ("Side Fur Texture", 2D) = "white" {}
       _FurMaskTex ("Fur Mask Texture", 2D) = "white" {}
       [Space(5)]
       _Size("Size" , float) = 0.5
        [Space(5)]
       _FurLayers("Fur Layers" , float) = 0.5
        [Space(5)]
       _FluffTex ("Fluff Texture", 2D) = "white" {}
       _Dirt("Dirt Texture", 2D) = "white" {}
       _Decals("Decals Texture", 2D) = "white" {}
       [Space(5)]
       _FalloffExponent("FallOff E" , float) = 0.5
           [Space(5)]
       _FalloffBrightness2("FallOff Brightness2" , float) = 0.5
           [Space(5)]
           //Float 4's
           _MainTex_ST("MainTex ST", Vector) = (1, 1, 1, 1)
               [Space(5)]
           _Comb("Comb", Vector) = (1, 1, 1, 1)
               [Space(5)]
           _Falloff("FallOff", Vector) = (1, 1, 1, 1)
               [Space(5)]
           _Falloff2("FallOff2", Vector) = (1, 1, 1, 1)
    }
        SubShader
       {
           //pass for main material
           Pass
            {
               Tags {"RenderType" = "Opaque"}
               CGPROGRAM
           

   #pragma vertex vert
   #pragma fragment frag
        #pragma exclude_renderers Flash


           sampler2D _MainTexture;
       sampler2D _DefTexture;
       sampler2D _AOTex;
       float4 _MainTexture_ST;
              float4 _Color;
       float4 _SpecColor;
       float4 _RimColor;
       float _Shininess;
       float _RimPower;

       float4 _LightColor0;
       
       struct vertexInput {

           float4 vertex : POSITION;
           float3 normal : NORMAL;
           float4 texcoord : TEXCOORD0;

       };

       struct vertexOutput {
           float4 pos : SV_POSITION;
           float4 tex : TEXTCOORD0;
           float4 posWorld : TEXCOORD1;
           float3 normalDir : TEXCOORD2;
       };

       vertexOutput vert(vertexInput v) {
           vertexOutput o;

           o.posWorld = mul(unity_ObjectToWorld, v.vertex);
           o.normalDir = normalize( mul( float4( v.normal, 0.0), unity_WorldToObject).xyz);
           o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
           o.tex = v.texcoord;



           return o;
       }

       float4 frag(vertexOutput i) : COLOR{
            float3 normalDirection = i.normalDir;
            float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            float3 lightDirection;
            float atten;



            if (_WorldSpaceLightPos0.w == 0.0) {
                atten = 1.0;
                lightDirection = normalize(_WorldSpaceLightPos0.xyz);

            }
            else {
                float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
                float distance = length(fragmentToLightSource);
                atten = 1.0 / distance;
                lightDirection = normalize(fragmentToLightSource);
            }

            

            float3 diffuseReflection = atten * _LightColor0.xyz * saturate(dot(normalDirection, lightDirection));
            float3 specularReflection = diffuseReflection * _SpecColor.xyz * pow(saturate(dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);

            float rim = 1 - saturate(dot(viewDirection, normalDirection));
            float3 rimLighting = saturate(dot(normalDirection, lightDirection) * _RimColor.xyz * _LightColor0.xyz * pow(rim, _RimPower));

            float3 lightFinal = diffuseReflection + specularReflection + rimLighting + UNITY_LIGHTMODEL_AMBIENT.xyz;;

            float4 tex = tex2D(_MainTexture, i.tex.xy * _MainTexture_ST.xy + _MainTexture_ST.zw);

            return float4(tex.xyz * lightFinal * _Color.xyz , 1.0);
       }
           ENDCG
            
         }

  
        //pass for fins 
        Pass
           {
            LOD 100
            Cull Off
            ZWrite Off
            Ztest Less
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #define UNITY_SHADER_NO_UPGRADE
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma multi_compile_fwd_base
            #pragma target 3.0
            #pragma require geometry

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "AutoLight.cginc"

            struct Input
        {
            float2 uv_MainTex1;
        };
        sampler2D _MainTex1;
        half _Glossiness;
        half _Metallic;
        float4 _FinColor;
        fixed4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float3 normal : NORMAL;
            };


            struct v2g
            {
                float2 uv : TEXCOORD0;
                float2 uv_unscaled : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float3 falloff : COLOR0;
                float4 diffuse : COLOR1;
                float4 ambient : COLOR2;
                SHADOW_COORDS(3)
            };

            struct g2f
            {
                float2 uv : TEXCOORD0;
                float2 originalUv : TEXCOORD1;
                float2 originalUv_unscaled : TEXCOORD2;
                float2 uv2 : TEXCOORD3;
                float4 vertex : SV_POSITION;
                float3 falloff : COLOR0;
                float4 diffuse : COLOR1;
                float4 ambient : COLOR2;
                SHADOW_COORDS(4)
            };

            sampler2D _MainTex;
            sampler2D _SideFurTex;
            sampler2D _FurMaskTex;
            float4 _MainTex_ST;
            float _Size;
            float _FurLayers;
            float4 _Comb;
            sampler2D _FluffTex;
            sampler2D _Dirt;
            sampler2D _Decals;
            float4 _Falloff;
            float _FalloffExponent;
            //float _FalloffExponent2;
            float _FalloffBrightness2;
            float4 _Falloff2;
             

    
            

            float3 DoFalloff(float lightatten, float4 normal, float4 viewDir, float3 worldNormal)
            {
                float biasedatten = lightatten * 0.5f;//pow(saturate(lightatten), _RimBias)*_RimBias;
                float4 FalloffTerm = pow(1 - dot(normal, normalize(viewDir)), 1);
                float4 Falloff1 = _Falloff * lerp(1 - worldNormal.y, 1, 0.5);
                float4 Falloff2 = _Falloff2 * lerp(worldNormal.y, 1, 0.75);
                float3 Falloff = lerp(Falloff1 * _FalloffExponent, Falloff2 * _FalloffBrightness2, biasedatten) * FalloffTerm / 3;
                return Falloff * saturate(pow(FalloffTerm, 0.35));
            }

            float DoSoftDiffuse(float NdotL)
            {
                NdotL = saturate(NdotL);
                return (pow(NdotL, 0.5) / 6) + (pow(NdotL, 1) / 5) + (pow(NdotL, 2) / 1.5);
            }

            v2g vert(appdata v)
            {
                v2g o;
                o.pos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv_unscaled = v.uv;
                o.uv2 = v.uv2;
                o.normal = mul(v.normal, unity_WorldToObject);
                float4 viewDir = float4(ObjSpaceViewDir(v.vertex),1);
                float3 normal = v.normal;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 wNormal = worldNormal;
                worldNormal *= 1.25f;
                float3 screenNormal = mul(UNITY_MATRIX_V, UnityObjectToWorldNormal(v.normal));
                //float3 falloff = pow(1 - dot(normal, normalize(viewDir)), _FalloffExponent2) * _FalloffExponent * 0.75;
                //float falloffthingy = 1 - lerp((clamp(screenNormal.x, 0, 1) * clamp(screenNormal.x, 0, 1)), (clamp(screenNormal.y, 0, 1) * clamp(screenNormal.y, 0, 1)), 0.5f);
                o.diffuse = DoSoftDiffuse(dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diffuse.w = 1;
                o.falloff = DoFalloff(o.diffuse, float4(normal,0), viewDir, worldNormal) * 2;//((falloff * falloffthingy * _Falloff) + ((falloff * falloff * (1 - clamp(falloffthingy, 0, 1)) / 8) * _FalloffBrightness2 * _LightColor0));
                float ambientUp = wNormal.y;
                float ambientDown = 1 - wNormal.y;
                float4 ambient;
                ambient.w = 1;
                ambient.xyz = unity_AmbientSky * ambientUp;
                ambient.xyz += unity_AmbientGround * ambientDown;
                o.ambient = ambient;
                TRANSFER_SHADOW(o);
                return o;
            }

            [maxvertexcount(4)]
            void geom(line v2g IN[2], inout TriangleStream<g2f> triStream)
            {
                float4x4 vp = mul(UNITY_MATRIX_MVP, unity_WorldToObject);

                float3 eyeVec = normalize(((IN[0].pos - _WorldSpaceCameraPos) + (IN[1].pos - _WorldSpaceCameraPos)) / 2);
                float4 lineNormal = float4(normalize((IN[0].normal + IN[1].normal) / 2), 0) * 0.15 * _Size;
                float eyeDot = dot(lineNormal, eyeVec);

                float3 newNormal = normalize(cross(IN[1].pos - IN[0].pos, lineNormal));
                float maxOffset = 0.25f;

                if (eyeDot < maxOffset && eyeDot > -maxOffset)
                {

                    g2f pIn;

                    pIn.vertex = mul(vp, IN[1].pos);
                    pIn.uv = float2(0.0625 + ((IN[1].uv.x + IN[1].uv.y) / 2), 0);
                    pIn.originalUv = IN[1].uv;
                    pIn.originalUv_unscaled = IN[1].uv_unscaled;
                    pIn.uv2 = IN[1].uv2;
                    pIn.falloff = IN[1].falloff;
                    #if defined (SHADOWS_SCREEN) || defined (SHADOWS_DEPTH) || defined (SPOT)
                    pIn._ShadowCoord = IN[1]._ShadowCoord;
                    #endif
                    pIn.diffuse = IN[1].diffuse;
                    pIn.ambient = IN[1].ambient;
                    triStream.Append(pIn);

                    pIn.vertex = mul(vp, IN[1].pos + lineNormal);
                    pIn.uv = float2(0.0625 + ((IN[1].uv.x + IN[1].uv.y) / 2), 1);
                    pIn.originalUv = IN[1].uv;
                    pIn.originalUv_unscaled = IN[1].uv_unscaled;
                    pIn.uv2 = IN[1].uv2;
                    pIn.falloff = IN[1].falloff;
                    #if defined (SHADOWS_SCREEN) || defined (SHADOWS_DEPTH) || defined (SPOT)
                    pIn._ShadowCoord = IN[1]._ShadowCoord;
                    #endif
                    pIn.diffuse = IN[1].diffuse;
                    pIn.ambient = IN[1].ambient;
                    triStream.Append(pIn);

                    pIn.vertex = mul(vp, IN[0].pos);
                    pIn.uv = float2(((IN[1].uv.x + IN[1].uv.y) / 2), 0);
                    pIn.originalUv = IN[0].uv;
                    pIn.originalUv_unscaled = IN[0].uv_unscaled;
                    pIn.uv2 = IN[0].uv2;
                    pIn.falloff = IN[0].falloff;
                    #if defined (SHADOWS_SCREEN) || defined (SHADOWS_DEPTH) || defined (SPOT)
                    pIn._ShadowCoord = IN[0]._ShadowCoord;
                    #endif
                    pIn.diffuse = IN[0].diffuse;
                    pIn.ambient = IN[0].ambient;
                    triStream.Append(pIn);

                    pIn.vertex = mul(vp, IN[0].pos + lineNormal);
                    pIn.uv = float2(((IN[1].uv.x + IN[1].uv.y) / 2), 1);
                    pIn.originalUv = IN[0].uv;
                    pIn.originalUv_unscaled = IN[0].uv_unscaled;
                    pIn.uv2 = IN[0].uv2;
                    pIn.falloff = IN[0].falloff;
                    #if defined (SHADOWS_SCREEN) || defined (SHADOWS_DEPTH) || defined (SPOT)
                    pIn._ShadowCoord = IN[0]._ShadowCoord;
                    #endif
                    pIn.diffuse = IN[0].diffuse;
                    pIn.ambient = IN[0].ambient;
                    triStream.Append(pIn);

                    triStream.RestartStrip();
                }

            }

            fixed4 frag(g2f i) : SV_Target
            {

                half    shadow = 1.0;

                shadow = SHADOW_ATTENUATION(IN);

                fixed4 originalCol = tex2D(_MainTex, i.originalUv) * tex2D(_Dirt, i.originalUv_unscaled) * _FinColor;
                fixed col = tex2D(_FluffTex, i.uv).y;
                fixed mask = tex2D(_FluffTex, i.originalUv_unscaled).x;

                //if (mask.r <= .5) discard;
                clip(mask.r - 0.75);

                float clampcolorterm_uv2 = 1;
                clampcolorterm_uv2 -= clamp((-clamp(floor(i.uv2.x), -1, 0) + (clamp(ceil(i.uv2.x), 1, 2) - 1)), 0, 1);
                float4 decaldiff = 0;
                //#if defined(DECAL)
                decaldiff = tex2D(_Decals, i.uv2);
                //#endif
                col = lerp(col, 0, pow(decaldiff.w,2) / 2 * clampcolorterm_uv2);

                originalCol.w *= col * mask;
                originalCol.xyz *= ((i.diffuse * shadow) + i.ambient);
                originalCol.xyz += i.falloff / 4;

                return originalCol;// *col;
            }

            ENDCG
           }
    }
    FallBack "Diffuse"
}


