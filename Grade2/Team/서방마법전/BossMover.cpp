#include "pch.h"
#include "BossMover.h"
#include "Object.h"

BossMover::BossMover()
	: m_movePos(0,0)
	, m_startPos(0,0)
	, m_timer(0.f)
	, m_speed(50.f)
	, m_dir({0.f,0.f})
	, m_sec(0.f)
	, m_state(MoveState::Stop)
{
}

BossMover::~BossMover()
{
	Component::~Component();
}

void BossMover::Init()
{

}

void BossMover::LateUpdate()
{
	if (m_sec == 0.f || m_state == MoveState::Stop) return;
	m_timer += fDT;
	switch (m_state)
	{
	case MoveState::MoveTo:
	{
float t = m_timer / m_sec;
		if (t >= 1) Stop();
		else GetOwner()->SetPos(m_startPos + (m_movePos - m_startPos) * t);		
		break;
	}
	case MoveState::MoveDir:
	{
		if (m_timer >= m_sec) Stop();
		else
		{
			Vec2 curPos = GetOwner()->GetPos();
			Vec2 nextPos = curPos + m_dir * m_speed * fDT;
			GetOwner()->SetPos(nextPos);
		}
		break;
	}
	default:
		break;
	}
}

void BossMover::Render(HDC hDC)
{
}

void BossMover::MoveTo(Vec2 _pos, float _sec)
{
	m_timer = 0;
	m_startPos = GetOwner()->GetPos();
	m_movePos = _pos;
	m_sec = _sec;
	m_state = MoveState::MoveTo;
}

void BossMover::MoveDir(Vec2 _dir, float _sec)
{
	m_timer = 0;
	m_dir = _dir;
	m_sec = _sec;
	m_state = MoveState::MoveDir;
}

void BossMover::Stop()
{
	m_timer = 0;
	m_sec = 0.f;
	m_dir = { 0.f,0.f };
	m_movePos = GetOwner()->GetPos();
	m_state = MoveState::Stop;
}