#include "pch.h"
#include "SpiralPattern.h"
#include "EnemyProjectile.h"
#include "PoolManager.h"


SpiralPattern::SpiralPattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name)
    : Pattern(_owner, _target, _patternUseTime, _mover, _name)
    , m_projectileType(PoolType::EnemyProjectile)
    , m_armCount(14)
    , m_speed(200)
    , m_fireCount(20)
    , m_isleft(false)
{
    m_decValue = 1.2f;
    m_bulletTex = GET_SINGLE(ResourceManager)->GetTexture(L"GreenBullet3");
}

SpiralPattern::~SpiralPattern()
{
}

void SpiralPattern::Update()
{
    Pattern::Update();
    m_curTime += fDT;

    if (m_curTime > m_BaseShoutCooldown)
    {
        m_curTime = 0;
        int x = (rand() % 200) + GAME_WIDTH / 2 - 100;
        int y = (GAME_HEIGHT / 5) + rand() % 50;
        m_mover->MoveTo({ x,y }, m_BaseShoutCooldown);
        BaseShoot();
    }
}
Vec2 SpiralPattern::GetSpiralPos(Vec2 centerPos, float radius, int armCount, int armIndex)
{
    float angleDeg = (360.f / (float)armCount) * armIndex + 90.f;

    if (m_isleft)
    {
        angleDeg = 180.f - angleDeg;
    }

    float rad = angleDeg * D2R;

    float x = centerPos.x + cosf(rad) * radius;
    float y = centerPos.y + sinf(rad) * radius;

    return Vec2(x, y);
}

void SpiralPattern::BaseShoot()
{
    float radius = 50.f;
    float speed = m_speed;

    for (int i = 0; i < m_armCount; ++i)
    {
        Vec2 spawnPos = GetSpiralPos(m_owner->GetPos(), radius, m_armCount, i);

        radius += 20.f;

        float angleStep = 360.f / (float)m_fireCount;

        for (int j = 0; j < m_fireCount; ++j)
        {
            auto* projectile = PoolManager::GetInst()->Pop<EnemyProjectile>(PoolType::EnemyProjectile);

            projectile->SetSize({ 10.f, 20.f });
            projectile->SetColliderSize(5.f);
            projectile->SetPos(spawnPos);
            projectile->SetTexture(m_bulletTex);

            float finalAngle = angleStep * j;


            projectile->SetDir(finalAngle);
            projectile->SetSpeed(0.f);

            projectile->Coroutine([=]()
                {
                    projectile->SetSpeed(speed);
                }, i * 0.1f);
        }
    }
    m_isleft = !m_isleft;
}