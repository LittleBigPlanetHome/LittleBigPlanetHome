Shader "LittleBigPlanet" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _SecondTex ("Second Texture", 2D) = "white" {}
        _ThirdTex ("Third Texture", 2D) = "white" {}
        _FourthTex ("Fourth Texture", 2D) = "white" {}
        _FifthTex ("Fifth Texture", 2D) = "white" {}
        _SixthTex ("Sixth Texture", 2D) = "white" {}
        _SeventhTex ("Seventh Texture", 2D) = "white" {}
        _EighthTex ("Eighth Texture", 2D) = "white" {}
        _ShadowTex ("Shadow Texture", 2D) = "white" {}
        _ColorBuffer ("Color Buffer", 2D) = "white" {}
        _CamPos ("Camera Position", Vector) = (0, 0, 0)
        _AmbientColor ("Ambient Color", Color) = (1, 1, 1, 1)
        _FogColor ("Fog Color", Color) = (1, 1, 1, 1)
        _SunColor ("Sun Color", Color) = (1, 1, 1, 1)
        _SunPosition ("Sun Position", Vector) = (0, 0, 0)
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimColor2 ("Rim Color 2", Color) = (1, 1, 1, 1)
        _LightScaleAdd ("Light Scale Add", Range(0, 1)) = 0.5
        _ThingColor ("Thing Color", Color) = (1, 1, 1, 1)
    }
    SubShader {
        Tags {"RenderType"="Opaque"}
        LOD 100
        Pass {
            
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float3 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
    float2 texcoord : TEXCOORD0;
    float4 vertex : SV_POSITION;
    float3 worldPos : TEXCOORD1;
    float3 normal : TEXCOORD2;
};
        Fallback "Standard"
        sampler2D _MainTex;
        sampler2D _SecondTex;
        sampler2D _ThirdTex;
        sampler2D _FourthTex;
        sampler2D _FifthTex;
        sampler2D _SixthTex;
        sampler2D _SeventhTex;
        sampler2D _EighthTex;
        sampler2D _ShadowTex;
        sampler2D _ColorBuffer;
        float4 _CamPos;
        uniform float4 _AmbientColor;
        uniform float4 _FogColor;
        uniform float4 _SunColor;
        float4 _SunPosition;
        uniform float4 _RimColor;
        uniform float4 _RimColor2;
        float4 _ThingColor;

sampler2D _ShadowTex;
sampler2D _Cbuf;
float4 main(float4 color : COLOR, float2 uv : TEXCOORD0) : SV_Target {
    float4x4 model = float4x4(1.0);
    for (int i = 0; i < 100; i++) {
        model = mul(model, _MatrixArray[i]);
    }
    float3 fragPos = mul(model, float4(_Position, 1.0)).xyz;
    float3 normal = normalize(mul(model, float4(_Normal, 0.0)).xyz);
    float2 texCoord = uv;

    float4x4 mvp = mul(mul(_MatrixVP, model), _Matrix9);
    float4 clipPos = mul(mvp, float4(_Position, 1.0));

    float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
    float3 viewDir = normalize(_WorldSpaceCameraPos - fragPos);
    float3 halfDir = normalize(lightDir + viewDir);
    float ndotl = max(dot(normal, lightDir), 0.0);
    float ndoth = max(dot(normal, halfDir), 0.0);
    float fresnel = pow(1.0 - dot(viewDir, halfDir), 5.0);
    float specular = pow(ndoth, _Gloss) * _LightColor0.rgb * _SpecularColor.rgb * _SpecularIntensity;
    float4 diffuse = _LightColor0 * _Color.rgb * ndotl;

    float4 finalColor = float4(0, 0, 0, 0);
    finalColor.rgb = lerp(_Color.rgb, _RimColor.rgb, fresnel);
    finalColor.rgb += _AmbientColor.rgb * _AmbientIntensity;
    finalColor.rgb += diffuse.rgb * _LightIntensity;
    finalColor.rgb += specular.rgb;

    float fogFactor = saturate((fragPos.z - _FogStart) / (_FogEnd - _FogStart));
    finalColor.rgb = lerp(finalColor.rgb, _FogColor.rgb, fogFactor);
    
    // Textures
    float4 tex0 = tex2D(_MainTex, texCoord);
    float4 tex1 = tex2D(_MainTex1, texCoord);
    float4 tex2 = tex2D(_MainTex2, texCoord);
    float4 tex3 = tex2D(_MainTex3, texCoord);
    float4 tex4 = tex2D(_MainTex4, texCoord);
    float4 tex5 = tex2D(_MainTex5, texCoord);
    float4 tex6 = tex2D(_MainTex6, texCoord);
    float4 tex7 = tex2D(_MainTex7, texCoord);


    // Combine textures
    float4 albedo = tex0 * _MainTexStrength;
    albedo.rgb = lerp(albedo.rgb, tex1.rgb, tex1.a);
    albedo.rgb = lerp(albedo.rgb, tex2.rgb, tex2.a);
    albedo.rgb = lerp(albedo.rgb, tex3.rgb, tex3.a);
    albedo.rgb = lerp(albedo.rgb, tex4.rgb, tex4.a);
    albedo.rgb = lerp(albedo.rgb, tex5.rgb, tex5.a);
    albedo.rgb = lerp(albedo.rgb, tex6.rgb, tex6.a);
   
    fixed4 frag (v2f i) : SV_Target
{
    // Sample textures
    fixed4 tex0 = tex2D(_MainTex, i.uv);
    fixed4 tex1 = tex2D(_MainTex1, i.uv);
    fixed4 tex2 = tex2D(_MainTex2, i.uv);
    fixed4 tex3 = tex2D(_MainTex3, i.uv);
    fixed4 tex4 = tex2D(_MainTex4, i.uv);
    fixed4 tex5 = tex2D(_MainTex5, i.uv);
    fixed4 tex6 = tex2D(_MainTex6, i.uv);
    fixed4 tex7 = tex2D(_MainTex7, i.uv);

    // Do lighting calculations
    float3 worldPos = mul(_Matrix9, float4(i.vertex, 1)).xyz;
    float3 worldNormal = normalize(mul(_Matrix9, float4(i.normal, 0)).xyz);
    float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);

    float4 mainColor = tex0 * _Color;
    float4 emissive = tex1 * _EmissionColor;
    float metallic = tex2.r * _Metallic;
    float smoothness = tex3.r * _Smoothness;
    float3 specularColor = tex4.rgb * _SpecularColor.rgb;
    float3 ambientColor = tex5.rgb * _AmbientColor.rgb;

    float3 diffuseColor = (1.0 - metallic) * mainColor.rgb;
    float3 specular = specularColor * _LightColor0.rgb;
    float4 finalColor = float4(0, 0, 0, mainColor.a);

    float ndotl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
    float ndots = max(0, dot(worldNormal, worldViewDir));
    float3 h = normalize(_WorldSpaceLightPos0.xyz + worldViewDir);
    float ndoth = max(0, dot(worldNormal, h));
    float specularTerm = pow(ndoth, smoothness) * (smoothness + 2.0) / 8.0; 
    specular *= specularTerm;

    finalColor.rgb += diffuseColor * ndotl;
    finalColor.rgb += specular;
    finalColor.rgb += ambientColor * _AmbientFactor;

    // Apply fog
    float fogFactor = 1.0 - saturate((i.fogCoord - _FogStart) / (_FogEnd - _FogStart));
    finalColor.rgb = lerp(finalColor.rgb, _FogColor.rgb, fogFactor);

    // Apply emissive
    finalColor.rgb += emissive.rgb;

    // Apply texture 6 as overlay
    finalColor.rgb = lerp(finalColor.rgb, tex6.rgb, tex6.a);

    return finalColor;
}
}