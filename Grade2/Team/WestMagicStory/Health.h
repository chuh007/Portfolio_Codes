#pragma once
#include "Component.h"
class Health :
    public Component
{
public:
    Health();
    ~Health();
public:
    // Component을(를) 통해 상속됨
    void Init() override;
    void LateUpdate() override;
    void Render(HDC hDC) override;
public:
    const int& GetHP()
    { return m_currentHp; }
    const int& GetMaxHP()
    { return m_maxHp; }
    void SetCurrentHP(int _value)
    {
        m_currentHp = _value;
    }
    void SetMaxHP(int _value)
    {
        m_maxHp = _value;
    }
    void TakeDamage(int _value)
    {
        m_currentHp -= _value;
        if (m_currentHp <= 0)
        {
            m_currentHp = 0;
            Dead();
        }
    }
public:
    void Dead();
private:
    int m_maxHp;
    int m_currentHp;
    bool m_isDead;
};

