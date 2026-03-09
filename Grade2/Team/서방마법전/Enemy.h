#pragma once
#include "Object.h"
#include "IDamageable.h"
class Texture;
class Enemy :
    public Object
    , public IDamageable
{
public:
    Enemy();
    virtual ~Enemy();

public:
    // Object을(를) 통해 상속됨
    virtual void Update() override;
    virtual void Render(HDC _hdc) override;
    virtual void EnterCollision(Collider* _other)override;
    void StayCollision(Collider* _other) override;
    void ExitCollision(Collider* _other) override;

    void SetTexture(Texture* _newTex)
    {
        m_pTex = _newTex;
    }

    // IDamageable을(를) 통해 상속됨
    virtual void TakeDamage(int _damage) override;
    virtual void HPZero() override;

private:
    Texture* m_pTex;
};

