#include "pch.h"
#include "Scene.h"
#include "Enemy.h"
#include "Boss.h"
#include "EnemySpawnManger.h"
#include "BossHPBar.h"
#include "BossMover.h"
void EnemySpawnManger::Init()
{
	GET_SINGLE(EnemySpawnManger)->Realese();
	m_calcedPath.clear();
	AssignPath();
	m_currentTime = 0;
	m_bossSpawned = false;
	m_bossSpawnTime = 20000.f;

}

void EnemySpawnManger::Update()
{
	m_currentTime += fDT;
	TryToSpawn();
	m_bossSpawned = TryToSpawnBoss();
}

void  EnemySpawnManger::StopSpawn()
{
}

void EnemySpawnManger::Realese()
{
    while (!m_enemySpawnQueue.empty())
    {
        SpawnInfo info = m_enemySpawnQueue.top();

		SAFE_DELETE(info.enemy);

        m_enemySpawnQueue.pop();
    }


	for (std::pair<wstring, BezierPathData*> kvp : m_calcedPath)
	{
		SAFE_DELETE(kvp.second)
	}
	m_calcedPath.clear();
}

void EnemySpawnManger::AddEnemySpawnQueue(SpawnInfo _spawnInfo)
{
	cout << "enemy added" << endl;
	m_enemySpawnQueue.push(_spawnInfo);
}

void EnemySpawnManger::AddBossSpawn(float _time)
{
	m_bossSpawnTime = _time;
}

bool EnemySpawnManger::TryToSpawn()
{
	if (m_enemySpawnQueue.empty())
	{
		return false;
	}

	SpawnInfo info = m_enemySpawnQueue.top();
	if (m_currentTime >= info.spawnTime)
	{
		m_spawnTargetScene->AddObject(info.enemy, Layer::ENEMY);
		m_enemySpawnQueue.pop();
		return true;
	}
	return false;
}

bool EnemySpawnManger::TryToSpawnBoss()
{
	if (m_bossSpawned)
		return true;
	if (m_currentTime >= m_bossSpawnTime)
	{
		Boss* boss = m_spawnTargetScene->Spawn<Boss>(Layer::ENEMY, { GAME_WIDTH / 2, -100 }, { 30.f,70.f });;
		auto* hpBar = m_spawnTargetScene->Spawn<BossHPBar>(Layer::UI, { GAME_WIDTH / 2, 25 }, { GAME_WIDTH - 20, 50 });
		hpBar->SetBoss(boss);
		boss->SetBackground(m_bg);
		boss->GetComponent<BossMover>()->MoveTo({ GAME_WIDTH / 2, GAME_HEIGHT / 4 }, 0.5f);
		boss->Coroutine([=]()
			{
				boss->Start();
			}, 0.5f);
		return true;
	}
	return false;
}

void EnemySpawnManger::AssignPath()
{
	BezierPathData* defaultPath = new BezierPathData;
	defaultPath->BezierPathData::CalculateArcLengthMap({ {0,0},{200,200},{400,-200},{600,0} });
	m_calcedPath.insert({ L"Default", defaultPath });
	BezierPathData* reverse = new BezierPathData;
	reverse->BezierPathData::CalculateArcLengthMap({ {0,0},{-200,-500},{-400,100},{-600,-300} });
	m_calcedPath.insert({ L"Reverse", reverse });
	BezierPathData* circlePath = new BezierPathData;
	circlePath->BezierPathData::CalculateArcLengthMap({ {-100,300},{100,0},{600,600},{400,300} });
	m_calcedPath.insert({ L"Circle", circlePath });

	BezierPathData* up = new BezierPathData;
	up->BezierPathData::CalculateArcLengthMap({ {0,0}, {0,100} });
	m_calcedPath.insert({ L"Up", up });

	BezierPathData* down = new BezierPathData;
	down->BezierPathData::CalculateArcLengthMap({ {0,0}, {0,-100} });
	m_calcedPath.insert({ L"Down", down });

	BezierPathData* left = new BezierPathData;
	left->BezierPathData::CalculateArcLengthMap({ {0,0}, {-100,0} });
	m_calcedPath.insert({ L"Left", left });

	BezierPathData* rightPath = new BezierPathData;
	rightPath->BezierPathData::CalculateArcLengthMap({ {0,0}, {100,0} });
	m_calcedPath.insert({ L"Right", rightPath });

	BezierPathData* zigzagR = new BezierPathData;
	zigzagR->BezierPathData::CalculateArcLengthMap({ {0,0}, {100,100} });
	m_calcedPath.insert({ L"ZigzagR", zigzagR });

	BezierPathData* zigzagL = new BezierPathData;
	zigzagL->BezierPathData::CalculateArcLengthMap({ {0,0}, {-100,100} });
	m_calcedPath.insert({ L"ZigzagL", zigzagL });

	BezierPathData* sideMove = new BezierPathData;
	sideMove->BezierPathData::CalculateArcLengthMap({ {0,0},{200,400},{900,0} });
	m_calcedPath.insert({ L"Left-Right", sideMove });

	BezierPathData* sideMove2 = new BezierPathData;
	sideMove2->BezierPathData::CalculateArcLengthMap({ {0,0},{-200,400},{-900,0} });
	m_calcedPath.insert({ L"Right-Left", sideMove2 });

	BezierPathData* ArcMoveL = new BezierPathData;
	ArcMoveL->BezierPathData::CalculateArcLengthMap({ {0,0},{400,-200},{400,-400}, {-100, -400 } });
	m_calcedPath.insert({ L"ArcMoveL", ArcMoveL });

	BezierPathData* ArcMoveR = new BezierPathData;
	ArcMoveR->BezierPathData::CalculateArcLengthMap({ {0,0},{-400,-200},{-400,-400}, {100, -400 } });
	m_calcedPath.insert({ L"ArcMoveR", ArcMoveR });
}
