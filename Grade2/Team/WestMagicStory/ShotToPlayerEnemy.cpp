#include "pch.h"
#include "PoolManager.h"
#include "EnemyProjectile.h"
#include "PlayerManager.h"
#include "EnemyMovement.h"
#include "ShotToPlayerEnemy.h"
#include "ResourceManager.h"

ShotToPlayerEnemy::ShotToPlayerEnemy()
{
	Enemy::SetTexture(GET_SINGLE(ResourceManager)->GetTexture(L"Enemy_3"));
	m_pBulletTexture = GET_SINGLE(ResourceManager)->GetTexture(L"RedSword");
}

void ShotToPlayerEnemy::Update()
{
	m_currentTime += fDT;

	if (m_currentTime >= m_fireInterval)
	{
		isFire = true;
		targetPos = GET_SINGLE(PlayerManager)->GetPlayer()->GetPos();
		auto* movement = GetComponent<EnemyMovement>();
		tempSpeed = movement->GetSpeed();
		movement->SetSpeed(0);
	}

	if (isFire)
	{
		TryToShot();
	}
}

void ShotToPlayerEnemy::SetFireTime(float _time , float _fireRate, int _shotCount, bool _fireOnce)
{
	m_fireInterval = _time;
	fireRate = _fireRate;
	shotCount = _shotCount;
	fireOnce = _fireOnce;
}

void ShotToPlayerEnemy::TryToShot()
{
	if (m_currentTime >= fireRate)
	{
		if (m_curShot >= shotCount)
		{
			isFire = false;

			auto* movement = GetComponent<EnemyMovement>();
			movement->SetSpeed(tempSpeed);
			if (fireOnce == false)
				m_curShot = 0;

			return;
		}
		auto* projectile = PoolManager::GetInst()->
			Pop<EnemyProjectile>(PoolType::EnemyProjectile);
		projectile->SetSize({ 10.f, 10.f });
		projectile->SetTexture(m_pBulletTexture);
		projectile->SetColliderSize(7.5f);
		projectile->SetPos(GetPos());
		projectile->SetDir(targetPos - GetPos());
		projectile->SetSpeed(300.f);

		m_currentTime = 0;
		++m_curShot;
	}
}
