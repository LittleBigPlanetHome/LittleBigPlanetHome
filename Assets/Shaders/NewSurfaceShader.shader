Shader "Custom/MyHLSShader" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_inkParam ("Ink Parameter", Range(0,1)) = 1
		_inkEffect ("Ink Effect", Range(0,10)) = 0
		_rgbCoeff ("RGB Coeff", Vector) = (1,1,1,1)
		_fA ("F A", Float) = 0
		_fB ("F B", Float) = 0
		_scale ("Scale", Vector) = (1,1)
		_offset ("Offset", Vector) = (0,0)
		_pDir ("P Dir", Range(0,1)) = 0
	}
	
	SubShader {
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
				float2 texCoord : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
			
			uniform sampler2D _MainTex;
			uniform float _inkParam;
			uniform int _inkEffect;
			uniform float3 _rgbCoeff;
			uniform float _fA;
			uniform float _fB;
			uniform float2 _scale;
			uniform float2 _offset;
			uniform int _pDir;
			
			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texCoord = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				float4 color = float4(0, 0, 0, 0);
				float2 posTex;
				
				if(_pDir != 0) {
					float ScreenX = (_fA-(_fA-_fB)*(1.0-i.texCoord.y));
					posTex = i.texCoord * float2(ScreenX, 1.0) + float2((1.0-ScreenX)/2.0, 0.0);
					color = tex2D(_MainTex, posTex*_scale+_offset);
				}
				else {
					float ScreenY = (_fA-(_fA-_fB)*i.texCoord.x);
					posTex = i.texCoord * float2(1.0, ScreenY) + float2(0.0, (1.0-ScreenY)/2.0);
					color = tex2D(_MainTex, posTex*_scale+_offset);
				}
				color = color * float4(_rgbCoeff, _inkParam);
				
				if(_inkEffect == 2)			//INVERT
					color.rgb = 1.0-color.rgb;
				else if(_inkEffect == 10)	//MONO
				{
					float mono = 0.3125*color.r + 0.5625*color.g + 0.125*color.b;
					color.rgb = float3(mono,mono,mono);
				}
				
				return color;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
