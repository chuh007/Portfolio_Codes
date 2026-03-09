#pragma once
#include "Pattern.h"
#include <vector>

class EnemyProjectile;

class SecondMagicPattern :
    public Pattern
{
public:
    SecondMagicPattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name);
    ~SecondMagicPattern();

public:
    // Pattern을(를) 통해 상속됨
    void Update() override;
    void BaseShoot() override;

private:
    void FinalBurst();
    void CreateEmber(Vec2 pos);

private:
    int m_shotCount;
    bool m_isFinishedMovement;
    bool m_isBurstFired;
    int trailCount;

    vector<EnemyProjectile*> m_embers;

    Texture* m_phoenixTex;
    Texture* m_emberTex;
};