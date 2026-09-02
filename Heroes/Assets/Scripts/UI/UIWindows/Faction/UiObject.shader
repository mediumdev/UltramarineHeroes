Shader "Ultramarine/UiObject"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}

		[Header(Ambient)]
		_AmbientColor ("Color", Color) = (1., 1., 1., 1.)

		[Header(Direct Lighting)]
		_DiffuseColor ("Color", Color) = (1., 1., 1., 1.)
		_ShadowColor ("Shadow Color", Color) = (0., 0., 0., 0.)
		[Header(Hardness And Offset (XY direct ZW shadow))]
		_HardnessAndShift ("Parameters", Vector) = (1.,0.,1.,0.)

		[Header(Mask)]
		[Toggle] _Mask("Enabled", Float) = 0.
		[NoScaleOffset] _MaskTex ("Texture  (A - rim G - spec/refl B - self illum)", 2D) = "white" {}

		[Header(Rim (R mask channel))]
		[Toggle] _Rim("Enabled", Float) = 0.
		_RimStart ("Start", Range(0., 1.)) = 1.
		_RimEnd ("End", Range(0., 1.)) = 1.
		_RimColor ("Color", Color) = (1., 1., 1., 1.)

		[Header(Reflection (G mask channel))]
		[Toggle] _Reflection("Enabled", Float) = 0.
		_ReflectionColor ("Color", Color) = (1., 1., 1., 1.)
		_ReflectionTex ("Reflection Texture", CUBE) = "black" {}

		[Header(Specular (G mask channel))]
		[Toggle] _Specular("Enabled", Float) = 0.
		_Shininess ("Shininess", Range(0.01, 1.)) = 1.
		_SpecularColor ("Color", Color) = (1., 1., 1., 1.)

		[Header(Self Illumination (B mask channel))]
		[Toggle] _Illumination ("Enabled", Float) = 0.
		_IlluminationColor ("Color", Color) = (1., 1., 1., 1.)

		[Header(Fog)]
		[Toggle] _Fog("Fog Enabled", Float) = 0.

		[Header(Hit)]
		[Toggle] _Hit("Enabled", Float) = 0.
		[PerRendererData] _HitColorNew ("Hit Color", Color) = (0,0,0,0)

		[Header(Gray Scale)]
		[Toggle] _GrayScale ("Gray Scale Enabled", Float) = 0.
		_GrayScaleColor ("Color", Color) = (1., 1., 1., 0.5)
		
		[Header(Stencil)]
		_StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        
        [Header(Ui)]
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"RenderType"="Opaque"
		}
        
        Stencil
        {
            Ref 1
            Comp LEqual
            Pass Keep
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
		
		Pass
		{
			Tags { "LightMode"="Vertex" }
			CGPROGRAM
			#pragma shader_feature _MASK_ON
			#pragma shader_feature _REFLECTION_ON
			#pragma shader_feature _SPECULAR_ON
			#pragma shader_feature _ILLUMINATION_ON
			#pragma shader_feature _RIM_ON
			#pragma shader_feature _FOG_ON
			#pragma shader_feature _HIT_ON
			#pragma multi_compile __ _GRAYSCALE_ON
			#pragma multi_compile_instancing

			#include "Default_fragment.cginc"
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}

	Fallback "Mobile/VertexLit"
}