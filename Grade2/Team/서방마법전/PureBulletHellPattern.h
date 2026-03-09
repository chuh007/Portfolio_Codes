#pragma once
#include "Pattern.h"
class PureBulletHellPattern :
    public Pattern
{
public:
    PureBulletHellPattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name);
    ~PureBulletHellPattern();

public:
    void Update() override;
    void BaseShoot() override;

    void SecondFire();
    void ThirdFire();

private:
    PoolType m_projectileType;
    int m_fireCount;
    int m_speed;

    float m_timer;
    float m_secondTimer;
    float m_thirdTimer;

    Texture* m_bulletTex1;
    Texture* m_bulletTex2;
    Texture* m_bulletTex3;
};

