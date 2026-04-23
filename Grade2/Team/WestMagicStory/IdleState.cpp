#include "pch.h"
#include "IdleState.h"
#include "StateMachine.h"
#include "Player.h"
#include "MoveState.h"
#include "PlayerAttackState.h"
#include "InputManager.h"
#include "TimeManager.h"

void PlayerIdleState::Enter(StateMachine* fsm) {
	assert(fsm != nullptr && 
		"PlayerIdleState::Enter called with a nullptr StateMachine.");

	m_player = static_cast<Player*>(fsm->GetOwner());
	assert(m_player != nullptr && 
		"StateMachine's owner object is nullptr. Check SetOwner");

	// Idle 애니메이션 재생
}

void PlayerIdleState::Excute(StateMachine* fsm) {
	float _fDT = GET_SINGLE(TimeManager)->GetDT();
	m_player->TryContinueFire(_fDT);

	if (m_player->IsMovingInputProcessed()) {
		fsm->ChangeState(PlayerMoveState::GetInstance());
		return;
	}

	/*if (GET_KEY(KEY_TYPE::SPACE)) {
		fsm->ChangeState(new PlayerAttackState());
		return;
	}*/
}

void PlayerIdleState::Exit(StateMachine* fsm) {
	assert(fsm != nullptr && 
		"PlayerIdleState::Exit called with a nullptr StateMachine");
}
