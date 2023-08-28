// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/MyShader" {
    Properties {
        _CamPos("Camera Position", Vector) = (0,0,0,0)
        _AmbCol("Ambient Color", Color) = (1,1,1,1)
        _FogCol("Fog Color", Color) = (0,0,0,0)
        _SunCol("Sun Color", Color) = (1,1,1,1)
        _SunPos("Sun Position", Vector) = (0,0,0,0)
        _RimCol("Rim Color", Color) = (1,1,1,1)
        _RimCol2("Rim Color 2", Color) = (1,1,1,1)
        _LightScaleAdd("Light Scale Add", Float) = 1.0
        _ThingColor("Thing Color", Color) = (1,1,1,1)
        _S0("Texture 0", 2D) = "white" {}
        _S1("Texture 1", 2D) = "white" {}
        _S2("Texture 2", 2D) = "white" {}
        _S3("Texture 3", 2D) = "white" {}
        _S4("Texture 4", 2D) = "white" {}
        _S5("Texture 5", 2D) = "white" {}
        _S6("Texture 6", 2D) = "white" {}
        _S7("Texture 7", 2D) = "white" {}
        _ShadowTex("Shadow Texture", 2D) = "white" {}
        _CbufTex("Custom Buffer", 2D) = "white" {}
    }
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float3 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float3 normal : NORMAL;
            };
            
            struct v2f {
                float4 pos : SV_POSITION;
                float3 fragpos : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float2 texcoord : TEXCOORD2;
            };
            
            v2f vert(appdata v) {
                v2f o;
                float model = float(1.0);
                for (int i = 0; i < 100; i++) {
                    model = mul(model, _Matrices[i]); 
                }
                o.fragpos = mul(model, float4(v.vertex, 1.0)).xyz;
                o.normal = mul(float3x3(transpose(inverse(model))), v.normal);
                o.texcoord = v.texcoord;
                o.pos = UnityObjectToClipPos(float4(v.vertex, 1.0));
                return o;
            }
            
            float4 _ThingColor;
            sampler2D _S0;
            sampler2D _S1;
            sampler2D _S2;
            sampler2D _S3;
            sampler2D _S4;
            sampler2D _S5;
            sampler2D _S6;
            sampler2D _S7;
            sampler2D _ShadowTex;
sampler2D _CbufTex;
float _ProjInfo[3];

float Linear01Depth(float depth)
{
float z = depth * 2.0 - 1.0;
return _ProjInfo[2] / (_ProjInfo[0] * z + _ProjInfo[1]);
}

float CalculateShadowFactor(vec4 fragPosLightSpace, vec3 normal)
{
vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
projCoords = projCoords * 0.5 + 0.5;
float closestDepth = texture(_ShadowTex, projCoords.xy).r;
float currentDepth = Linear01Depth(projCoords.z);
float shadow = 0.0;
vec3 normalCam = normalize(mat3(view) * normal);
vec3 fragToLight = normalize(vec3(fragPosLightSpace) - FragPos);
float bias = max(0.05 * (1.0 - dot(normalCam, fragToLight)), 0.005);
vec2 texelSize = 1.0 / textureSize(_ShadowTex, 0);
for (int x = -1; x <= 1; ++x)
{
for (int y = -1; y <= 1; ++y)
{
float pcfDepth = textureOffset(_ShadowTex, projCoords.xy, ivec2(x, y)).r;
shadow += currentDepth - bias > Linear01Depth(pcfDepth) ? 1.0 : 0.0;
}
}
shadow /= 9.0;
if (projCoords.z > 1.0 || projCoords.z < 0.0)
    shadow = 1.0;

return shadow;
}

