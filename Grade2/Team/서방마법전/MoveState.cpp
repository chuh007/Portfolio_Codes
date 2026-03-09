#include "pch.h"
#include "MoveState.h"
#include "StateMachine.h"
#include "Player.h"
#include "IdleState.h"
#include "PlayerAttackState.h"
#include "InputManager.h"
#include "Projectile.h"
#include  "TimeManager.h"

void PlayerMoveState::Enter(StateMachine* fsm) {
	assert(fsm != nullptr && 
		"PlayerMoveState::Enter StateMachine is nullptr");

	m_player = static_cast<Player*>(fsm->GetOwner());
	// 오브젝트 풀링
	assert(m_player != nullptr, 
		"PlayerMoveState::Enter Owner Obejct is nullptr");

	// 애니메이션 재생해준다
}

void PlayerMoveState::Excute(StateMachine* fsm) {
	float _fDT = GET_SINGLE(TimeManager)->GetDT();
	const float BASE_SPEED = 200.f;
	float curSpeed = BASE_SPEED;

	m_player->TryContinueFire(_fDT);

	Vec2 dir = {};
	// shift 누르면 속도 반토막
	if (GET_KEY(KEY_TYPE::W)) dir.y -= 1.f;
	if (GET_KEY(KEY_TYPE::S)) dir.y += 1.f;
	if (GET_KEY(KEY_TYPE::A)) dir.x -= 1.f;
	if (GET_KEY(KEY_TYPE::D)) dir.x += 1.f;

	if (GET_KEY(KEY_TYPE::LSHIFT)) curSpeed *= 0.5f;

	dir.Normalize();
	m_player->RequestTranslate({ dir.x * _fDT * curSpeed, dir.y * _fDT * curSpeed});

	Vec2 pos = m_player->GetPos();
	Vec2 size = m_player->GetSize();

	float halfWidth = size.x / 2.f;
	float halfHeight = size.y / 2.f;

	pos.x = std::max(pos.x, halfWidth - 30.f);
	pos.x = std::min(pos.x, GAME_WIDTH + 30.f - halfWidth);
	pos.y = std::max(pos.y, 0.f + halfHeight);
	pos.y = std::min(pos.y, GAME_HEIGHT + 30.f - halfHeight);

	m_player->SetPos(pos);

	bool isMoving = (dir.x != 0.f || dir.y != 0.f);
	// 계속 Space키를 누르고 있는 상태인가?

	if (!isMoving)
	{
		fsm->ChangeState(PlayerIdleState::GetInstance());
		return;
	}
}

void PlayerMoveState::Exit(StateMachine* fsm) {
	assert(fsm != nullptr && 
		"PlayerMoveState::Exit StateMachine is nullptr");
}
