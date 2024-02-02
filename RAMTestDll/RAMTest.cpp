#include "pch.h"
#include "RAMTest.h"
#include <chrono>
#include <numeric>
#include <vector>

Test_API int WriteSpeed()
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

Test_API int ReadSpeed()
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