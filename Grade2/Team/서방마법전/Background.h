#pragma once
#include "Object.h"

class Texture;
class Background :
    public Object
{
public:
    Background();
    ~Background();
public:
    // Object을(를) 통해 상속됨
    void Render(HDC _hdc) override;
    void Update() override;
public:
    void SetTexture(Texture* _tex) { m_pTex = _tex; }
    void ChangeScene();
private:
    Texture* m_pTex;
    float m_curTime;
    bool m_isChangeing;
    wstring m_str;
};

