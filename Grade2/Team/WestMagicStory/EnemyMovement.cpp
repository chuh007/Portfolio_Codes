#include "pch.h"
#include "Object.h"
#include "SceneManager.h"
#include "EnemyMovement.h"

EnemyMovement::EnemyMovement()
{
    
}

EnemyMovement::~EnemyMovement()
{
    Component::~Component();
    while (moveOrder.empty() == false)
    {
        moveOrder.pop();
    }
}

void EnemyMovement::Init()
{
    m_distanceTraveled = 0.0f;
    m_repeatType = MoveRepeatType::PingPong;
    defaultPos = GetOwner()->GetPos();
    m_pingPongDirection = 1;
}

void EnemyMovement::LateUpdate()
{
    if (moveOrder.front()->totalLength <= 0.0f)
    {
        return;
    }

    m_distanceTraveled += m_speed * fDT * m_pingPongDirection;

    switch (m_repeatType)
    {
    case MoveRepeatType::Stop:
        if (m_distanceTraveled >= moveOrder.front()->totalLength)
        {
            if (moveOrder.size()<=1)
            {
                m_distanceTraveled = moveOrder.front()->totalLength;
                GET_SINGLE(SceneManager)->RequestDestroy(GetOwner());
            }
            else
            {
                m_distanceTraveled = 0.0f;
                defaultPos += GetBezierPoint(moveOrder.front()->points, 1);
                moveOrder.pop();
            }
        }
        break;

    case MoveRepeatType::Loop:
        if (m_distanceTraveled >= moveOrder.front()->totalLength)
        {
            m_distanceTraveled = fmod(m_distanceTraveled, moveOrder.front()->totalLength);
        }
        break;

    case MoveRepeatType::PingPong:
        if (m_pingPongDirection > 0 && m_distanceTraveled >= moveOrder.front()->totalLength)
        {
            m_pingPongDirection = -1;
            m_distanceTraveled = moveOrder.front()->totalLength;
        }
        else if (m_pingPongDirection < 0 && m_distanceTraveled <= 0.0f)
        {
            m_pingPongDirection = 1;
            m_distanceTraveled = 0.0f;
        }

        break;
    case MoveRepeatType::Repeat:
        if (m_distanceTraveled >= moveOrder.front()->totalLength)
        {
            if (moveOrder.size() <= 1)
            {
                m_distanceTraveled = moveOrder.front()->totalLength;
            }
            else
            {
                m_distanceTraveled = 0.0f;
                defaultPos += GetBezierPoint(moveOrder.front()->points, 1);
                moveOrder.push(moveOrder.front());
                moveOrder.pop();
            }
        }
        break;
    }

    double target_t = moveOrder.front()->FindTValueForDistance(m_distanceTraveled);

    Vec2 newPos = GetBezierPoint(moveOrder.front()->points, target_t) + defaultPos;

    if (GetOwner())
    {
        GetOwner()->SetPos(newPos);
    }
}

void EnemyMovement::SetSpeed(float _speed)
{
    m_speed = _speed;
}

void EnemyMovement::AddPathData(BezierPathData* path)
{
    moveOrder.push(path);
    m_distanceTraveled = 0.0f;
}

void EnemyMovement::Render(HDC hDC)
{
    //const std::vector<Vec2>& points = moveOrder.front()->points;

    //if (points.size() < 2)
    //{
    //    return;
    //}

    //const int segments = 100;

    //Vec2 first_v = GetBezierPoint(points, 0.0) + defaultPos;

    //LONG first_x = static_cast<LONG>(std::roundf(first_v.x));
    //LONG first_y = static_cast<LONG>(std::roundf(first_v.y));

    //MoveToEx(hDC, first_x, first_y, NULL);

    //for (int i = 1; i <= segments; ++i)
    //{
    //    double t = (double)i / (double)segments;

    //    Vec2 v = GetBezierPoint(points, t) + defaultPos;

    //    LONG current_x = static_cast<LONG>(std::roundf(v.x));
    //    LONG current_y = static_cast<LONG>(std::roundf(v.y));

    //    LineTo(hDC, current_x, current_y);
    //}
}