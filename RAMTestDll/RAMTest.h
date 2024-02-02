#pragma once

#ifdef TEST_EXPORTS
#define Test_API __declspec(dllexport)
#else
#define Test_API __declspec(dllimport)
#endif


extern "C" Test_API int WriteSpeed();
extern "C" Test_API int ReadSpeed();