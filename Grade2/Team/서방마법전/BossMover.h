#pragma once
#include "Component.h"
#include "Vec2.h"
#include "Enums.h"

class BossMover
	: public Component
{
public:
	BossMover();
	~BossMover();
public:
	// Component을(를) 통해 상속됨
	void Init() override;
	void LateUpdate() override;
	void Render(HDC hDC) override;
public:
	void MoveTo(Vec2 _pos, float _sec); // 몇 초에 걸처서 어디로 이동하는가?
	void MoveDir(Vec2 _dir, float _sec); // 몇 초동안 어느 방향으로 이동하는가?
	void Stop();
public:
	void SetSpeend(float _speed) { m_speed = _speed; }
private:
	Vec2 m_movePos;
	Vec2 m_startPos;
	float m_timer;

	float m_speed;
	Vec2 m_dir;

	float m_sec;
	MoveState m_state;
};

