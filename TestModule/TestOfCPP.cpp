#include "pch.h"
#define TEST_EXPORTS
#include "TestOfCPP.h"
#include <chrono>
#include <numeric>
#include <vector>
#include <thread>
#include <fstream>
#include <filesystem>

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

Test_API int DiskWriteRandomSpeed()
{
    const int fileSize = 4 * 1024;
    const int totalSize = 64 * 1024 * 1024;
    const int fileCount = totalSize / fileSize;

    std::vector<char> buffer(fileSize);

    char* temp_path = nullptr;
    size_t len = 0;
    _dupenv_s(&temp_path, &len, "TEMP");
    std::string dir_path = std::string(temp_path) + "/KotoKaze/TestFiles";
    free(temp_path);

    // ´´½¨Ä¿Â¼
    std::filesystem::create_directories(dir_path);

    auto start = std::chrono::high_resolution_clock::now();

    for (int i = 0; i < fileCount; ++i)
    {
        std::string file_path = dir_path + "/test" + std::to_string(i) + ".bin";
        std::ofstream ofs(file_path, std::ios::binary);
        ofs.write(buffer.data(), buffer.size());
        ofs.close();
    }

    auto stop = std::chrono::high_resolution_clock::now();
    std::chrono::duration<double> elapsed = stop - start;

    return static_cast<int>(totalSize / 1024.0 / elapsed.count());
}
Test_API int DiskReadRandomSpeed()
{
    const int fileSize = 4 * 1024;
    const int totalSize = 64 * 1024 * 1024;
    const int fileCount = totalSize / fileSize;

    std::vector<char> buffer(fileSize);

    char* temp_path = nullptr;
    size_t len = 0;
    _dupenv_s(&temp_path, &len, "TEMP");
    std::string dir_path = std::string(temp_path) + "/KotoKaze/TestFiles";
    free(temp_path);

    auto start = std::chrono::high_resolution_clock::now();

    for (int i = 0; i < fileCount; ++i)
    {
        std::string file_path = dir_path + "/test" + std::to_string(i) + ".bin";
        std::ifstream ifs(file_path, std::ios::binary);
        ifs.read(buffer.data(), buffer.size());
        if (ifs.fail())
        {
            return 0;
        }
        ifs.close();
    }

    auto stop = std::chrono::high_resolution_clock::now();
    std::chrono::duration<double> elapsed = stop - start;
    return static_cast<int>(totalSize / 1024.0 / elapsed.count());
}