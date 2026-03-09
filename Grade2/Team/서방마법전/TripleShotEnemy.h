#pragma once
#include "Enemy.h"
#include "MathHelper.h"
class Texture;
class TripleShotEnemy :
    public Enemy
{
public:
    TripleShotEnemy();
    ~TripleShotEnemy();
    void Update() override;
protected:

private:
    BezierPathData* pathData;
    float fireTime;
    Texture* m_pTextrue;
};

