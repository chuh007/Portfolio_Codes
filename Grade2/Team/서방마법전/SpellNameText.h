#pragma once
#include "Object.h"
class SpellNameText :
    public Object
{
public:
    SpellNameText();
    ~SpellNameText();

public:
    // Object을(를) 통해 상속됨
    void Render(HDC _hdc) override;
    void Update() override;
public:
    void MoveTo(Vec2 _pos, float _sec);
    void Stop();
public:
    wstring GetName() { return m_name; }
    void SetName(wstring _str) { m_name = _str; }
private:
    wstring m_name;

    Vec2 m_movePos;
    Vec2 m_startPos;
    float m_timer;
    float m_sec;
    bool m_isStop;
};

