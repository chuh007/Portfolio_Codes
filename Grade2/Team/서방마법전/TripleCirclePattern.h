#pragma once
#include "Pattern.h"
#include "PoolManager.h"
class TripleCirclePattern :
    public Pattern
{
public:
    TripleCirclePattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name);
    ~TripleCirclePattern();

public:
    // Pattern을(를) 통해 상속됨
    void Update() override;
    void BaseShoot() override;
private:
    PoolType m_projectileType;
    int m_fireCount;
    int m_speed;

    Texture* m_bulletTex;
};

