#pragma once
#include "State.h"
class Player;
class PlayerDeadState : public State
{
public:
	PlayerDeadState() = default;
	virtual ~PlayerDeadState() = default;
public:
	virtual void Enter(StateMachine* _fsm) override;
	virtual void Excute(StateMachine* _fsm) override;
	virtual void Exit(StateMachine* _fsm) override;
public:
	static PlayerDeadState* GetInstance() {
		static PlayerDeadState instance;
		return &instance;
	}
};

