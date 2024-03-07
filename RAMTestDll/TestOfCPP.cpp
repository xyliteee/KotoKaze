#include "pch.h"
#define TEST_EXPORTS
#include "TestOfCPP.h"
#include <chrono>
#include <numeric>
#include <vector>
#include <fstream>

Test_API int RamWriteSpeed()
{
    std::vector<int> speeds(5);
    std::vector<char> buffer(64 * 1024 * 1024);
    for (int index = 0; index < 5; index++)
    {
        auto start = std::chrono::high_resolution_clock::now();
        for (size_t i = 0; i < buffer.size(); i++)
        {
            buffer[i] = 0;
        }
        auto stop = std::chrono::high_resolution_clock::now();
        std::chrono::duration<double> elapsed = stop - start;
        speeds[index] = static_cast<int>(64.0 / elapsed.count());
    }
    double average = std::accumulate(speeds.begin(), speeds.end(), 0.0) / speeds.size();
    return static_cast<int>(average);
}

Test_API int RamReadSpeed()
{
    std::vector<int> speeds(5);
    std::vector<char> buffer(64 * 1024 * 1024);
    std::vector<char> read_buffer(buffer.size());
    for (auto& byte : buffer)
    {
        byte = 0;
    }
    for (int index = 0; index < 5; index++)
    {
        auto start = std::chrono::high_resolution_clock::now();
        std::memcpy(read_buffer.data(), buffer.data(), buffer.size());
        auto stop = std::chrono::high_resolution_clock::now();
        std::chrono::duration<double> elapsed = stop - start;
        speeds[index] = static_cast<int>(64 / elapsed.count());
    }
    double average = std::accumulate(speeds.begin(), speeds.end(), 0.0) / speeds.size();
    return static_cast<int>(average);
}

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