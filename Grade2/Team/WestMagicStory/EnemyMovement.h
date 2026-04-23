#pragma once
#include "Component.h"
#include "MathHelper.h" 
#include <queue>
struct QueueModule
{
    BezierPathData* moveOrder;
    float m_pingPongDirection;
    MoveRepeatType repeatType;
    int repeatTime;
};
class EnemyMovement : public Component
{
private:
    float m_distanceTraveled = 0.0f;
    float m_speed = 0.0f;

public:
    EnemyMovement();
    ~EnemyMovement();
    void Init() override;
    void LateUpdate() override;
    void Render(HDC hDC) override;

    void AddPathData(BezierPathData * path);
    void SetSpeed(float _speed);
    inline void SetDefaultPos(Vec2 pos) { defaultPos = pos; };
    void SetRepeatType(MoveRepeatType type)
    {
        m_repeatType = type;
    }
    float GetSpeed() { return m_speed; };

private:
    Vec2 defaultPos = { 0,0 };
    MoveRepeatType m_repeatType;
    float m_pingPongDirection;
    std::queue<BezierPathData*> moveOrder;
};