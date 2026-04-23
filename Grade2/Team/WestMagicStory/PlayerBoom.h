#pragma once
#include "Object.h"
class Texture;
class PlayerBoom :
    public Object
{
public:
    PlayerBoom();
    ~PlayerBoom();
public:
    // Object을(를) 통해 상속됨
    void Render(HDC _hdc) override;
    void Update() override;

    virtual void EnterCollision(Collider* _other)override;

private:
    Texture* m_pTex;
};

