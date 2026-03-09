#pragma once
#include "Object.h"
class Texture;
class Button :
    public Object
{
public:
    Button();
    // Object을(를) 통해 상속됨
    void Render(HDC _hdc) override;
    void Update() override;
public:
    virtual void OnClick();
    void SetText(const wstring& str);
    void SetTexture(Texture* _tex) { m_pTex = _tex; };
private :
    wstring m_text;
    Texture* m_pTex;
};

