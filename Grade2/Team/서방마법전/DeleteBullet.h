#pragma once
#include "Object.h"
class DeleteBullet :
    public Object
{
public:
    DeleteBullet();
    ~DeleteBullet();
public:
    // Object을(를) 통해 상속됨
    void Render(HDC _hdc) override;

    virtual void StayCollision(Collider* _other)override;
};

