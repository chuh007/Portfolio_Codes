#include "pch.h"

#include <functional>
#include "CircleMoveEnemy.h"
#include "EnemyMovement.h"
#include "EnemySpawnManger.h"
#include "EnemyProjectile.h"
#include "ResourceManager.h"
#include "Health.h"

CircleMoveEnemy::CircleMoveEnemy()
{
	Enemy::SetTexture((GET_SINGLE(ResourceManager)->GetTexture(L"Enemy_2")));
	m_pBulletTexture = GET_SINGLE(ResourceManager)->GetTexture(L"IceBullet");
	fireCount = 0;
	fireTime = 0;
}

CircleMoveEnemy::~CircleMoveEnemy()
{
	Enemy::~Enemy();
}

void CircleMoveEnemy::Update()
{
	fireTime += fDT;
	if (fireTime >= 1.5f)
	{
		float dir = 360.f / fireCount;
		for (int i = 0; i <= fireCount; ++i)
		{
			auto* projectile = PoolManager::GetInst()->
				Pop<EnemyProjectile>(PoolType::EnemyProjectile);
			projectile->SetSize({ 10.f, 10.f });
			projectile->SetColliderSize(7.5f);
			projectile->SetPos(GetPos());
			projectile->SetDir(-90.f + i * dir);
			projectile->SetSpeed(200.f);
			projectile->SetTexture(m_pBulletTexture);

			fireTime = 0;
		}
	}
}

void CircleMoveEnemy::SetShotCount(int count)
{
	fireCount = count;
}
