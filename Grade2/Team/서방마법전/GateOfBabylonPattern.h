#pragma once
#include "Pattern.h"

class GateOfBabylonPattern :
    public Pattern
{
public:
    GateOfBabylonPattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name);
    ~GateOfBabylonPattern();

public:
    // Pattern을(를) 통해 상속됨
    void Update() override;
    void BaseShoot() override;

    void CircleFire();

private:
    int m_basePortalCount; // 기본 사출 수
    int m_trailCount; // 쏠때 몇발 쏠지
    float m_baseSpeed; // 속도
    float m_timer;
    float m_rotate;

    Texture* m_swordTexs[3];
    Texture* m_bulletTex;
};