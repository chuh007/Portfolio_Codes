#include "pch.h"
#include "CircleToPlayerPattern.h"
#include "EnemyProjectile.h"
// 직선으로 내려오는 탄이 있음.
// 원탄 만들고 플레이어쪽으로 보내기
CircleToPlayerPattern::CircleToPlayerPattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name)
	: Pattern(_owner, _target, _patternUseTime, _mover, _name)
	, m_projectileType(PoolType::EnemyProjectile)
	, m_fireCount(20)
	, m_speed(250)
{
	m_decValue = 0.9f;
	m_bulletTex = GET_SINGLE(ResourceManager)->GetTexture(L"GreenBullet1");
	m_swordTex = GET_SINGLE(ResourceManager)->GetTexture(L"GreenSword");
}

CircleToPlayerPattern::~CircleToPlayerPattern()
{
}

void CircleToPlayerPattern::Update()
{
	Pattern::Update();
	m_curTime += fDT;
	if (m_curTime > m_BaseShoutCooldown)
	{
		m_curTime = 0;
		int x = (rand() % GAME_WIDTH / 2) + GAME_WIDTH / 4;
		int y = (GAME_HEIGHT / 5) + rand() % 100;
		BaseShoot();
		RayShout();
	}
}

void CircleToPlayerPattern::BaseShoot()
{
	GET_SINGLE(ResourceManager)->Play(L"FireSound");
	GET_SINGLE(ResourceManager)->Play(L"CircleSound");
	float angle = 360.f / (float)m_fireCount;
	auto target = m_target;
	float speed = m_speed;
	for (int i = 0; i < m_fireCount; ++i)
	{
		auto* projectile = PoolManager::GetInst()->
			Pop<EnemyProjectile>(PoolType::EnemyProjectile);
		projectile->SetSize({ 16.f, 16.f });
		projectile->SetColliderSize(7.25f);
		projectile->SetTexture(m_bulletTex);
		projectile->SetPos(m_owner->GetPos());
		projectile->SetDir(angle * i);
		projectile->SetSpeed(speed);
		projectile->Coroutine([=]()
			{
				projectile->SetDir(target->GetPos() - projectile->GetPos());
				projectile->SetSpeed(speed * 0.75f);
			}, 0.5f);
	}
}

void CircleToPlayerPattern::RayShout()
{
	for (int i = 0; i < 5; ++i)
	{
		auto* projectile = PoolManager::GetInst()->
			Pop<EnemyProjectile>(PoolType::EnemyProjectile);
		projectile->SetSize({ 16.f, 24.f });
		projectile->SetColliderSize(8.f);
		projectile->SetTexture(m_swordTex);
		projectile->SetPos({ GAME_WIDTH / 6 * (i + 1), 0 });
		projectile->SetDir({ 0.f, 1.f });
		projectile->SetSpeed(m_speed);
	}
}

