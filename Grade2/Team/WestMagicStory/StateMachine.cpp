#include "pch.h"
#include "StateMachine.h"

StateMachine::StateMachine() : m_curState(nullptr) {
	
}

StateMachine::~StateMachine() {
	if (m_curState) {
		m_curState->Exit(this);
		m_curState = nullptr;
	}
}

void StateMachine::Init() {
	// 초기화 시 기본 상태를 정의
	//ChangeState(new PlayerIdleState());
}

void StateMachine::LateUpdate() {
	assert(m_curState != nullptr && 
		"StateMachine::LateUpdate called no with current State. Check Init and ChangeState");

	// 만약 현재 상태가 존재한다면
	if (m_curState) {
		m_curState->Excute(this);
	}
}

void StateMachine::Render(HDC _hdc) {
	
}

void StateMachine::ChangeState(State* _newState) {
	assert(_newState != nullptr && 
		"Can't change State to a nullptr State");

	if (m_curState) {
		m_curState->Exit(this);
	}

	m_curState = _newState;

	if (m_curState)
		m_curState->Enter(this);
}
