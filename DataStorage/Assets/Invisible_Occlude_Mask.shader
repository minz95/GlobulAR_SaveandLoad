Shader "Custom/Invisible_Occlude_Mask" {
	Properties{
		_TintColor("TintColor", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_AlphaCut("Cutoff", Range(0,1)) = 0.5
		_AlphaTex("AlphaMap (R)", 2D) = "white" {}
		_Emission("Emission", Range(0, 2)) = 0
		_Fade("Fade", Range(0, 1)) = 0
	}

	SubShader{
		Tags{"Queue" = "Geometry+1" }			
		
		Blend SrcAlpha OneMinusSrcAlpha
		Zwrite On

		LOD 200

		CGPROGRAM
		#pragma surface surf Unlit alphatest:_AlphaCut noforwardadd halfasview approxview 

		sampler2D _MainTex;
		sampler2D _AlphaTex;
		fixed4 _TintColor;
		fixed _Fade;
		fixed _Emission;

		fixed4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
			return fixed4(s.Albedo,1);
		}
		struct Input {
			fixed2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _TintColor.rgb * _Emission;
			fixed4 al = tex2D(_AlphaTex, IN.uv_MainTex);
			o.Alpha = al.r * _Fade;
		}
	}
	FallBack "Diffuse"
}
