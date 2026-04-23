#pragma once
#include "Pattern.h"
#include "PoolManager.h"
class CirclePattern :
    public Pattern
{
public:
    CirclePattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name);
    ~CirclePattern();

public:
    // Pattern을(를) 통해 상속됨
    void Update() override;
    void BaseShoot() override;
public:

private:
    PoolType m_projectileType;
    int m_fireCount;
    int m_speed;

    Texture* m_bulletTex;
};

