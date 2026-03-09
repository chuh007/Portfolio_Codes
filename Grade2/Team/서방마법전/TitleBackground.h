#pragma once
#include "Object.h"
class Texture;
class TitleBackground :
    public Object
{
public:
    TitleBackground();
public:
    void Render(HDC _hdc) override;

private:
    Texture* m_pTex;
};

