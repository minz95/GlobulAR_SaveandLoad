// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/InvisibleMask" {
	SubShader
	{
		// draw after all opaque objects (queue = 2001):
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry+1" }
		Pass
		{
			Blend Zero One // keep the image behind it
		}
	}
}