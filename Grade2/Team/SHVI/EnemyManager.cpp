#include "EnemyManager.h"
#include "Enemy.h"
#include "FasterEnemy.h"
#include "InvisibleEnemy.h"
#include "FlipEnemy.h"
#include "EnemyData.h"
#include "Console.h"
#include <random>

EnemyManager* EnemyManager::m_inst = __nullptr;

EnemyManager::EnemyManager()
{
	srand((unsigned int)time(NULL));
	oldTime = clock();
	currentTime = clock();
	timeMultiply = 1;
	lifePer = 1;
	FasterPer = 0;
	InvisiblePer = 0;
	FlipPer = 0;
	spawnTimeMultiply = 1.5f;
}

void EnemyManager::Update()
{
	currentTime = clock();
	if (currentTime - oldTime >= SPAWN_TIME * 1000 * spawnTimeMultiply)
	{
		oldTime = currentTime;
		Dir dir = (Dir)(rand() % 4);
		int randnum = rand() % 100 + 1;
		if (randnum <= FasterPer)
		{
			new FasterEnemy(dir, timeMultiply * 0.025, 1);
		}
		else if (randnum <= FasterPer + InvisiblePer)
		{
			new InvisibleEnemy(dir, timeMultiply * 5, SetHp(10, lifePer));
		}
		else if(randnum <= FasterPer + InvisiblePer + FlipPer)
		{
			new FlipEnemy(dir, timeMultiply, SetHp(10, lifePer));
		}
		else
		{
			new Enemy(dir, timeMultiply, SetHp(10 , lifePer));
		}

	}
}

int EnemyManager::SetHp(int baseHp, float hpPer)
{
	return (int)(baseHp * hpPer);
}

void EnemyManager::Init()
{
	oldTime = clock();
	currentTime = clock();
}

void EnemyManager::WaveToEnemySet(int Wave)
{
	lifePer = max(1.0f, pow(1.15f, Wave / 5));
	if (Wave >= 20)
	{
		FasterPer = 20;
		FlipPer = 10;
		InvisiblePer = 5;
		if (Wave >= 50)
		{
			spawnTimeMultiply = 0.3f;
		}
		else if (Wave >= 40)
		{
			spawnTimeMultiply = 0.4f;
		}
		else if (Wave >= 30)
		{
			spawnTimeMultiply = 0.5f;
		}
		else if (Wave >= 22)
		{
			spawnTimeMultiply = 0.65f;
		}
		else
		{
			spawnTimeMultiply = 0.75f;
		}
	}
	else if (Wave >= 15)
	{
		FasterPer = 15;
		FlipPer = 7;
		InvisiblePer = 2;
		if (Wave >= 17)
		{
			spawnTimeMultiply = 0.8f;
		}
		else
		{
			spawnTimeMultiply = 0.825f;
		}
	}
	else if (Wave >= 10)
	{
		if (Wave >= 12)
		{
			spawnTimeMultiply = 0.85f;
		}
		else
		{
			spawnTimeMultiply = 0.925f;
		}
		FlipPer = 5;
		FasterPer = 12;
	}
	else if (Wave >= 5)
	{
		FasterPer = 10;
		if (Wave >= 7)
		{
			spawnTimeMultiply = 1;
		}
		else
		{
			spawnTimeMultiply = 1.15f;
		}
	}
	else
	{
		if (Wave >= 2)
		{
			spawnTimeMultiply = 1.25f;
		}
		else
		{
			spawnTimeMultiply = 1.5f;
		}
		FasterPer = 0;
		InvisiblePer = 0;
		FlipPer = 0;
	}
}
