#pragma once
#include <queue>
#include "MathHelper.h"
class Enemy;
class Scene;
class Boss;
class Background;
struct SpawnInfo
{
	float spawnTime;
	Enemy* enemy;

};
struct CompareSpawnInfo
{
	bool operator()(const SpawnInfo& a, const SpawnInfo& b) const {
		return a.spawnTime > b.spawnTime;
	}
};
class EnemySpawnManger
{ 
	DECLARE_SINGLE(EnemySpawnManger)

public:
	void Init();
	void Update();
	void StopSpawn();
	void Realese();

public:
	BezierPathData* GetPath(wstring pathName)
	{
		return  m_calcedPath[pathName];
	}
public:
	void AddEnemySpawnQueue(SpawnInfo _spawn);
	void AddBossSpawn(float _time);
	void SetSpawnScene(Scene* scene)
	{
		m_spawnTargetScene = scene;
	}
	void SetBG(Background* bg) { m_bg = bg; };
private: 
	bool TryToSpawn();
	bool TryToSpawnBoss();
	void AssignPath();
private:
	float m_currentTime;
	Scene* m_spawnTargetScene;
	Background* m_bg;
	std::priority_queue<SpawnInfo, std::vector<SpawnInfo>, CompareSpawnInfo> m_enemySpawnQueue;
	std::unordered_map<wstring, BezierPathData*> m_calcedPath;
	float m_bossSpawnTime;
	bool m_bossSpawned;
};

