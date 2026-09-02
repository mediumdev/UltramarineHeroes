#include "UnityCG.cginc"
#include "UnityInstancing.cginc"
#include "Standard_Functions.cginc"

struct appdata {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	#if _BUMP_ON
	float4 tangent : TANGENT;
	#endif
	float2 uv : TEXCOORD0;
	#if _SEPARATELIGHT_ON
	float2 uv1 : TEXCOORD1;
	#endif
	#if _VERTEXCOLOR_ON || _VERTEXCOLOR_MAP
	fixed4 color : COLOR;
	#endif
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f {
	float4 pos : SV_POSITION;
	float4 uv : TEXCOORD0;// _MainTex _LightTex
	float4 light : TEXCOORD1;
	#if _BUMP_ON
	half4 tS0 : TEXCOORD2;
	half4 tS1 : TEXCOORD3;
	half4 tS2 : TEXCOORD4;
	#else
	float3 wpos : TEXCOORD2;
	half3 normal : TEXCOORD3;
	#endif
	#if _VERTEXCOLOR_ON || _VERTEXCOLOR_MAP
	fixed4 color : COLOR0;
	#endif
	#if _HIT_ON
	half4 hit : COLOR1;
	#endif
};

sampler2D _MainTex;
float4 _MainTex_ST;
#if _SEPARATELIGHT_ON
sampler2D _LightTex;
float4 _LightTex_ST;
#endif
#if _BUMP_ON
sampler2D _BumpMap;
#endif
sampler2D _MaskTex;
#if _REFLECTION_ON
samplerCUBE _ReflectionTex;
#endif

fixed4 _AmbientColor;
fixed4 _DiffuseColor;
fixed4 _ShadowColor;
half4 _HardnessAndShift;

fixed _Shininess;
fixed4 _SpecularColor;
fixed4 _ReflectionColor;

fixed _RimStart;
fixed _RimEnd;
fixed4 _RimColor;
fixed4 GlobalRimColor;

fixed4 _IlluminationColor;

fixed4 _GrayScaleColor;

sampler2D _FogLUT;
float2 _FogLUTParams; // x: -1/(end-start) y: end/(end-start)
fixed _IllumInfluenceValue;

fixed4 _rColor;
fixed4 _gColor;
fixed4 _bColor;
fixed4 _aColor;

UNITY_INSTANCING_BUFFER_START(Props)
	UNITY_DEFINE_INSTANCED_PROP(fixed4, _HitColorNew)
UNITY_INSTANCING_BUFFER_END(Props)

v2f vert(appdata i)
{
	UNITY_SETUP_INSTANCE_ID(i);
	v2f o;
	float4 wpos = mul(unity_ObjectToWorld, i.vertex);

	o.pos = UnityObjectToClipPos(i.vertex);//mul(UNITY_MATRIX_VP, wpos);

	o.uv.xy = TRANSFORM_TEX(i.uv, _MainTex);
	o.light = mul(UNITY_MATRIX_I_V, unity_LightPosition[0]);

	#if _SEPARATELIGHT_ON
	o.uv.zw = TRANSFORM_TEX(i.uv1, _LightTex);
	#else
	o.uv.zw = float2(0.0f, 0.0f);
	#endif

	#if _BUMP_ON
	half3 wnorm = UnityObjectToWorldNormal(i.normal);
	half3 wtang = UnityObjectToWorldDir(i.tangent);
	half tangS = i.tangent.w * unity_WorldTransformParams.w;
	half3 wbi = cross(wnorm, wtang) * tangS;
	o.tS0 = float4(wtang.x, wbi.x, wnorm.x, wpos.x);
	o.tS1 = float4(wtang.y, wbi.y, wnorm.y, wpos.y);
	o.tS2 = float4(wtang.z, wbi.z, wnorm.z, wpos.z);
	#else
	o.wpos = wpos.xyz;
	o.normal = UnityObjectToWorldNormal(i.normal);
	#endif

	#if _VERTEXCOLOR_ON || _VERTEXCOLOR_MAP
	o.color = i.color;
	#endif

	#if _HIT_ON
	o.hit = UNITY_ACCESS_INSTANCED_PROP(Props, _HitColorNew);
	#endif
	return o;
}

fixed4 frag(v2f i) : SV_Target
{
	fixed4 color = tex2D(_MainTex, i.uv.xy);
	color.a = 1.0f;

	#if _SEPARATELIGHT_ON
	color *= 2.0f * tex2D(_LightTex, i.uv.zw);
	#endif

	#if _VERTEXCOLOR_ON
	color *= i.color;
	#endif

	#if _VERTEXCOLOR_MAP
	fixed3 rColor = i.color.r * _rColor.rgb;
	fixed3 gColor = i.color.g * _gColor.rgb;
	fixed3 bColor = i.color.b * _bColor.rgb;
	fixed3 aColor = i.color.a * _aColor.rgb;

	color.rgb *= (rColor + gColor + bColor + aColor) * avoidDBZ(i.color.r + i.color.g + i.color.b + i.color.a);
	#endif

	#if _MASK_ON
	fixed4 mask = tex2D(_MaskTex, i.uv.xy);
	#endif

	#if _BUMP_ON
	float3 wpos = float3(i.tS0.w, i.tS1.w, i.tS2.w);
	half3 norm = normalize(UnpackNormal(tex2D(_BumpMap, i.uv.xy)));
	fixed3 n;
	n.x = dot(i.tS0.xyz, norm);
	n.y = dot(i.tS1.xyz, norm);
	n.z = dot(i.tS2.xyz, norm);
	#else
	float3 wpos = i.wpos;
	half3 n = normalize(i.normal);
	#endif
	half3 l = normalize(i.light.xyz - wpos * i.light.w);

	#if _SPECULAR_ON || _RIM_ON || _REFLECTION_ON
	half3 v = normalize(_WorldSpaceCameraPos.xyz - wpos);
	#endif
	#if _SPECULAR_ON || _REFLECTION_ON
	half3 r = reflect(-v, n);
	#endif

	#if _REFLECTION_ON
	fixed4 reflection = texCUBE(_ReflectionTex, r);
	fixed reflectionIntensity = _ReflectionColor.a;
	#if _MASK_ON
	reflectionIntensity *= mask.b;  //changed reflection channel from G to B
	#endif
	color.rgb = lerp(color.rgb, reflection.rgb * _ReflectionColor.rgb, reflectionIntensity);
	#endif

	fixed3 light = _AmbientColor.rgb * _AmbientColor.a;
	#if _AMBIENTREACT_ON
	light *= UNITY_LIGHTMODEL_AMBIENT;
	#endif

	fixed3 lightColor = unity_LightColor[0].rgb * _DiffuseColor.a * _DiffuseColor.rgb;
	fixed3 shadowColor = _ShadowColor.rgb * _ShadowColor.a;
	half ndl = dot(n, l);
	light += saturate(ndl * _HardnessAndShift.x + _HardnessAndShift.y) * lightColor;
	light += saturate(1.0f - max(ndl * _HardnessAndShift.z + _HardnessAndShift.w, 0.0f)) * shadowColor;

	#if _SPECULAR_ON
	fixed3 specular = sfSpecularCoeff(l, r, _Shininess * 64.0f) * _SpecularColor.a * _SpecularColor.rgb * lightColor;
	#if _MASK_ON
	specular *= mask.g;
	#endif
	light += specular;
	#endif

	#if _ILLUMINATION_ON
	fixed illuminationIntensity = _IlluminationColor.a;
	#if _MASK_ON
	illuminationIntensity *= mask.b;
	#endif
	light = lerp(light, _IlluminationColor.rgb, illuminationIntensity);
	#endif

	color.rgb *= light;

	#if _RIM_ON
	fixed4 rimColor;
	#if _GLOBALRIMCOLOR_ON
	rimColor = GlobalRimColor;
	#else
	rimColor = _RimColor;
	#endif
	fixed3 rim = sfSmoothstepRight(_RimEnd, _RimEnd - _RimStart, dot(n, v)) * rimColor.a * rimColor.rgb;
	#if _MASK_ON
	rim *= mask.r;
	#endif
	color.rgb += rim;
	#endif

	#if _FOG_ON
	fixed4 fog = tex2D(_FogLUT, float2(wpos.z * _FogLUTParams.x + _FogLUTParams.y, 0.5f));

	#if _ILLUMINFLUENCE_ON
	fog.rgb = lerp(fog.rgb, color.rgb, mask.b * _IllumInfluenceValue);
	#endif

	color.rgb = lerp(color.rgb, fog.rgb, fog.a);
	#endif

	#if _HIT_ON
	fixed3 blend = BlendHardLight(color.rgb, i.hit.rgb);
	color.rgb = lerp(color.rgb, blend, i.hit.a);
	#endif

	#if _GRAYSCALE_ON
	color.rgb = dot(color.rgb, fixed3(0.299f, 0.587f, 0.114f)) * _GrayScaleColor.rgb * 2.0f * _GrayScaleColor.a;
	#endif

	return color;
}
