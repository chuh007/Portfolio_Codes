#include "pch.h"
#include "IcicleFallPattern.h"
#include "EnemyProjectile.h"
// 좌우로 사격하다 각 줄어들기
// 3발쯤 쏘는데 속도가 다름.
// 가다가 어느정도 가면 방향 꺾기
// 꺾은 뒤로는 속도 같음
// 부체꼴 5갈래쯤으로 플레이어한테 쏨
// 부체꼴은 좌우사격 2번당 1번
IcicleFallPattern::IcicleFallPattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name)
	: Pattern(_owner, _target, _patternUseTime, _mover, _name)
	, m_projectileType(PoolType::EnemyProjectile)
	, m_fireCount(11)
	, m_curCnt(0)
	, m_iceAngle(100)
	, m_iceShootCount(3)
	, m_iceSpeed(350)
	, m_circularAngle(90.f)
	, circularShootCount(5)
	, circularSpeed(200)
{
	m_decValue = 0.75f;
	m_iceTex = GET_SINGLE(ResourceManager)
		->GetTexture(L"IceBullet");
	m_bulletTex = GET_SINGLE(ResourceManager)
		->GetTexture(L"BlueBullet");
}

IcicleFallPattern::~IcicleFallPattern()
{
}

void IcicleFallPattern::Update()
{
	Pattern::Update();
	m_curTime += fDT;
	if (m_curTime > m_BaseShoutCooldown)
	{
		m_curTime = 0;
		m_curCnt++;
		if (m_fireCount <= m_curCnt)
		{
			m_curCnt = 0;
		}
		BaseShoot();
		if (m_curCnt % 2 == 0)
			CircularShoot();
	}
}

void IcicleFallPattern::BaseShoot()
{
	GET_SINGLE(ResourceManager)->Play(L"FireSound");
	for (int i = 1; i <= m_iceShootCount; ++i)
	{
		float speed = m_iceSpeed;
		int cnt = m_curCnt;
		auto* projectile1 = PoolManager::GetInst()->
			Pop<EnemyProjectile>(PoolType::EnemyProjectile);
		projectile1->SetSize({ 16.f, 16.f });
		projectile1->SetColliderSize(6.5f);
		projectile1->SetTexture(m_iceTex);
		projectile1->SetPos(m_owner->GetPos());
		projectile1->SetDir(-90.f + m_iceAngle - m_curCnt * 5.f);
		projectile1->SetSpeed(speed / m_iceShootCount * i);
		projectile1->Coroutine([=]()
			{
				projectile1->SetSpeed(150);
				projectile1->SetDir(-70.f - m_curCnt * 5.f);
			}, 1.f);

		auto* projectile2 = PoolManager::GetInst()->
			Pop<EnemyProjectile>(PoolType::EnemyProjectile);
		projectile2->SetSize({ 16.f, 16.f });
		projectile2->SetColliderSize(6.5f);
		projectile2->SetTexture(m_iceTex);
		projectile2->SetPos(m_owner->GetPos());
		projectile2->SetDir(-90.f - m_iceAngle + m_curCnt * 5.f);
		projectile2->SetSpeed(speed / m_iceShootCount * i);
		projectile2->Coroutine([=]()
			{
				projectile2->SetSpeed(150);
				projectile2->SetDir(-110.f + m_curCnt * 5.f);
			}, 1.f);
	}
}

void IcicleFallPattern::CircularShoot()
{
	float angle = m_circularAngle / circularShootCount;
	float speed = circularSpeed;
	for (int i = -circularShootCount / 2; i <= circularShootCount / 2; ++i)
	{
		auto* projectile = PoolManager::GetInst()->
			Pop<EnemyProjectile>(PoolType::EnemyProjectile);
		projectile->SetSize({ 22.5f, 22.5f });
		projectile->SetColliderSize(10.5f);
		projectile->SetTexture(m_bulletTex);
		projectile->SetPos(m_owner->GetPos());
		projectile->SetDir(-90.f + angle * i);
		projectile->SetSpeed(speed);
	}
}
