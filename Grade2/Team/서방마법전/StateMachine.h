#pragma once
#include "Component.h"
#include "State.h"
class State;
class StateMachine : public Component
{
public:
	StateMachine();
	virtual ~StateMachine();
public:
	virtual void Init() override;
	virtual void LateUpdate() override;
	virtual void Render(HDC _hdc) override;
public:
	void ChangeState(State* _newState);
private:
	State* m_curState;
};

