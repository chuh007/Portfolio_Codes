#include "pch.h"
#include "PureBulletHellPattern.h"
#include "EnemyProjectile.h"

PureBulletHellPattern::PureBulletHellPattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name)
    : Pattern(_owner, _target, _patternUseTime, _mover, _name)
	, m_projectileType(PoolType::EnemyProjectile)
	, m_fireCount(25)
	, m_speed(175)
	, m_timer(0)
	, m_secondTimer(0)
	, m_thirdTimer(0)
{
	m_decValue = 0.25f;
	m_bulletTex1 = GET_SINGLE(ResourceManager)->GetTexture(L"RedBullet1");
	m_bulletTex2 = GET_SINGLE(ResourceManager)->GetTexture(L"PurpleBullet1");
	m_bulletTex3 = GET_SINGLE(ResourceManager)->GetTexture(L"BlueBullet1");
}

PureBulletHellPattern::~PureBulletHellPattern()
{
}

void PureBulletHellPattern::Update()
{
	Pattern::Update();
	m_curTime += fDT;
	m_timer += fDT;
	m_secondTimer+= fDT;
	m_thirdTimer += fDT;
	if (m_curTime > m_BaseShoutCooldown)
	{
		m_curTime = 0;
		BaseShoot();
	}
	if (m_timer > 10.f && m_secondTimer >= 0.5f)
	{
		m_secondTimer = 0;
		SecondFire();
	}
	if (m_timer > 20.f && m_thirdTimer >= 1.f)
	{
		m_thirdTimer = 0;
		ThirdFire();
	}
}


void PureBulletHellPattern::BaseShoot()
{
	float angleStep = 360.f / (float)m_fireCount;
	float spawnRadius = 50.f; 
	Vec2 ownerPos = m_owner->GetPos();
	ownerPos += {rand() % 100 - 50, rand() % 100 - 50};
	GET_SINGLE(ResourceManager)->Play(L"FireSound2");
	for (int i = 0; i < m_fireCount; ++i)
	{
		auto* projectile = PoolManager::GetInst()->
			Pop<EnemyProjectile>(PoolType::EnemyProjectile);

		projectile->SetSize({ 20.f, 20.f});
		projectile->SetColliderSize(10.f);
		projectile->SetTexture(m_bulletTex1);

		float currentAngle = angleStep * i;
		float rad = currentAngle * D2R;

		float offsetX = cos(rad) * spawnRadius;
		float offsetY = -sin(rad) * spawnRadius;

		projectile->SetPos({ ownerPos.x + offsetX, ownerPos.y + offsetY });

		projectile->SetDir(currentAngle);
		projectile->SetSpeed(m_speed);
	}
}

void PureBulletHellPattern::SecondFire()
{
	float angleStep = 360.f / (float)8;
	float spawnRadius = 50.f;
	float randomangle = rand() % 60;
	Vec2 ownerPos1 = { GAME_WIDTH / 5, GAME_HEIGHT / 3 };
	GET_SINGLE(ResourceManager)->Play(L"FireSound");
	ownerPos1 += {rand() % 100 - 50, rand() % 100 - 50};
	for (int i = 0; i < 8; ++i)
	{
		auto* projectile = PoolManager::GetInst()->
			Pop<EnemyProjectile>(PoolType::EnemyProjectile);

		projectile->SetSize({ 20.f, 20.f });
		projectile->SetColliderSize(10.f);
		projectile->SetTexture(m_bulletTex2);

		float currentAngle = angleStep * i + randomangle;
		float rad = currentAngle * D2R;

		float offsetX = cos(rad) * spawnRadius;
		float offsetY = -sin(rad) * spawnRadius;

		projectile->SetPos({ ownerPos1.x + offsetX, ownerPos1.y + offsetY });

		projectile->SetDir(currentAngle);
		projectile->SetSpeed(m_speed * 0.5f);
	}
	Vec2 ownerPos2 = { GAME_WIDTH / 5 * 4, GAME_HEIGHT / 3 };
	ownerPos2 += {rand() % 100 - 50, rand() % 100 - 50};
	for (int i = 0; i < 8; ++i)
	{
		auto* projectile = PoolManager::GetInst()->
			Pop<EnemyProjectile>(PoolType::EnemyProjectile);

		projectile->SetSize({ 20.f, 20.f });
		projectile->SetColliderSize(10.f);
		projectile->SetTexture(m_bulletTex2);

		float currentAngle = angleStep * i + randomangle;
		float rad = currentAngle * D2R;

		float offsetX = cos(rad) * spawnRadius;
		float offsetY = -sin(rad) * spawnRadius;

		projectile->SetPos({ ownerPos2.x + offsetX, ownerPos2.y + offsetY });

		projectile->SetDir(currentAngle);
		projectile->SetSpeed(m_speed * 0.5f);
	}
}

void PureBulletHellPattern::ThirdFire()
{
	float angleStep = 360.f / (float)35;
	float spawnRadius = 50.f;
	float randomangle = rand() % 60;
	Vec2 ownerPos1 = { 0, GAME_HEIGHT / 3 };
	ownerPos1 += {0, rand() % 100 - 50};
	for (int i = 0; i < 35; ++i)
	{
		auto* projectile = PoolManager::GetInst()->
			Pop<EnemyProjectile>(PoolType::EnemyProjectile);

		projectile->SetSize({ 20.f, 20.f });
		projectile->SetColliderSize(10.f);
		projectile->SetTexture(m_bulletTex3);

		float currentAngle = angleStep * i + randomangle;
		float rad = currentAngle * D2R;

		float offsetX = cos(rad) * spawnRadius;
		float offsetY = -sin(rad) * spawnRadius;

		projectile->SetPos({ ownerPos1.x + offsetX, ownerPos1.y + offsetY });

		projectile->SetDir(currentAngle);
		projectile->SetSpeed(m_speed * 1.25f);
	}
	Vec2 ownerPos2 = { GAME_WIDTH, GAME_HEIGHT / 3 };
	ownerPos1 += {0, rand() % 100 - 50};
	for (int i = 0; i < 35; ++i)
	{
		auto* projectile = PoolManager::GetInst()->
			Pop<EnemyProjectile>(PoolType::EnemyProjectile);

		projectile->SetSize({ 20.f, 20.f });
		projectile->SetColliderSize(10.f);
		projectile->SetTexture(m_bulletTex3);

		float currentAngle = angleStep * i + randomangle;
		float rad = currentAngle * D2R;

		float offsetX = cos(rad) * spawnRadius;
		float offsetY = -sin(rad) * spawnRadius;

		projectile->SetPos({ ownerPos2.x + offsetX, ownerPos2.y + offsetY });

		projectile->SetDir(currentAngle);
		projectile->SetSpeed(m_speed * 1.25f);
	}
}
