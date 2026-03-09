#pragma once
#include "State.h"
class Player;
class PlayerAttackState : public State
{
public:
	PlayerAttackState() = default;
	virtual ~PlayerAttackState() = default;
public:
	virtual void Enter(StateMachine* fsm) override;
	virtual void Excute(StateMachine* fsm) override;
	virtual void Exit(StateMachine* fsm) override;
public:
	static PlayerAttackState* GetInstance() {
		static PlayerAttackState instance;
		return &instance;
	}
private:
	Player* m_player = nullptr;
	//전체 공격 상태를 유지할 시간
	float m_attackTimer = 0.f;
	// 투사체 발사 간격을 제어하는 타이머
	float m_projectileCooltime = 0.f;
	// 공격 애니메이션이 끝나는 시간
	const float PROJECTILE_INTERVAL = 0.15f;
};

