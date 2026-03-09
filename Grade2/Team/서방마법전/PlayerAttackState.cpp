#include "pch.h"
#include "PlayerAttackState.h"
#include "StateMachine.h"
#include "Player.h"
#include "IdleState.h"
#include "MoveState.h"
#include "InputManager.h"

void PlayerAttackState::Enter(StateMachine* fsm) {
	m_player = static_cast<Player*>(fsm->GetOwner());

	// 공격 판정 처리
	//m_player->CreateProjectile();
	m_attackTimer = 0.f;
}

void PlayerAttackState::Excute(StateMachine* fsm) {
	m_player->TryContinueFire(fDT);

	if (m_player->IsMovingInputProcessed()) {
		fsm->ChangeState(PlayerMoveState::GetInstance());
		return;
	}

	if (!GET_KEY(KEY_TYPE::SPACE)) {
		m_attackTimer += fDT;

		if (m_attackTimer >= 0.1f) {
			fsm->ChangeState(new PlayerIdleState());
			return;
		}
	}
	else {
		m_attackTimer = 0.f;
	}
}

void PlayerAttackState::Exit(StateMachine* fsm) {
	// 공격 종료
}
