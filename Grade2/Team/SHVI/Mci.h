#pragma once
#include <Windows.h>
#include <string>

// 오픈
bool OpenMciDevice(LPCWSTR
    _deviceType, LPCWSTR _elementName,
    UINT& _deviceId);
// 닫기
void CloseMciDevice(UINT& _deviceId);
// 재생
void PlayMciDevice(UINT _deviceId,
    bool _repeat = false);

enum class SOUNDID
{
    BGM, ATTACK, GAMEOVER, HIT, TITLE, UPGRADE, GAMEOVERBGM, END
};
struct SoundEntry
{
    //LPCWSTR path;     // 파일 경로
    std::wstring path; // 파일 경로
    int     volume;  // 볼륨 (0~100)  
    UINT    deviceId; // MCI 장치 ID (초기값 0)
};
// 초기화
bool InitAllSounds();
// 재생
void PlaySoundID(SOUNDID _id, bool _repeat = false);
// 릴리즈 
void ReleaseAllSounds();