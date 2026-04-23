#include "pch.h"
#include "GateOfBabylonPattern.h"
#include "EnemyProjectile.h"
#include "PoolManager.h"
#include "Effect.h"

// GOB ³Ê³¦
// »ó´Ü ·£´ý À§Ä¡(¸¶¹ýÁø) »ý¼º
// »ý¼ºµÈ Åº¸·Àº ´ë±âÇß´Ù°¡ »çÃâ
// ½ñ¾ÆÁö´Â ´À³¦
// Åº¿¡ ¼ÓµµÂ÷ÀÌ Áà¼­ ÃÒ¶ó¶ô ÇÏ´Â ´À³¦

GateOfBabylonPattern::GateOfBabylonPattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name)
	: Pattern(_owner, _target, _patternUseTime, _mover, _name)
	, m_basePortalCount(4) 
	, m_trailCount(5)
	, m_baseSpeed(250.f)
	, m_timer(0)
	, m_rotate(0)
{

	m_decValue = 0.65f;
	m_swordTexs[0] = GET_SINGLE(ResourceManager)->GetTexture(L"GreenSword");
	m_swordTexs[1] = GET_SINGLE(ResourceManager)->GetTexture(L"RedSword");
	m_swordTexs[2] = GET_SINGLE(ResourceManager)->GetTexture(L"YellowSword");
	m_bulletTex = GET_SINGLE(ResourceManager)->GetTexture(L"BlueSword");
}

GateOfBabylonPattern::~GateOfBabylonPattern()
{
}

void GateOfBabylonPattern::Update()
{
	Pattern::Update();
	m_curTime += fDT;
	m_timer += fDT;

	if (m_curTime > m_BaseShoutCooldown)
	{
		m_curTime = 0;
		BaseShoot();
	}
	if (m_timer >= 1.f)
	{
		m_timer = 0;
		CircleFire();
	}
}

void GateOfBabylonPattern::BaseShoot()
{
	GET_SINGLE(ResourceManager)->Play(L"SwordSound");
	int currentPortalCount = m_basePortalCount + (int)(m_patternUseTime / 5.0f);
	float currentBaseSpeed = m_baseSpeed + (m_patternUseTime * 5.f);

	for (int i = 0; i < currentPortalCount; ++i)
	{
		Vec2 startPos = { (float)(rand() % GAME_WIDTH), (float)(rand() % (GAME_HEIGHT / 5)) };
		Vec2 targetPos = { (float)(rand() % GAME_WIDTH), (float)(GAME_HEIGHT) };
		Vec2 dir = targetPos - startPos;
		dir.Normalize();

		float randomLaunchDelay = (rand() % 100) / 100.f;

		auto* magicEffect = GET_SINGLE(PoolManager)->Pop<Effect>(PoolType::Effect);

		magicEffect->SetPos(startPos);
		magicEffect->SetSize({ 100.f, 100.f });
		magicEffect->Coroutine([=]()
			{
				GET_SINGLE(PoolManager)->Push<Effect>(PoolType::Effect, magicEffect);
			}, randomLaunchDelay + 0.1f);
		int idx = rand() % 3;
		for (int j = 0; j < m_trailCount; ++j)
		{
			auto* projectile = GET_SINGLE(PoolManager)->Pop<EnemyProjectile>(PoolType::EnemyProjectile);
			projectile->SetSize({ 20.f, 24.f });
			projectile->SetTexture(m_swordTexs[idx]);
			projectile->SetColliderSize(10.f);
			projectile->SetPos(startPos);
			projectile->SetDir(dir);
			projectile->SetSpeed(0.f);

			float finalSpeed = currentBaseSpeed * (1.0f - (j * 0.05f));

			projectile->Coroutine([=]()
				{
					projectile->SetSpeed(finalSpeed);
				}, randomLaunchDelay);
		}
	}
}

void GateOfBabylonPattern::CircleFire()
{
	m_rotate += 20;
	float angle = 360.f / (float)5;
	for (int i = 0; i < 5; ++i)
	{
		auto* projectile = PoolManager::GetInst()->
			Pop<EnemyProjectile>(PoolType::EnemyProjectile);
		projectile->SetSize({ 15.f, 30.f });
		projectile->SetColliderSize(10.f);
		projectile->SetTexture(m_bulletTex);
		projectile->SetPos(m_owner->GetPos());
		projectile->SetDir(angle * i + m_rotate);
		projectile->SetSpeed(300.f);
	}
}
