#include "EnemyCollisionManager.h"
#include "Enemy.h"

EnemyCollisionManager* EnemyCollisionManager::m_inst = __nullptr;
EnemyCollisionManager::EnemyCollisionManager()
	:hit(0)
{
}

EnemyCollisionManager::~EnemyCollisionManager()
{
}

void EnemyCollisionManager::Add(Enemy* _enemy)
{
	m_enemys.push_back(_enemy);
}

void EnemyCollisionManager::CheckCollision(Bullet* _bullet)
{
	for (int i = 0;i < m_enemys.size(); i++)
	{
		if (abs(_bullet->GetPos().x 
			- m_enemys[i]->GetPos().x) <= 1
			&& abs(_bullet->GetPos().y
			- m_enemys[i]->GetPos().y) <= 1)
		{
			m_enemys[i]->CheckFeedback(_bullet->GetDamage());
			_bullet->isDead = true;
			hit++;
		}
	}
}

void EnemyCollisionManager::UpdateEnemyList()
{
	for (int i = 0; i < m_enemys.size(); ++i)
	{
		if (m_enemys[i]->isDead)
		{
			m_enemys.erase(m_enemys.begin() + i);

		}
	}
}

void EnemyCollisionManager::GameOver()
{
	m_enemys.clear();
}
