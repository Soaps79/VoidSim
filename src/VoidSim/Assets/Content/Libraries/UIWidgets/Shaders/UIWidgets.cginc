#ifndef UIWIDGETS_INCLUDED
#define UIWIDGETS_INCLUDED

inline half4 LinearToGammaSpace4(half4 linRGB)
{
	linRGB = max(linRGB, half4(0.h, 0.h, 0.h, 0.h));
	// An almost-perfect approximation from http://chilliant.blogspot.com.au/2012/08/srgb-approximations-for-hlsl.html?m=1
	return max(1.055h * pow(linRGB, 0.416666667h) - 0.055h, 0.h);

	// Exact version, useful for debugging.
	//return half4(LinearToGammaSpaceExact(linRGB.r), LinearToGammaSpaceExact(linRGB.g), LinearToGammaSpaceExact(linRGB.b), LinearToGammaSpaceExact(linRGB.a));
}

inline half4 GammaToLinearSpace4(half4 sRGB)
{
	// Approximate version from http://chilliant.blogspot.com.au/2012/08/srgb-approximations-for-hlsl.html?m=1
	return sRGB * (sRGB * (sRGB * 0.305306011h + 0.682171111h) + 0.012522878h);

	// Precise version, useful for debugging.
	//return half4(GammaToLinearSpaceExact(sRGB.r), GammaToLinearSpaceExact(sRGB.g), GammaToLinearSpaceExact(sRGB.b), GammaToLinearSpaceExact(sRGB.a));
}

inline float4 Hue(float H)
{
	float R = abs(H * 6 - 3) - 1;
	float G = 2 - abs(H * 6 - 2);
	float B = 2 - abs(H * 6 - 4);
	return saturate(float4(R,G,B,1));
}
			
inline float4 HSVtoRGB(in float4 HSV)
{
	return ((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z;
}
#endif