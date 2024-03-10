#pragma once
#include <iostream>
#ifdef TEST_EXPORTS
#define Test_API __declspec(dllexport)
#else
#define Test_API __declspec(dllimport)
#endif

extern "C" Test_API std::string testModuleVersion;
extern "C" Test_API int RamWriteSpeed();
extern "C" Test_API int RamReadSpeed();
extern "C" Test_API int DiskReadSpeed();
extern "C" Test_API int DiskWriteSpeed();