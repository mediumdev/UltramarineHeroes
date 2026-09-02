#include "UnityCG.cginc"

#define BlendHardLight(base, blend) (blend < 0.5 ? (2.0 * blend * base) : (1.0 - 2.0 * (1.0 - blend) * (1.0 - base)))

//optimized
float avoidDBZ(float value)
{
	//float signValue = abs(sign(value));
	//return (1.0f * signValue) / (value + (signValue - 1.0f));
	return (value == 0.0f) ? (0.0f) : (1.0f / value);
}

//optimized
inline float sfPow(float t, float s)
{
	return t * (1.0f + (t - 1.0f) * s);
}

inline half sfPowH(half t, half s)
{
	return t * (1.0f + (t - 1.0f) * s);
}

inline fixed sfPowF(half t, half s)
{
	return saturate(t * (1.0f + (t - 1.0f) * s));
}

//on PowerVR equal to exp(x)
inline float sfExp(float t)
{
	return t * (0.5430806348f * t + 1.1752011937f) + 1.0f;
}

//on PowerVR equal to log(x)
inline float sfLog(float t)
{
	return t * (-0.35f * t + 1.9027972799f) - 1.5861696346f;
}

//optimized
inline float sfSmoothstepLeft(float minEdge, float maxEdge, float t)
{
	float x = saturate((t - minEdge) / (maxEdge - minEdge));
	return x * x;
}

inline half sfSmoothstepLeftH(half minEdge, half maxEdge, half t)
{
	half x = saturate((t - minEdge) / (maxEdge - minEdge));
	return x * x;
}

inline fixed sfSmoothstepLeftF(half minEdge, half maxEdge, half t)
{
	fixed x = saturate((t - minEdge) / (maxEdge - minEdge));
	return x * x;
}

//optimized
inline float sfSmoothstepRight(float minEdge, float maxEdge, float t)
{
	float x = saturate((t - minEdge) / (maxEdge - minEdge));
	return x * (2.0f - x);
}

inline half sfSmoothstepRightH(half minEdge, half maxEdge, half t)
{
	half x = saturate((t - minEdge) / (maxEdge - minEdge));
	return x * (2.0f - x);
}

inline fixed sfSmoothstepRightF(half minEdge, half maxEdge, half t)
{
	fixed x = saturate((t - minEdge) / (maxEdge - minEdge));
	return x * (2.0f - x);
}

//линейная периодическая функция. Диапазон значения: (0, 1)
inline float sfLWave(float t)
{
	return abs(frac(t) - 0.5f) * 2.0f; //linear periodic function
}

//нелинейная периодическая функция. Диапазон значения: (0, 1)
inline float sfWave(float t, float s)
{
	return sfPow(abs((frac(t) - 0.5f)) * 2.0f, s); //non-linear periodic function
}

//optimized for PowerVR compared to the sin(x)
//"линейный синус"
inline float sfLSin(float t)
{
	return (abs(frac(t / 6.28f + 0.57f) - 0.5f) - 0.25f) * 4.0f; //approximated sine - linear variant
}

//optimized for PowerVR compared to the cos(x)
//"линейный косинус"
inline float sfLCos(float t)
{
	return (abs(frac(t / 6.28f) - 0.5f) - 0.25f) * 4.0f; //approximated cosine - linear variant
}

//optimized for PowerVR compared to the sin(x) & sfLSin(x)
//Допустимые значения: от -pi до +pi
inline float sfSin2(float t)
{
	return t * (0.8276196351 - 0.0843118645 * t * t);
}

//optimized for PowerVR compared to the cos(x)
//Допустимые значения: от -1.5pi до +0.5pi
inline float sfCos2(float t)
{
	t += 1.57f;
	return t * (0.8276196351 - 0.0843118645 * t * t);
}

//Not optimal
/*inline float sfCosF(float t, float s)
{
	float result = (abs(frac(t / 6.28f) - 0.5f) - 0.25f) * 4.0f;
	return result * (1.0f + (abs(result) - 1.0f) * s);//approximated cosine - non-linear variant
}*/

//Not optimal
/*inline float sfSinF(float t, float s)
{
	float result = (abs(frac(t / 6.28f + 0.74575f) - 0.5f) - 0.25f) * 4.0f;
	return result * (1.0f + (abs(result) - 1.0f) * s);//approximated sine - non-linear variant
}*/

//Not optimal
/*inline float sfCos(float t)
{
	float s = -0.85f;
	float result = (abs(frac(t / 6.28f) - 0.5f) - 0.25f) * 4.0f;
	return result * (1.0f + (abs(result) - 1.0f) * s);//approximated cosine - non-linear variant, s = -0.85
}*/

//Not optimal
/*inline float sfSin(float t)
{
	float s = -0.85f;
	float result = (abs(frac(t / 6.28f + 0.74575f) - 0.5f) - 0.25f) * 4.0f;
	return result * (1.0f + (abs(result) - 1.0f) * s);//approximated sine - non-linear variant
}*/

//approximated arcsine
inline float sfASin(float t)
{
	return t * (0.8234164f * t * t + 0.7473799f);
}

//approximated arccosine
inline float sfACos(float t)
{
	return 1.57f - t * (0.8234164f * t * t + 0.7473799f);
}

inline float sfSpecularCoeff(float3 lightDir, float3 VNReflectionDir, float strength)
{
	return saturate(sfPow(saturate(dot(lightDir, VNReflectionDir)), strength));
}

inline half sfSpecularCoeffH(half3 lightDir, half3 VNReflectionDir, half strength)
{
	return saturate(sfPowH(saturate(dot(lightDir, VNReflectionDir)), strength));
}

inline fixed sfSpecularCoeffF(half3 lightDir, half3 VNReflectionDir, half strength)
{
	return sfPowF(saturate(dot(lightDir, VNReflectionDir)), strength);
}

//Experimental
//разложение волны на RGB спектр
/*inline float3 spectrum(float wave, float residue, float shift)
{
	float rResidue = residue * sfWave(shift, 1.0f);
	float gResidue = residue * sfWave(shift + 0.33f, 1.0f);
	float bResidue = residue * sfWave(shift + 0.66f, 1.0f);

	return saturate(float3((wave - rResidue) / (1.0f - rResidue), (wave - gResidue) / (1.0f - gResidue), (wave - bResidue) / (1.0f - bResidue)));
}*/