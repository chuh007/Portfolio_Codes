#pragma once
#include "Object.h"
#include "PoolManager.h"
class Texture;
class PlayerProjectile :
    public Object, public IPoolable
{
public:
    PlayerProjectile();
    ~PlayerProjectile();
public:
    // Object을(를) 통해 상속됨
    void Update() override;
    void Render(HDC _hdc) override;
    void EnterCollision(Collider* _other) override;
public:
    virtual void Reset() override;
public:
    // 인라인 함수
    void SetAngle(float _angle)
    {
        m_angle = _angle;
    }
    void SetDir(Vec2 _dir)
    {
        m_dir = _dir;
        m_dir.Normalize();
    }
    void SetSpeed(float _speed)
    {
        m_speed = _speed;
    }
    void SetDamage(float _damage)
    { m_damage = _damage; }
    int getDamage()
    { return m_damage; }
    void SetTextureByName(const wstring& _texName);
private:
    //float m_dir;
    float m_angle;
    float m_speed;
    int m_damage;
    Vec2 m_dir;
    Texture* m_pTex;
};

