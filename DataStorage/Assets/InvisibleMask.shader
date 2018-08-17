Shader "Custom/Occluder"
{
	SubShader
	{
		Tags
		{
			"Queue" = "Background+1"
		}


		BlendOp Min
		Blend Zero One


		CGPROGRAM
		#pragma surface surf Lambert


		struct Input
		{
			float Nothing;
		};


		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = half4(1, 1, 1, 1);
		}
		ENDCG
	}


	Fallback "VertexLit"
}