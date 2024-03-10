#include "pch.h"
#define TEST_EXPORTS
#include "TestOfCPP.h"
#include <chrono>
#include <numeric>
#include <vector>
#include <thread>
#include <fstream>

#pragma optimize("", off)
Test_API int RamWriteSpeed()
{
    std::vector<int> speeds(100);
    std::vector<char> buffer(256 * 1024 * 1024);
    size_t stride = 256;
    for (int index = 0; index < 100; index++)
    {

        auto start = std::chrono::high_resolution_clock::now();
        for (size_t i = 0; i < buffer.size(); i += stride)
        {
            buffer[i] = 0;
        }
        auto stop = std::chrono::high_resolution_clock::now();
        std::chrono::duration<double> elapsed = stop - start;
        speeds[index] = static_cast<int>((256 * 1024 / stride) / elapsed.count());
        std::this_thread::sleep_for(std::chrono::milliseconds(50));
    }
    double average = std::accumulate(speeds.begin(), speeds.end(), 0.0) / speeds.size();
    return static_cast<int>(average);
}


Test_API int RamReadSpeed()
{
    const int size = 256 * 1024 * 1024;
    const size_t stride = 256;
    std::vector<int> speeds(100);
    std::vector<char> buffer(size);
    for (size_t i = 0; i < buffer.size(); i += stride)
    {
        buffer[i] = 0;
    }

    for (int index = 0; index < 100; index++)
    {
        volatile char temp = 0;
        auto start = std::chrono::high_resolution_clock::now();
        for (size_t i = 0; i < buffer.size(); i += stride)
        {
            temp += buffer[i];
        }
        auto stop = std::chrono::high_resolution_clock::now();
        std::chrono::duration<double> elapsed = stop - start;
        speeds[index] = static_cast<int>((256 * 1024 / stride) / elapsed.count());

        std::this_thread::sleep_for(std::chrono::milliseconds(50));
    }

    double average = std::accumulate(speeds.begin(), speeds.end(), 0.0) / speeds.size();
    return static_cast<int>(average);
}


#pragma optimize("", on)
Test_API int DiskWriteSpeed()
{
    std::vector<char> buffer(1000 * 1024 * 1024, 0);
    char* temp_path = nullptr;
    size_t len = 0;
    _dupenv_s(&temp_path, &len, "TEMP");
    std::string file_path = std::string(temp_path) + "/test.bin";
    free(temp_path);
    auto start = std::chrono::high_resolution_clock::now();
    std::ofstream ofs(file_path, std::ios::binary);
    ofs.write(buffer.data(), buffer.size());
    ofs.close();
    auto stop = std::chrono::high_resolution_clock::now();
    std::chrono::duration<double> elapsed = stop - start;
    return static_cast<int>(1024000.0 / elapsed.count());
}

Test_API int DiskReadSpeed()
{
    std::vector<char> buffer(500 * 1024 * 1024);
    char* temp_path = nullptr;
    size_t len = 0;
    _dupenv_s(&temp_path, &len, "TEMP");
    std::string file_path = std::string(temp_path) + "/test.bin";
    free(temp_path);
    auto start = std::chrono::high_resolution_clock::now();
    std::ifstream ifs(file_path, std::ios::binary);
    ifs.read(buffer.data(), buffer.size());
    ifs.close();
    auto stop = std::chrono::high_resolution_clock::now();
    std::chrono::duration<double> elapsed = stop - start;
    int speed = static_cast<int>(1024000.0 / elapsed.count());
    std::remove(file_path.c_str());
    return speed;
}