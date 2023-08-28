Shader "LittleBigPlanet HUB/LittleBigPlanet Shader (old)" {
	Properties {
		_DiffuseTexture ("Diffuse Texture", 2D) = "white" {}
		_Emm ("Emission", 2D) = "black" {}
		_emmBrightness ("Emission Brightness", Float) = 0
		_EffectAmount ("Cubemap Metallic", Range(2, 0)) = 1
		_DiffuseColor ("Diffuse Color", Vector) = (1,1,1,1)
		_NormalTexture ("Normal Texture", 2D) = "bump" {}
		_NormalStrength ("Normal Strength", Float) = 0
		_SpecularTexture ("Specular Texture", 2D) = "white" {}
		_SpecularPower ("Specular Power", Float) = 32
		_Fal ("Dirt Texture", 2D) = "white" {}
		_Fal2 ("Secondary Dirt Texture", 2D) = "white" {}
		_FalloffExponent ("Falloff Brightness", Float) = 1
		_FalloffExponent2 ("Falloff Exponent", Float) = 1.3
		_Sphere ("Reflection Spheremap", 2D) = "black" {}
		_ReflectColor ("Reflection Color", Vector) = (1,1,1,1)
		_ReflectStrength ("Reflection Strength", Float) = 0.15
		_FresnelStrength ("Fresnel Strength", Float) = 0.5
		_SpecularBrightness ("Specular Brightness", Float) = 1
		_MaxBrightness ("Max Brightness", Float) = 999999
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Pass {
			Name "FORWARD"
			Tags { "LIGHTMODE" = "FORWARDBASE" "RenderType" = "Opaque" "SHADOWSUPPORT" = "true" }
			GpuProgramID 36257
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 texcoord3 : TEXCOORD3;
				float4 texcoord4 : TEXCOORD4;
				float4 texcoord5 : TEXCOORD5;
				float2 texcoord6 : TEXCOORD6;
				float3 texcoord7 : TEXCOORD7;
				float4 texcoord8 : TEXCOORD8;
				float3 texcoord9 : TEXCOORD9;
				float4 texcoord11 : TEXCOORD11;
				float4 texcoord12 : TEXCOORD12;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float4 _Emm_ST;
			float4 _Fal_ST;
			float4 _Fal2_ST;
			float4 _DiffuseTexture_ST;
			float4 _NormalTexture_ST;
			float4 _SpecularTexture_ST;
			// $Globals ConstantBuffers for Fragment Shader
			float4 _LightColor0;
			float4 _ReflectColor;
			float4 _DiffuseColor;
			float _SpecularPower;
			float4 _Falloff;
			float _FalloffExponent;
			float _FalloffExponent2;
			float _NormalStrength;
			float _emmBrightness;
			float _SpecularBrightness;
			float _ReflectStrength;
			float _MaxBrightness;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _Fal;
			sampler2D _Fal2;
			sampler2D _DiffuseTexture;
			sampler2D _Emm;
			sampler2D _SpecularTexture;
			sampler2D _NormalTexture;
			sampler2D _Sphere;
			
			// Keywords: DIRECTIONAL
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                float4 tmp3;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp1 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp0.xyz = unity_ObjectToWorld._m03_m13_m23 * v.vertex.www + tmp0.xyz;
                tmp2 = tmp1.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp2 = unity_MatrixVP._m00_m10_m20_m30 * tmp1.xxxx + tmp2;
                tmp2 = unity_MatrixVP._m02_m12_m22_m32 * tmp1.zzzz + tmp2;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp1.wwww + tmp2;
                o.texcoord.xy = v.texcoord.xy * _Emm_ST.xy + _Emm_ST.zw;
                o.texcoord.zw = v.texcoord.xy * _Fal_ST.xy + _Fal_ST.zw;
                o.texcoord1.xy = v.texcoord.xy * _Fal2_ST.xy + _Fal2_ST.zw;
                o.texcoord1.zw = v.texcoord.xy * _DiffuseTexture_ST.xy + _DiffuseTexture_ST.zw;
                o.texcoord2.xy = v.texcoord.xy * _NormalTexture_ST.xy + _NormalTexture_ST.zw;
                o.texcoord2.zw = v.texcoord1.xy * _SpecularTexture_ST.xy + _SpecularTexture_ST.zw;
                o.texcoord3.w = tmp0.x;
                tmp1.y = dot(v.normal.xyz, unity_WorldToObject._m00_m10_m20);
                tmp1.z = dot(v.normal.xyz, unity_WorldToObject._m01_m11_m21);
                tmp1.x = dot(v.normal.xyz, unity_WorldToObject._m02_m12_m22);
                tmp0.x = dot(tmp1.xyz, tmp1.xyz);
                tmp0.x = rsqrt(tmp0.x);
                tmp1.xyz = tmp0.xxx * tmp1.xyz;
                tmp2.xyz = v.tangent.yyy * unity_ObjectToWorld._m11_m21_m01;
                tmp2.xyz = unity_ObjectToWorld._m10_m20_m00 * v.tangent.xxx + tmp2.xyz;
                tmp2.xyz = unity_ObjectToWorld._m12_m22_m02 * v.tangent.zzz + tmp2.xyz;
                tmp0.x = dot(tmp2.xyz, tmp2.xyz);
                tmp0.x = rsqrt(tmp0.x);
                tmp2.xyz = tmp0.xxx * tmp2.xyz;
                tmp3.xyz = tmp1.xyz * tmp2.xyz;
                tmp3.xyz = tmp1.zxy * tmp2.yzx + -tmp3.xyz;
                tmp0.x = v.tangent.w * unity_WorldTransformParams.w;
                tmp3.xyz = tmp0.xxx * tmp3.xyz;
                o.texcoord3.y = tmp3.x;
                o.texcoord3.x = tmp2.z;
                o.texcoord3.z = tmp1.y;
                o.texcoord4.x = tmp2.x;
                o.texcoord5.x = tmp2.y;
                o.texcoord4.z = tmp1.z;
                o.texcoord5.z = tmp1.x;
                o.texcoord4.w = tmp0.y;
                o.texcoord5.w = tmp0.z;
                o.texcoord4.y = tmp3.y;
                o.texcoord5.y = tmp3.z;
                tmp0 = unity_WorldToObject._m01_m11_m21_m31 * unity_MatrixInvV._m10_m10_m10_m10;
                tmp0 = unity_WorldToObject._m00_m10_m20_m30 * unity_MatrixInvV._m00_m00_m00_m00 + tmp0;
                tmp0 = unity_WorldToObject._m02_m12_m22_m32 * unity_MatrixInvV._m20_m20_m20_m20 + tmp0;
                tmp0 = unity_WorldToObject._m03_m13_m23_m33 * unity_MatrixInvV._m30_m30_m30_m30 + tmp0;
                tmp1.xyz = v.normal.xyz;
                tmp1.w = 1.0;
                o.texcoord6.x = dot(tmp0, tmp1);
                tmp0 = unity_WorldToObject._m01_m11_m21_m31 * unity_MatrixInvV._m11_m11_m11_m11;
                tmp0 = unity_WorldToObject._m00_m10_m20_m30 * unity_MatrixInvV._m01_m01_m01_m01 + tmp0;
                tmp0 = unity_WorldToObject._m02_m12_m22_m32 * unity_MatrixInvV._m21_m21_m21_m21 + tmp0;
                tmp0 = unity_WorldToObject._m03_m13_m23_m33 * unity_MatrixInvV._m31_m31_m31_m31 + tmp0;
                o.texcoord6.y = dot(tmp0, tmp1);
                o.texcoord7.xyz = v.color.xyz;
                o.texcoord8 = float4(0.0, 0.0, 0.0, 0.0);
                o.texcoord9.xyz = float3(0.0, 0.0, 0.0);
                o.texcoord11 = float4(0.0, 0.0, 0.0, 0.0);
                o.texcoord12 = float4(0.0, 0.0, 0.0, 0.0);
                return o;
			}
			// Keywords: DIRECTIONAL
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                float4 tmp3;
                float4 tmp4;
                float4 tmp5;
                float4 tmp6;
                float4 tmp7;
                float4 tmp8;
                float4 tmp9;
                tmp0 = tex2D(_NormalTexture, inp.texcoord2.xy);
                tmp0.xy = tmp0.wx * float2(-1.0, 1.0) + float2(0.5, -0.5);
                tmp0.xy = tmp0.xy * _NormalStrength.xx + float2(0.5, 0.5);
                tmp0.xy = tmp0.xy * float2(2.0, 2.0) + float2(-1.0, -1.0);
                tmp0.w = dot(tmp0.xy, tmp0.xy);
                tmp0.w = min(tmp0.w, 1.0);
                tmp0.w = 1.0 - tmp0.w;
                tmp0.z = sqrt(tmp0.w);
                tmp1.x = dot(inp.texcoord3.xyz, tmp0.xyz);
                tmp1.z = dot(inp.texcoord5.xyz, tmp0.xyz);
                tmp1.y = dot(inp.texcoord4.xyz, tmp0.xyz);
                tmp0.w = dot(tmp1.xyz, tmp1.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp2.xyz = tmp0.www * tmp1.xyz;
                tmp2.w = tmp1.y * tmp0.w + 0.5;
                tmp1.xyz = tmp2.xwz * float3(0.85, 0.85, 0.85);
                tmp0.w = saturate(dot(tmp1.xyz, _WorldSpaceLightPos0.xyz));
                tmp1.x = inp.texcoord3.w;
                tmp1.y = inp.texcoord4.w;
                tmp1.z = inp.texcoord5.w;
                tmp1.xyz = _WorldSpaceCameraPos - tmp1.xyz;
                tmp1.w = dot(tmp1.xyz, tmp1.xyz);
                tmp1.w = rsqrt(tmp1.w);
                tmp3.xyz = tmp1.xyz * tmp1.www + _WorldSpaceLightPos0.xyz;
                tmp1.xyz = tmp1.www * tmp1.xyz;
                tmp1.w = dot(tmp3.xyz, tmp3.xyz);
                tmp1.w = rsqrt(tmp1.w);
                tmp3.xyz = tmp1.www * tmp3.xyz;
                tmp1.w = dot(tmp2.xyz, tmp3.xyz);
                tmp1.w = max(tmp1.w, 0.0);
                tmp1.w = log(tmp1.w);
                tmp1.w = tmp1.w * _SpecularPower;
                tmp1.w = exp(tmp1.w);
                tmp2.xyz = tmp1.www * _LightColor0.xyz;
                tmp3 = tex2D(_SpecularTexture, inp.texcoord2.zw);
                tmp3.xyz = tmp3.xyz * _SpecularBrightness.xxx;
                tmp4 = tex2D(_DiffuseTexture, inp.texcoord1.zw);
                tmp5.xyz = tmp4.xyz * float3(0.5, 0.5, 0.5) + float3(0.5, 0.5, 0.5);
                tmp6.xyz = tmp3.xyz * tmp5.xyz;
                tmp3.xyz = tmp3.xyz * tmp5.xyz + float3(0.5, 0.5, 0.5);
                tmp2.xyz = tmp2.xyz * tmp6.xyz;
                tmp5 = tex2D(_Fal, inp.texcoord.zw);
                tmp6 = tex2D(_Fal2, inp.texcoord1.xy);
                tmp5.xyz = tmp5.xyz * tmp6.xyz;
                tmp6.xyz = tmp4.xyz * tmp5.xyz;
                tmp7.xyz = -tmp6.xyz * _DiffuseColor.xyz + float3(1.5, 1.5, 1.5);
                tmp6.xyz = tmp6.xyz * _DiffuseColor.xyz;
                tmp2.xyz = tmp2.xyz * tmp7.xyz;
                tmp2.xyz = tmp0.www * tmp2.xyz;
                tmp2.xyz = tmp0.www * tmp2.xyz;
                tmp7.xyz = tmp0.www * _LightColor0.xyz;
                tmp7.xyz = tmp6.xyz * tmp7.xyz;
                tmp2.xyz = tmp7.xyz * tmp0.www + tmp2.xyz;
                tmp7.xy = inp.texcoord6.xy * float2(1.0, -1.0) + float2(0.0, 1.0);
                tmp7.xy = tmp7.xy * float2(0.5, 0.5) + float2(0.5, 0.5);
                tmp7 = tex2D(_Sphere, tmp7.xy);
                tmp8.xyz = tmp7.xyz * _ReflectColor.xyz;
                tmp7.xyz = tmp7.xyz - float3(0.75, 0.75, 0.75);
                tmp7.xyz = max(tmp7.xyz, float3(0.0, 0.0, 0.0));
                tmp9.xyz = tmp4.xyz * float3(0.75, 0.75, 0.75) + float3(0.25, 0.25, 0.25);
                tmp8.xyz = tmp8.xyz * tmp9.xyz;
                tmp8.xyz = tmp4.www * tmp8.xyz;
                tmp4.xyz = float3(1.0, 1.0, 1.0) - tmp4.xyz;
                tmp4.xyz = tmp4.xyz * tmp7.xyz;
                tmp4.xyz = tmp4.xyz + tmp4.xyz;
                tmp7.xyz = tmp5.xyz * tmp8.xyz;
                tmp7.xyz = tmp7.xyz * float3(0.75, 0.75, 0.75);
                tmp2.xyz = tmp7.xyz * _ReflectStrength.xxx + tmp2.xyz;
                tmp7.xyz = tmp1.yyy * inp.texcoord4.xyz;
                tmp1.xyw = inp.texcoord3.xyz * tmp1.xxx + tmp7.xyz;
                tmp1.xyz = inp.texcoord5.xyz * tmp1.zzz + tmp1.xyw;
                tmp0.x = dot(tmp1.xyz, tmp0.xyz);
                tmp0.x = 1.0 - tmp0.x;
                tmp0.x = log(tmp0.x);
                tmp0.x = tmp0.x * _FalloffExponent2;
                tmp0.x = exp(tmp0.x);
                tmp0.x = tmp0.x * _FalloffExponent;
                tmp0.xyz = tmp3.xyz * tmp0.xxx;
                tmp0.xyz = tmp0.xyz * float3(0.5, 0.5, 0.5);
                tmp0.xyz = tmp0.xyz * _Falloff.xyz;
                tmp0.xyz = tmp5.xyz * tmp0.xyz;
                tmp0.xyz = tmp0.xyz * tmp5.xyz + tmp2.xyz;
                tmp1 = tex2D(_Emm, inp.texcoord.xy);
                tmp2.xyz = tmp1.xyz * float3(0.75, 0.75, 0.75) + float3(0.25, 0.25, 0.25);
                tmp1.xyz = tmp1.xyz * tmp2.xyz;
                tmp1.xyz = tmp5.xyz * tmp1.xyz;
                tmp0.xyz = tmp1.xyz * _emmBrightness.xxx + tmp0.xyz;
                tmp0.xyz = max(tmp0.xyz, -_MaxBrightness.xxx);
                tmp0.xyz = min(tmp0.xyz, _MaxBrightness.xxx);
                tmp0.xyz = tmp6.xyz * inp.texcoord9.xyz + tmp0.xyz;
                o.sv_target.xyz = tmp4.xyz * tmp5.xyz + tmp0.xyz;
                o.sv_target.w = 1.0;
                return o;
			}
			ENDCG
		}
		Pass {
			Name "FORWARD"
			Tags { "LIGHTMODE" = "FORWARDADD" "RenderType" = "Opaque" }
			Blend One One, One One
			ZWrite Off
			GpuProgramID 87392
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float3 texcoord3 : TEXCOORD3;
				float3 texcoord4 : TEXCOORD4;
				float3 texcoord5 : TEXCOORD5;
				float3 texcoord6 : TEXCOORD6;
				float2 texcoord7 : TEXCOORD7;
				float3 texcoord8 : TEXCOORD8;
				float4 texcoord9 : TEXCOORD9;
				float3 texcoord10 : TEXCOORD10;
				float4 texcoord11 : TEXCOORD11;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float4x4 unity_WorldToLight;
			float4 _Emm_ST;
			float4 _Fal_ST;
			float4 _Fal2_ST;
			float4 _DiffuseTexture_ST;
			float4 _NormalTexture_ST;
			float4 _SpecularTexture_ST;
			// $Globals ConstantBuffers for Fragment Shader
			float4 _LightColor0;
			float4 _ReflectColor;
			float4 _DiffuseColor;
			float _SpecularPower;
			float4 _Falloff;
			float _FalloffExponent;
			float _FalloffExponent2;
			float _NormalStrength;
			float _emmBrightness;
			float _SpecularBrightness;
			float _ReflectStrength;
			float _MaxBrightness;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _Fal;
			sampler2D _Fal2;
			sampler2D _DiffuseTexture;
			sampler2D _Emm;
			sampler2D _SpecularTexture;
			sampler2D _NormalTexture;
			sampler2D _Sphere;
			sampler2D _LightTexture0;
			
			// Keywords: POINT
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                float4 tmp3;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp1 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp2 = tmp1.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp2 = unity_MatrixVP._m00_m10_m20_m30 * tmp1.xxxx + tmp2;
                tmp2 = unity_MatrixVP._m02_m12_m22_m32 * tmp1.zzzz + tmp2;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp1.wwww + tmp2;
                o.texcoord.xy = v.texcoord.xy * _Emm_ST.xy + _Emm_ST.zw;
                o.texcoord.zw = v.texcoord.xy * _Fal_ST.xy + _Fal_ST.zw;
                o.texcoord1.xy = v.texcoord.xy * _Fal2_ST.xy + _Fal2_ST.zw;
                o.texcoord1.zw = v.texcoord.xy * _DiffuseTexture_ST.xy + _DiffuseTexture_ST.zw;
                o.texcoord2.xy = v.texcoord.xy * _NormalTexture_ST.xy + _NormalTexture_ST.zw;
                o.texcoord2.zw = v.texcoord1.xy * _SpecularTexture_ST.xy + _SpecularTexture_ST.zw;
                tmp1.y = dot(v.normal.xyz, unity_WorldToObject._m00_m10_m20);
                tmp1.z = dot(v.normal.xyz, unity_WorldToObject._m01_m11_m21);
                tmp1.x = dot(v.normal.xyz, unity_WorldToObject._m02_m12_m22);
                tmp1.w = dot(tmp1.xyz, tmp1.xyz);
                tmp1.w = rsqrt(tmp1.w);
                tmp1.xyz = tmp1.www * tmp1.xyz;
                tmp2.xyz = v.tangent.yyy * unity_ObjectToWorld._m11_m21_m01;
                tmp2.xyz = unity_ObjectToWorld._m10_m20_m00 * v.tangent.xxx + tmp2.xyz;
                tmp2.xyz = unity_ObjectToWorld._m12_m22_m02 * v.tangent.zzz + tmp2.xyz;
                tmp1.w = dot(tmp2.xyz, tmp2.xyz);
                tmp1.w = rsqrt(tmp1.w);
                tmp2.xyz = tmp1.www * tmp2.xyz;
                tmp3.xyz = tmp1.xyz * tmp2.xyz;
                tmp3.xyz = tmp1.zxy * tmp2.yzx + -tmp3.xyz;
                tmp1.w = v.tangent.w * unity_WorldTransformParams.w;
                tmp3.xyz = tmp1.www * tmp3.xyz;
                o.texcoord3.y = tmp3.x;
                o.texcoord3.x = tmp2.z;
                o.texcoord3.z = tmp1.y;
                o.texcoord4.x = tmp2.x;
                o.texcoord5.x = tmp2.y;
                o.texcoord4.z = tmp1.z;
                o.texcoord5.z = tmp1.x;
                o.texcoord4.y = tmp3.y;
                o.texcoord5.y = tmp3.z;
                o.texcoord6.xyz = unity_ObjectToWorld._m03_m13_m23 * v.vertex.www + tmp0.xyz;
                tmp0 = unity_ObjectToWorld._m03_m13_m23_m33 * v.vertex.wwww + tmp0;
                tmp1 = unity_WorldToObject._m01_m11_m21_m31 * unity_MatrixInvV._m10_m10_m10_m10;
                tmp1 = unity_WorldToObject._m00_m10_m20_m30 * unity_MatrixInvV._m00_m00_m00_m00 + tmp1;
                tmp1 = unity_WorldToObject._m02_m12_m22_m32 * unity_MatrixInvV._m20_m20_m20_m20 + tmp1;
                tmp1 = unity_WorldToObject._m03_m13_m23_m33 * unity_MatrixInvV._m30_m30_m30_m30 + tmp1;
                tmp2.xyz = v.normal.xyz;
                tmp2.w = 1.0;
                o.texcoord7.x = dot(tmp1, tmp2);
                tmp1 = unity_WorldToObject._m01_m11_m21_m31 * unity_MatrixInvV._m11_m11_m11_m11;
                tmp1 = unity_WorldToObject._m00_m10_m20_m30 * unity_MatrixInvV._m01_m01_m01_m01 + tmp1;
                tmp1 = unity_WorldToObject._m02_m12_m22_m32 * unity_MatrixInvV._m21_m21_m21_m21 + tmp1;
                tmp1 = unity_WorldToObject._m03_m13_m23_m33 * unity_MatrixInvV._m31_m31_m31_m31 + tmp1;
                o.texcoord7.y = dot(tmp1, tmp2);
                o.texcoord8.xyz = v.color.xyz;
                o.texcoord9 = float4(0.0, 0.0, 0.0, 0.0);
                tmp1.xyz = tmp0.yyy * unity_WorldToLight._m01_m11_m21;
                tmp1.xyz = unity_WorldToLight._m00_m10_m20 * tmp0.xxx + tmp1.xyz;
                tmp0.xyz = unity_WorldToLight._m02_m12_m22 * tmp0.zzz + tmp1.xyz;
                o.texcoord10.xyz = unity_WorldToLight._m03_m13_m23 * tmp0.www + tmp0.xyz;
                o.texcoord11 = float4(0.0, 0.0, 0.0, 0.0);
                return o;
			}
			// Keywords: POINT
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                float4 tmp3;
                float4 tmp4;
                float4 tmp5;
                float4 tmp6;
                float4 tmp7;
                float4 tmp8;
                tmp0.xyz = inp.texcoord6.yyy * unity_WorldToLight._m01_m11_m21;
                tmp0.xyz = unity_WorldToLight._m00_m10_m20 * inp.texcoord6.xxx + tmp0.xyz;
                tmp0.xyz = unity_WorldToLight._m02_m12_m22 * inp.texcoord6.zzz + tmp0.xyz;
                tmp0.xyz = tmp0.xyz + unity_WorldToLight._m03_m13_m23;
                tmp0.x = dot(tmp0.xyz, tmp0.xyz);
                tmp0 = tex2D(_LightTexture0, tmp0.xx);
                tmp0.y = tmp0.x * tmp0.x;
                tmp1 = tex2D(_NormalTexture, inp.texcoord2.xy);
                tmp0.zw = tmp1.wx * float2(-1.0, 1.0) + float2(0.5, -0.5);
                tmp0.zw = tmp0.zw * _NormalStrength.xx + float2(0.5, 0.5);
                tmp1.xy = tmp0.zw * float2(2.0, 2.0) + float2(-1.0, -1.0);
                tmp0.z = dot(tmp1.xy, tmp1.xy);
                tmp0.z = min(tmp0.z, 1.0);
                tmp0.z = 1.0 - tmp0.z;
                tmp1.z = sqrt(tmp0.z);
                tmp2.x = dot(inp.texcoord3.xyz, tmp1.xyz);
                tmp2.z = dot(inp.texcoord5.xyz, tmp1.xyz);
                tmp2.y = dot(inp.texcoord4.xyz, tmp1.xyz);
                tmp0.z = dot(tmp2.xyz, tmp2.xyz);
                tmp0.z = rsqrt(tmp0.z);
                tmp3.xyz = tmp0.zzz * tmp2.xyz;
                tmp3.w = tmp2.y * tmp0.z + 0.5;
                tmp2.xyz = tmp3.xwz * float3(0.85, 0.85, 0.85);
                tmp4.xyz = _WorldSpaceCameraPos - inp.texcoord6.xyz;
                tmp0.z = dot(tmp4.xyz, tmp4.xyz);
                tmp0.z = rsqrt(tmp0.z);
                tmp4.xyz = tmp0.zzz * tmp4.xyz;
                tmp5.xyz = _WorldSpaceLightPos0.xyz - inp.texcoord6.xyz;
                tmp0.z = dot(tmp5.xyz, tmp5.xyz);
                tmp0.z = rsqrt(tmp0.z);
                tmp6.xyz = tmp5.xyz * tmp0.zzz + tmp4.xyz;
                tmp5.xyz = tmp0.zzz * tmp5.xyz;
                tmp0.z = saturate(dot(tmp2.xyz, tmp5.xyz));
                tmp0.w = dot(tmp6.xyz, tmp6.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp2.xyz = tmp0.www * tmp6.xyz;
                tmp0.w = dot(tmp3.xyz, tmp2.xyz);
                tmp0.w = max(tmp0.w, 0.0);
                tmp0.w = log(tmp0.w);
                tmp0.w = tmp0.w * _SpecularPower;
                tmp0.w = exp(tmp0.w);
                tmp2.xyz = tmp0.www * _LightColor0.xyz;
                tmp3 = tex2D(_SpecularTexture, inp.texcoord2.zw);
                tmp3.xyz = tmp3.xyz * _SpecularBrightness.xxx;
                tmp5 = tex2D(_DiffuseTexture, inp.texcoord1.zw);
                tmp6.xyz = tmp5.xyz * float3(0.5, 0.5, 0.5) + float3(0.5, 0.5, 0.5);
                tmp7.xyz = tmp3.xyz * tmp6.xyz;
                tmp3.xyz = tmp3.xyz * tmp6.xyz + float3(0.5, 0.5, 0.5);
                tmp2.xyz = tmp2.xyz * tmp7.xyz;
                tmp2.xyz = tmp0.yyy * tmp2.xyz;
                tmp6 = tex2D(_Fal, inp.texcoord.zw);
                tmp7 = tex2D(_Fal2, inp.texcoord1.xy);
                tmp6.xyz = tmp6.xyz * tmp7.xyz;
                tmp7.xyz = tmp5.xyz * tmp6.xyz;
                tmp8.xyz = -tmp7.xyz * _DiffuseColor.xyz + float3(1.5, 1.5, 1.5);
                tmp7.xyz = tmp7.xyz * _DiffuseColor.xyz;
                tmp2.xyz = tmp2.xyz * tmp8.xyz;
                tmp2.xyz = tmp0.zzz * tmp2.xyz;
                tmp2.xyz = tmp0.zzz * tmp2.xyz;
                tmp8.xyz = tmp0.zzz * _LightColor0.xyz;
                tmp7.xyz = tmp7.xyz * tmp8.xyz;
                tmp0.yzw = tmp7.xyz * tmp0.zzz + tmp2.xyz;
                tmp2.xy = inp.texcoord7.xy * float2(1.0, -1.0) + float2(0.0, 1.0);
                tmp2.xy = tmp2.xy * float2(0.5, 0.5) + float2(0.5, 0.5);
                tmp2 = tex2D(_Sphere, tmp2.xy);
                tmp2.xyz = tmp2.xyz * _ReflectColor.xyz;
                tmp5.xyz = tmp5.xyz * float3(0.75, 0.75, 0.75) + float3(0.25, 0.25, 0.25);
                tmp2.xyz = tmp2.xyz * tmp5.xyz;
                tmp2.xyz = tmp5.www * tmp2.xyz;
                tmp2.xyz = tmp6.xyz * tmp2.xyz;
                tmp2.xyz = tmp2.xyz * float3(0.75, 0.75, 0.75);
                tmp0.yzw = tmp2.xyz * _ReflectStrength.xxx + tmp0.yzw;
                tmp2.xyz = tmp4.yyy * inp.texcoord4.xyz;
                tmp2.xyz = inp.texcoord3.xyz * tmp4.xxx + tmp2.xyz;
                tmp2.xyz = inp.texcoord5.xyz * tmp4.zzz + tmp2.xyz;
                tmp1.x = dot(tmp2.xyz, tmp1.xyz);
                tmp1.x = 1.0 - tmp1.x;
                tmp1.x = log(tmp1.x);
                tmp1.x = tmp1.x * _FalloffExponent2;
                tmp1.x = exp(tmp1.x);
                tmp1.x = tmp1.x * _FalloffExponent;
                tmp1.xyz = tmp3.xyz * tmp1.xxx;
                tmp1.xyz = tmp1.xyz * float3(0.5, 0.5, 0.5);
                tmp1.xyz = tmp1.xyz * _Falloff.xyz;
                tmp1.xyz = tmp6.xyz * tmp1.xyz;
                tmp1.xyz = tmp6.xyz * tmp1.xyz;
                tmp0.xyz = tmp0.yzw * tmp0.xxx + tmp1.xyz;
                tmp1 = tex2D(_Emm, inp.texcoord.xy);
                tmp2.xyz = tmp1.xyz * float3(0.75, 0.75, 0.75) + float3(0.25, 0.25, 0.25);
                tmp1.xyz = tmp1.xyz * tmp2.xyz;
                tmp1.xyz = tmp6.xyz * tmp1.xyz;
                tmp0.xyz = tmp1.xyz * _emmBrightness.xxx + tmp0.xyz;
                tmp0.xyz = max(tmp0.xyz, -_MaxBrightness.xxx);
                o.sv_target.xyz = min(tmp0.xyz, _MaxBrightness.xxx);
                o.sv_target.w = 1.0;
                return o;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}