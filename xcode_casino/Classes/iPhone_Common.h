
#ifndef _TRAMPOLINE_IPHONE_COMMON_H_
#define _TRAMPOLINE_IPHONE_COMMON_H_

#define ENABLE_UNITY_DEBUG_LOG 0


extern	bool	_ios30orNewer;
extern	bool	_ios31orNewer;
extern	bool	_ios43orNewer;
extern	bool	_ios50orNewer;


struct UnityFrameStats;

enum DeviceGeneration
{
	deviceUnknown = 0,
	deviceiPhone = 1,
	deviceiPhone3G = 2,
	deviceiPhone3GS = 3,
	deviceiPodTouch1Gen = 4,
	deviceiPodTouch2Gen = 5,
	deviceiPodTouch3Gen = 6,
	deviceiPad1Gen = 7,
	deviceiPhone4 = 8,
	deviceiPodTouch4Gen = 9,
	deviceiPad2Gen = 10,
	deviceiPhone4S = 11,
	deviceiPad3Gen = 12,
};


//------------------------------------------------------------------------------

#if ENABLE_UNITY_DEBUG_LOG
	#define UNITY_DBG_LOG(...)				\
		do 									\
		{									\
			printf_console(__VA_ARGS__);	\
		}									\
		while(0)
#else
	#define UNITY_DBG_LOG(...)				\
		do 									\
		{									\
		}									\
		while(0)
#endif // ENABLE_UNITY_DEBUG_LOG


#endif // _TRAMPOLINE_IPHONE_COMMON_H_
