#pragma once
#include "Pattern.h"
class MultiSpeedRadialPattern :
    public Pattern
{
public:
    MultiSpeedRadialPattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name);
    ~MultiSpeedRadialPattern();

public:
    // Pattern을(를) 통해 상속됨
    void Update() override;
    void BaseShoot() override;
public:

private:
    PoolType m_projectileType;
    int m_fireCount;
    int m_speed;

    float m_moveTime;

    Texture* m_bulletTex;
};

