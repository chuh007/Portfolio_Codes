#pragma once
#include "Object.h"
class Texture;
class BoomEffect :
    public Object
{
public:
    BoomEffect();
    ~BoomEffect();

public:
    // Object을(를) 통해 상속됨
    void Render(HDC _hdc) override;
    virtual void Update() override;

public:
    void DoScale(float endValue, float duration);

private:
    Texture* m_pTex;

    float m_duration;
    float m_timer;
    float m_endValue;
    Vec2 m_baseScale;

    bool isTweening;
};

