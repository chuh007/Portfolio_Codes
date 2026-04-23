#pragma once
#include "Object.h"
class Texture;
class Item :
    public Object
{
public:
    Item();
    virtual ~Item();
    void Update() override;
    void EnterCollision(Collider* _other) override;

protected :
    virtual void OnCollect() abstract;
    // Object을(를) 통해 상속됨
    void Render(HDC _hdc) override;

private:
    float m_currentSpeed = 0;

protected:
    Texture* m_Tex;
};

