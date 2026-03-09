#pragma once
#include "Enemy.h"
class Texture;
class ShotToPlayerEnemy :
    public Enemy
{
public:
    ShotToPlayerEnemy();
    void Update() override;

public:
    void SetFireTime(float _time, float _fireRate, int _shotCount, bool _fireOnce);

private:
    void TryToShot();

private:
    bool isFire = false;
    bool fireOnce;
    float m_currentTime = 0;
    float fireRate = 200.0f;
    float m_fireInterval = 20000.f;
    float tempSpeed = 0.f;
    Vec2 targetPos;
    int shotCount = 0;
    int m_curShot = 0;
    Texture* m_pBulletTexture;
};

