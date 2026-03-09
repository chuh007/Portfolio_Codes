#pragma once
#include "Pattern.h"

class IcicleFallPattern :
    public Pattern
{
public:
    IcicleFallPattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name);
    ~IcicleFallPattern();
    
public:
    // Pattern을(를) 통해 상속됨
    void Update() override;
    void BaseShoot() override;

private:
    void CircularShoot();

private:
    PoolType m_projectileType;
    int m_fireCount; // 총 몇번 쏠거임
    int m_curCnt; // 지금 몇번 쏨

    float m_iceAngle; // 얼음쏘기 각도
    int m_iceShootCount; // 한번에 몇발
    int m_iceSpeed; // 얼음 속도

    float m_circularAngle; // 부채꼴 각
    int circularShootCount; // 한번에 몇발
    int circularSpeed; // 속도

    Texture* m_iceTex;
    Texture* m_bulletTex;
};

