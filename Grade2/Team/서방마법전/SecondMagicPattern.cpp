#include "pch.h"
#include "SecondMagicPattern.h"
#include "EnemyProjectile.h"
#include "PoolManager.h"

// 불사「불새 -봉익천상-」 모티브
// 움직이며 플레이어를 향해 조준탄 발사
// 조준탄은 크고, 지나간 자리에 랜덤하게 탄막 흩어놓음
// 조준탄 발사와 동시에 원형으로 탄 한번 뿌려줌
// 이걸 3번쯤 반복하며, 한쪽 끝 위치에 도달
// 거기서 원형으로 탄을 5번정도 발사함.
// 동시에 조준탄이 소환한 탄들이 랜덤하게 움직임

SecondMagicPattern::SecondMagicPattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name)
    : Pattern(_owner, _target, _patternUseTime, _mover, _name)
    , m_shotCount(0)
    , m_isFinishedMovement(false)
    , m_isBurstFired(false)
    , trailCount(15)
{
    m_decValue = 0.7f;

    m_phoenixTex = GET_SINGLE(ResourceManager)->GetTexture(L"RedBullet2");
    m_emberTex = GET_SINGLE(ResourceManager)->GetTexture(L"RedBullet1");
}

SecondMagicPattern::~SecondMagicPattern()
{
    m_embers.clear();
}

void SecondMagicPattern::Update()
{
    Pattern::Update();
    m_curTime += fDT;

    if (!m_isFinishedMovement)
    {
        if (m_curTime > 1.5f)
        {
            m_curTime = 0;

            if (m_shotCount < 3)
            {
                float nextX = (GAME_WIDTH / 4.f) * ((m_shotCount % 2) == 0 ? 1 : 3);
                float nextY = (GAME_HEIGHT / 5.f) + (rand() % 100);

                m_mover->MoveTo({ (int)nextX, (int)nextY }, 1.5f);
                BaseShoot();
                m_shotCount++;
            }
            else
            {
                m_isFinishedMovement = true;
                m_curTime = 0;
            }
        }
    }
    else
    {
        if (!m_isBurstFired && m_curTime > 0.5f)
        {
            FinalBurst();
            m_isBurstFired = true;
            m_mover->MoveTo({ GAME_WIDTH / 2, GAME_HEIGHT / 4 }, 0.75f);
        }

        if (m_isBurstFired && m_curTime > 1.f) 
        {
            m_curTime = 0;
            m_shotCount = 0;
            m_isFinishedMovement = false;
            m_isBurstFired = false;
            m_embers.clear();
        }
    }
}

void SecondMagicPattern::BaseShoot()
{
    GET_SINGLE(ResourceManager)->Play(L"FireSound");

    Vec2 ownerPos = m_owner->GetPos();
    Vec2 targetPos = m_target->GetPos();

    auto* phoenix = PoolManager::GetInst()->Pop<EnemyProjectile>(PoolType::EnemyProjectile);
    phoenix->SetTexture(m_phoenixTex);
    phoenix->SetSize({ 60.f, 60.f }); 
    phoenix->SetColliderSize(30.f);
    phoenix->SetPos(ownerPos);

    Vec2 dir = targetPos - ownerPos;
    dir.Normalize();
    phoenix->SetDir(dir);
    float phoenixSpeed = 400.f;
    phoenix->SetSpeed(phoenixSpeed);

    for (int i = 0; i < trailCount; ++i)
    {
        float delay = i * 0.08f;

        m_owner->Coroutine([=]()
            {
                if (phoenix->IsActive() == false) return;
                // (시간 * 속도 = 거리)
                Vec2 spawnPos = ownerPos + (dir * phoenixSpeed * delay);
                for (int j = 0; j < 2; j++)
                {
                    spawnPos += Vec2(rand() % 50 - 25, rand() % 50 - 25);
                    CreateEmber(spawnPos);
                }
            }, delay);
    }


    int circleCount = 24;
    float angleStep = 360.f / circleCount;
    for (int i = 0; i < circleCount; ++i)
    {
        auto* proj = PoolManager::GetInst()->Pop<EnemyProjectile>(PoolType::EnemyProjectile);
        proj->SetTexture(m_emberTex);
        proj->SetSize({ 15.f, 15.f });
        proj->SetColliderSize(5.f);
        proj->SetPos(ownerPos);
        proj->SetDir(angleStep * i);
        proj->SetSpeed(200.f);
    }
}

void SecondMagicPattern::CreateEmber(Vec2 pos)
{
    if (this == nullptr) return;
    auto* ember = PoolManager::GetInst()->Pop<EnemyProjectile>(PoolType::EnemyProjectile);
    ember->SetTexture(m_emberTex);
    ember->SetSize({ 20.f, 20.f });
    ember->SetColliderSize(8.f);
    ember->SetPos(pos);

    ember->SetDir((float)(rand() % 360));

    ember->SetSpeed(0.f);
    m_embers.push_back(ember);
}

void SecondMagicPattern::FinalBurst()
{
    GET_SINGLE(ResourceManager)->Play(L"FireSound");

    for (auto* ember : m_embers)
    {
        float speed = 100.f + (rand() % 50);
        ember->SetSpeed(speed);

    }
    m_embers.clear();

    Vec2 bossPos = m_owner->GetPos();

    for (int wave = 0; wave < 5; ++wave)
    {
        float delay = wave * 0.15f;

        m_owner->Coroutine([=]()
            {
                int count = 30;
                float step = 360.f / count;
                float startAngle = (wave % 2 == 0) ? 0.f : (step / 2.f);

                for (int i = 0; i < count; ++i)
                {
                    auto* p = PoolManager::GetInst()->Pop<EnemyProjectile>(PoolType::EnemyProjectile);
                    p->SetTexture(m_emberTex);
                    p->SetPos(bossPos);
                    p->SetDir(startAngle + step * i);
                    p->SetSpeed(250.f);
                    p->SetSize({ 15.f, 15.f });
                    p->SetColliderSize(5.f);
                }

            }, delay);
    }
}