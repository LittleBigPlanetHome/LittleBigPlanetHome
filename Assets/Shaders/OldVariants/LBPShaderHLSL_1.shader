Shader "LBP" {
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
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf StandardSpecular

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
        float3 _CamPos;
        uniform float4 _AmbientColor;
        uniform float4 _FogColor;
        uniform float4 _SunColor;
        float3 _SunPosition;
        uniform float4 _RimColor;
        uniform float4 _RimColor2;
        float4 _ThingColor;

        struct Input {
            float3 worldPos;
            float3 worldNormal;
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
            float4 mainColor = tex2D(_MainTex, IN.uv_MainTex);
            float4 secondColor = tex2D(_SecondTex, IN.uv_MainTex);
            float4 thirdColor = tex2D(_ThirdTex, IN.uv_MainTex);
            float4 fourthColor = tex2D(_FourthTex, IN.uv_MainTex);
            float4 fifthColor = tex2D(_FifthTex, IN.uv_MainTex);
            float4 sixthColor = tex2D(_SixthTex, IN.uv_MainTex);
            float4 seventhColor = tex2D(_SeventhTex, IN.uv_MainTex);
            float4 eighthColor = tex2D(_EighthTex, IN.uv_MainTex);
                    float4 thing_color = _ThingColor;

        float4 baseColor = mainColor * thing_color;
        float4 secondColorMod = secondColor * thing_color;
        float4 thirdColorMod = thirdColor * thing_color;
        float4 fourthColorMod = fourthColor * thing_color;
        float4 fifthColorMod = fifthColor * thing_color;
        float4 sixthColorMod = sixthColor * thing_color;
        float4 seventhColorMod = seventhColor * thing_color;
        float4 eighthColorMod = eighthColor * thing_color;

        float4 colorBuffer = tex2D(_ColorBuffer, IN.uv_MainTex);
        float4 shadowColor = tex2D(_ShadowTex, IN.uv_MainTex);

        float4 finalColor = baseColor;
        finalColor += secondColorMod;
        finalColor += thirdColorMod;
        finalColor += fourthColorMod;
        finalColor += fifthColorMod;
        finalColor += sixthColorMod;
        finalColor += seventhColorMod;
        finalColor += eighthColorMod;

        // Ambient
        float ambientStrength = 0.15;
        o.Albedo = finalColor.rgb * (1 - ambientStrength) + _AmbientColor.rgb * ambientStrength;
        o.Alpha = finalColor.a;

        // Shadows
        // float shadowStrength = 0.5;
        // o.Metallic = shadowColor.r * shadowStrength;

        // Rim lighting
        float3 rimcol = _RimColor.rgb;
        float3 rimcol2 = _RimColor2.rgb;
        float3 normal = normalize(IN.worldNormal);
        float3 viewDir = normalize(_CamPos.xyz - IN.worldPos);
        float rim = 1 - dot(normal, viewDir);
        rim = 1 - rim * rim;
        float3 rimColor = lerp(rimcol, rimcol2, pow(1.0f - dot(normalize(viewDir), normal), 1.0f - rim));
        o.Emission = rimColor;

        // Fog
        float fogStart = 10;
        float fogEnd = 50;
        float fogRange = 1 / (fogEnd - fogStart);
        float fogDensity = saturate((IN.worldPos.z - fogStart) * fogRange);
        o.Albedo = lerp(o.Albedo, _FogColor.rgb, fogDensity);

        // Sunlight
        float3 sunPos = _SunPosition;
        float3 lightDir = normalize(sunPos - IN.worldPos);
        float sunStrength = dot(IN.worldNormal, lightDir);
        float3 sunColor = _SunColor.rgb;
        float3 sunLight = sunColor * sunStrength;
        o.Albedo += sunLight;

        // Specular
        float specularStrength = 0.5;
        float3 viewDirection = normalize(_CamPos.xyz - IN.worldPos);
        float3 reflectedLight = reflect(-lightDir, IN.worldNormal);
        float specularIntensity = pow(saturate(dot(viewDirection, reflectedLight)), 20);
        o.Specular = sunColor * specularStrength * specularIntensity;
    }
    ENDCG
}
FallBack "Diffuse"
}