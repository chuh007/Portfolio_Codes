#pragma once
#include "State.h"
class Player;
class PlayerMoveState : public State
{
public:
	PlayerMoveState() = default;
	virtual ~PlayerMoveState() = default;
public:
	virtual void Enter(StateMachine* fsm) override;
	virtual void Excute(StateMachine* fsm) override;
	virtual void Exit(StateMachine* fsm) override;
public:
	static PlayerMoveState* GetInstance() {
		static PlayerMoveState instance;
		return &instance;
	}
private:
	Player* m_player = nullptr;
	float m_moveSpeed = 50.f;
};

