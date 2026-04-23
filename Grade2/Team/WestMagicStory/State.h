#pragma once
#include "StateMachine.h"
class StateMachine;
class Object;
class State
{
public:
	State() = default;
	virtual ~State() = default;
public:
	virtual void Enter(StateMachine* fsm) abstract; // 상태 진입
	virtual void Excute(StateMachine* fsm) abstract; // 매 프레임마다 호출
	virtual void Exit(StateMachine* fsm) abstract; // 상태 이탈 시 호출
};