void main()
{
mat4 model = mat4(1.0);
for (int i = 0; i < 100; i++)
{
model = model * unity_ObjectToWorld[i];
}
vec4 worldPos = model * vec4(aPos, 1.0);
vec4 clipPos = _Object2Clip * vec4(worldPos.xyz, 1.0);
vec3 normal = normalize(mat3(transpose(inverse(model))) * aNormal);

vec3 viewDir = normalize(_WorldSpaceCameraPos - worldPos.xyz);

vec3 baseColor = texture(_MainTex, aTexCoord).rgb;

float ao = texture(_AO, aTexCoord).r;
vec3 ambient = ao * _AmbientColor.rgb;

vec3 sunDir = normalize(_WorldSpaceLightPos0.xyz);
float diffuse = max(dot(normal, sunDir), 0.0);
vec3 sunColor = _LightColor0.rgb * diffuse;

vec3 rimColor = mix(_RimColor.rgb, _RimColor2.rgb, pow(1.0 - dot(normalize(viewDir), normal), 1.5));

vec3 shadowColor = vec3(1.0);
if (_ReceiveShadows && _ShadowTex)
{
    vec4 fragPosLightSpace = _ShadowMapMatrix * worldPos;
    shadowColor = vec3(1.0 - CalculateShadowFactor(fragPosLightSpace, normal));
}

float specularStrength = _SpecularStrength;
vec3 reflectDir = reflect(-sunDir, normal);
float spec = pow(max(dot(viewDir, reflectDir), 0.0), _Smoothness);
vec3 specular = specularStrength * spec * _LightColor
    // Calculate specular
    float shininess = _Gloss;
    float specularStrength = _SpecularStrength;
    vec3 specular = specularStrength * spec * _LightColor;
    if (dot(normal, _WorldSpaceLightPos0.xyz) > 0.0) {
        specular *= pow(max(dot(reflect(_WorldSpaceLightPos0.xyz, normal), viewDir), 0.0), shininess);
    } else {
        specular *= pow(max(dot(reflect(-_WorldSpaceLightPos0.xyz, normal), viewDir), 0.0), shininess);
    }

    // Calculate rim lighting
    float rim = 1.0 - max(dot(normalize(viewDir), normal), 0.0);
    float rimLight = 1.0 - pow(rim, _RimPower);
    vec3 rimColor = _RimColor.rgb * rimLight * _LightColor.rgb;

    // Combine lighting and textures
    float shadow = shadowCoord.z > texture(_ShadowTex, shadowCoord.xy).r ? 0.0 : 1.0;
    float lightScale = _LightScale + _LightScaleAdd;
    float brightness = pow(texture(_Cbuf, i.uv).a, 2.0);
    float ambient = _AmbientIntensity * _AmbientColor.a;
    float light = max(dot(normal, _WorldSpaceLightPos0.xyz), 0.0) * lightScale;
    float light2 = max(dot(normal, _WorldSpaceLightPos1.xyz), 0.0) * lightScale;
    float light3 = max(dot(normal, _WorldSpaceLightPos2.xyz), 0.0) * lightScale;
    float light4 = max(dot(normal, _WorldSpaceLightPos3.xyz), 0.0) * lightScale;
    vec4 col0 = texture(_S0, TexCoord) * light * brightness;
    vec4 col1 = texture(_S1, TexCoord) * light * brightness;
    vec4 col2 = texture(_S2, TexCoord) * light2 * brightness;
    vec4 col3 = texture(_S3, TexCoord) * light3 * brightness;
    vec4 col4 = texture(_S4, TexCoord) * light4 * brightness;
    vec4 col5 = texture(_S5, TexCoord) * ambient * brightness;
    vec4 col6 = texture(_S6, TexCoord) * _AmbientColor * brightness;
    vec4 col7 = texture(_S7, TexCoord) * specular * shadow * brightness;
    vec4 finalColor = col0 + col1 + col2 + col3 + col4 + col5 + col6 + col7 + rimColor;
    finalColor.a = _Color.a;

    // Output final color
    return finalColor;
            }
       
            ENDCG
        }
    }
}
 