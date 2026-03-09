#include "pch.h"
#include "MultiSpeedRadialPattern.h"
#include "EnemyProjectile.h"

MultiSpeedRadialPattern::MultiSpeedRadialPattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name)
	: Pattern(_owner, _target, _patternUseTime, _mover, _name)
	, m_projectileType(PoolType::EnemyProjectile)
	, m_fireCount(5)
	, m_speed(400)
	, m_moveTime(0)
{
	m_decValue = 1.0f;
	m_bulletTex = GET_SINGLE(ResourceManager)->GetTexture(L"BlueBullet3");
}

MultiSpeedRadialPattern::~MultiSpeedRadialPattern()
{
}

void MultiSpeedRadialPattern::Update()
{
	Pattern::Update();
	m_curTime += fDT;
	m_moveTime += fDT;
	if (m_curTime > 0.4f)
	{
		m_curTime = 0;
		BaseShoot();
	}
	if (m_moveTime > m_BaseShoutCooldown)
	{
		m_moveTime = 0;
		int x = (rand() % GAME_WIDTH / 2) + GAME_WIDTH / 4;
		int y = (GAME_HEIGHT / 5) + rand() % 100;
		m_mover->MoveTo({ x,y }, m_BaseShoutCooldown);
	}
}

void MultiSpeedRadialPattern::BaseShoot()
{
	Vec2 ownerPos = m_owner->GetPos();
	Vec2 targetPos = m_target->GetPos();
	Vec2 dir = targetPos - ownerPos;
	float radian = atan2f(-dir.y, dir.x);
	float totargetAngle = radian * (180.f / PI);

	GET_SINGLE(ResourceManager)->Play(L"FireSound");

	for (int i = 0; i < m_fireCount; ++i)
	{
		for (int j = 0; j < 6; ++j)
		{
			auto* projectile = PoolManager::GetInst()->Pop<EnemyProjectile>(PoolType::EnemyProjectile);
			projectile->SetSize({ 10.f, 20.f });
			projectile->SetColliderSize(5.5f);
			projectile->SetTexture(m_bulletTex);
			projectile->SetPos(m_owner->GetPos());
			projectile->SetDir(totargetAngle + 72.f * i);
			projectile->SetSpeed(m_speed * (1.f - j * 0.15f));
		}
	}
}