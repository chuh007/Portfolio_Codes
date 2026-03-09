#pragma once
#include "Define.h"
#include "Player.h"
#include <time.h>
class GameManager
{
private:
	GameManager();
	~GameManager();
public:
	void Init();
	void Update();
	static GameManager* GetInst()
	{
		if (m_inst == nullptr)
			m_inst = new GameManager;
		return m_inst;
	}
	static void DestoryInst()
	{
		SAFE_DELETE(m_inst);
	}
	void CheckGameOver();
	int GetWaveCount() { return m_waveCnt; }
	bool isGameOver() { return gameOver; }
	int GetMuzzleFireCount() { return muzzleFireCount; }
	void SetMuzzleFireCount(int newMuzzleFireCount) { muzzleFireCount = newMuzzleFireCount; }
private:
	int muzzleFireCount;
	static GameManager* m_inst;
	bool m_isBattleMode;
	bool gameOver;
	int m_waveCnt;
	clock_t m_oldTime;
	clock_t m_currentTime;
};

