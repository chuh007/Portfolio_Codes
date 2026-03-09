#pragma once
#include "Pattern.h"

class SpiralPattern :
    public Pattern
{
public:
    SpiralPattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name);
    ~SpiralPattern();

public:
    // Pattern을(를) 통해 상속됨
    void Update() override;
    void BaseShoot() override;

public:
    Vec2 GetSpiralPos(Vec2 centerPos, float radius, int armCount, int armIndex);

private:
    PoolType m_projectileType;

    int m_armCount; 
    int m_speed;
    int m_fireCount;
    bool m_isleft;

    Texture* m_bulletTex;
};