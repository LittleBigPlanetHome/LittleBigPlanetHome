struct VS_INPUT
{
    float3 aPos : POSITION;
    float2 aTexCoord : TEXCOORD0;
    float3 aNormal : NORMAL;
};

struct VS_OUTPUT
{
    float4 FragPos : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
    float3 Normal : TEXCOORD1;
};

VS_OUTPUT main(VS_INPUT input)
{
    VS_OUTPUT output;

    float4x4 view : register(c0);
    float4x4 projection : register(c4);
    float4x4 light : register(c8);
    float4x4 matrices[100] : register(c12);
    float3 campos : register(c112);
    float3 ambcol : register(c113);
    float3 fogcol : register(c114);
    float3 suncol : register(c115);
    float3 sunpos : register(c116);
    float3 rimcol : register(c117);
    float3 rimcol2 : register(c118);
    float lightscaleadd : register(c119);
    float4 thing_color : register(c120);
    Texture2D s0 : register(t0);
    Texture2D s1 : register(t1);
    Texture2D s2 : register(t2);
    Texture2D s3 : register(t3);
    Texture2D s4 : register(t4);
    Texture2D s5 : register(t5);
    Texture2D s6 : register(t6);
    Texture2D s7 : register(t7);
    Texture2D shadowtex : register(t8);
    Texture2D cbuf : register(t9);

    float4x4 model = float4x4(1.0f, 0.0f, 0.0f, 0.0f,
                              0.0f, 1.0f, 0.0f, 0.0f,
                              0.0f, 0.0f, 1.0f, 0.0f,
                              0.0f, 0.0f, 0.0f, 1.0f);

    for (int i = 0; i < 100; i++)
    {
        model = mul(model, matrices[i]);
    }

    output.FragPos = mul(mul(mul(projection, view), model), float4(input.aPos, 1.0f));
    output.Normal = normalize(mul(input.aNormal, (float3x3)transpose(inverse(model))));
    output.TexCoord = input.aTexCoord;

    float3 FragPos = mul(model, float4(input.aPos, 1.0f)).xyz;

    float3 lightPos = mul(light, float4(0.0f, 0.0f, 0.0f, 1.0f)).xyz;
    float distance = length(lightPos - FragPos);
    float3 lightDir = normalize(lightPos - FragPos);
    float3 normal = normalize(output.Normal);
    float diffuse = max(dot(normal, lightDir), 0.0f);
    float3 diffuseColor = suncol * diffuse;

    float ambientStrength = 0.15f;
    float3 ambient = ambcol * ambientStrength;

    float3 viewDir = normalize(campos - FragPos);
    float3 reflectDir = reflect(-lightDir, normal);
float specularStrength = 0.5f;
float specular = pow(max(dot(viewDir, reflectDir), 0.0f), 32.0f) * specularStrength;
vec3 specularColor = suncol * specular;

float fogDist = length(FragPos - campos);
float fogStrength = clamp((fogDist - 20.0f) / 20.0f, 0.0f, 1.0f);
vec3 fogColor = mix(fogcol, vec3(1.0f), fogStrength);

float shadow = texture(shadowtex, vec3(TexCoord, 0.0f)).r;
vec4 c0 = texture(cbuf, vec2(0.5f, shadow));

vec4 s0 = texture(s0, TexCoord);
vec4 s1 = texture(s1, TexCoord);
vec4 s2 = texture(s2, TexCoord);
vec4 s3 = texture(s3, TexCoord);
vec4 s4 = texture(s4, TexCoord);
vec4 s5 = texture(s5, TexCoord);
vec4 s6 = texture(s6, TexCoord);
vec4 s7 = texture(s7, TexCoord);

vec4 color = mix(c0, mix(mix(s0, s1, 0.5f), mix(s2, s3, 0.5f), 0.5f), c0.a);
color = mix(color, mix(mix(s4, s5, 0.5f), mix(s6, s7, 0.5f), 0.5f), color.a);

FragColor = vec4(color.rgb * (diffuseColor + ambient + specularColor + rimColor) * (1.0f - shadow) + shadow * 0.5f * vec3(0.1f, 0.1f, 0.1f) + fogColor, thing_color.a);

if (thing_color.a < 1.0f) {
if (thing_color.a <= 0.0f) {
discard;
}
FragColor.rgb *= thing_color.a;
}