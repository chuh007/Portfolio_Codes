#include "pch.h"
#include <functional>
#include "TestEnemy.h"
#include "EnemyMovement.h"
#include "EnemySpawnManger.h"
#include "EnemyProjectile.h"
#include "ResourceManager.h"
#include "Health.h"

TestEnemy::TestEnemy()
{
	m_pBulletTexture = GET_SINGLE(ResourceManager)->GetTexture(L"BlueBullet3");
	fireTime = 0;
}

TestEnemy::~TestEnemy()
{
	Enemy::~Enemy();

}

void TestEnemy::Update()
{
	fireTime += fDT;
	if (fireTime >= 0.75f)
	{
		auto* projectile = PoolManager::GetInst()->
			Pop<EnemyProjectile>(PoolType::EnemyProjectile);
		projectile->SetSize({ 20.f, 20.f });
		projectile->SetColliderSize(7.5f);
		projectile->SetPos(GetPos());
		projectile->SetDir(-90.f);
		projectile->SetSpeed(300.f);
		projectile->SetTexture(m_pBulletTexture);

		fireTime = 0;
	}
}
