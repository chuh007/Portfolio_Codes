#include "pch.h"
#include "CirclePattern.h"
#include "EnemyProjectile.h"
// 원형 사격
// 타탕 하고 속도 다르게 두번쏘기
CirclePattern::CirclePattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name)
	: Pattern(_owner, _target, _patternUseTime, _mover, _name)
	, m_projectileType(PoolType::EnemyProjectile)
	, m_fireCount(30)
	, m_speed(225)
{
	m_decValue = 1.9f;
	m_bulletTex = GET_SINGLE(ResourceManager)->GetTexture(L"OraneBullet3");
}

CirclePattern::~CirclePattern()
{
}

void CirclePattern::Update()
{
	Pattern::Update();
	m_curTime += fDT;
	if (m_curTime > m_BaseShoutCooldown)
	{
		m_curTime = 0;
		int x = (rand() % GAME_WIDTH / 2) + GAME_WIDTH / 4;
		int y = (GAME_HEIGHT / 5) + rand() % 100;
		m_mover->MoveTo({ x,y }, m_BaseShoutCooldown);
		BaseShoot();
	}
}
void CirclePattern::BaseShoot()
{
	float angle = 360.f / (float)m_fireCount;
	GET_SINGLE(ResourceManager)->Play(L"FireSound");
	for (int i = 0; i < m_fireCount; ++i)
	{
		auto* projectile = PoolManager::GetInst()->
			Pop<EnemyProjectile>(PoolType::EnemyProjectile);
		projectile->SetSize({ 10.f, 20.f });
		projectile->SetColliderSize(5.5f);
		projectile->SetTexture(m_bulletTex);
		projectile->SetPos(m_owner->GetPos());
		projectile->SetDir(angle * i);
		projectile->SetSpeed(m_speed);
	}
	for (int i = 0; i < m_fireCount; ++i)
	{
		auto* projectile = PoolManager::GetInst()->
			Pop<EnemyProjectile>(PoolType::EnemyProjectile);
		projectile->SetSize({ 10.f, 20.f });
		projectile->SetColliderSize(5.5f);
		projectile->SetTexture(m_bulletTex);
		projectile->SetPos(m_owner->GetPos());
		projectile->SetDir(angle * i);
		projectile->SetSpeed(m_speed * 0.8f);
	}
}
