#pragma once
#include "Object.h"
#include "Boss.h"
class BossHPBar :
    public Object
{
public:
    BossHPBar();
    ~BossHPBar();

public:
    // Object을(를) 통해 상속됨
    void Render(HDC _hdc) override;

public:
    void SetBoss(Boss* _health)
    {
        m_boss = _health;
    }

private:
    Boss* m_boss;
};

