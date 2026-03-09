#include "pch.h"
#include "Player.h"
#include "PlayerHitState.h"
#include "IdleState.h"
#include "MoveState.h"
#include "TimeManager.h"

void PlayerHitState::Enter(StateMachine* _fsm) {
	m_player = static_cast<Player*>(_fsm->GetOwner());
	m_player->InvokeBomb();
	m_player->GetInvincibleTime() = 0.f;
	m_player->GainPower(-15);

	std::cout << "Player hit" << std::endl;
}

void PlayerHitState::Excute(StateMachine* _fsm) {
	float _fDT = GET_SINGLE(TimeManager)->GetDT();
	m_player->GetInvincibleTime() += _fDT;

	if (m_player->GetInvincibleTime() >= m_player->GetMaxInvincibleTime()) {
		m_player->SetInvincible(false);
		m_player->GetInvincibleTime() = 0.f;
		_fsm->ChangeState(PlayerIdleState::GetInstance());
		return;
	}
	
	if (m_player->IsMovingInputProcessed()) {
		_fsm->ChangeState(PlayerMoveState::GetInstance());
		return;
	}

	return;
}

void PlayerHitState::Exit(StateMachine* _fsm) {
	Player* player = static_cast<Player*>(_fsm->GetOwner());
	if (player) {
		player->SetInvincible(false);
		player->GetInvincibleTime() = 0.f;
	}
}
