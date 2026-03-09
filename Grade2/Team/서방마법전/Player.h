#pragma once
#include "Object.h"
#include "StateMachine.h"
#include "Projectile.h"
#include "IDamageable.h"
#include "DeleteBullet.h"

class Texture;
class Health;
class Player :
    public Object, public IDamageable
{
public:
	Player();
	~Player();
public:
	void Update() override;
	void Render(HDC _hdc) override;
	virtual void EnterCollision(Collider* _other) override;
	virtual void StayCollision(Collider* _other) override;
	virtual void ExitCollision(Collider* _other) override;
public:
	void RequestTranslate(Vec2 _delta) {
		Translate(_delta);
	}
public:
	StateMachine* GetStateMachine() const;
	bool IsMovingInputProcessed() const;
	void CreateProjectile();
	void TryContinueFire(float _fDT);
	bool UseBomb();
	void InvokeBomb();
	void GainPower(float _amount);
	void SpawnPower();
public:
	virtual void TakeDamage(int _damage) override;
	virtual void HPZero() override;
public:
	int GetLifeCount() {
		return m_life;
	}
	void SetLifeCount(int _life) {
		m_life = _life;
	}
	void PlusLifeCount(int _life) {
		m_life += _life;
	}
	int GetBombCount() {
		return m_bombCnt;
	}
	int GetPowerLevel() {
		return m_powerLevel;
	}
	void SetBombCount(int _bombCnt) {
		m_bombCnt = _bombCnt;
	}
	void PlusBombCount(int _bombCnt) {
		m_bombCnt += _bombCnt;
	}
	void RequestGameOver() {
		requestGameOver = true;
	}
public:
	void SetInvincible(bool isInvincible);
	float& GetInvincibleTime();
	float GetMaxInvincibleTime() const;
private:
	Texture* m_pTex;
	StateMachine* fsm;
private:
	Health* m_health;
	Collider* col;
	bool m_isDead;
	int m_life;
	const int MAX_HP = 3;
private:
	PlayerProjectile* m_proj;
	float m_projCooldown;
	const float PROJECTILE_INTERVAL = 0.15f;
private:
	int m_bombCnt;
	float m_bombDurationTimer;
	float m_invokeBombTimer = 0.f;
	bool m_isInvokedBomb = false;
	const int MAX_BOMB_COUNT = 3;
	const float BOMB_DURATION = 2.f;
	const float INVOKE_BOMB_DURATION = 1.0f;
private:
	int m_powerLevel;
	float m_amountDmg;
	const int MAX_POWER = 128;
	const int POWER_SMALL_VALUE = 1;
	const int POWER_BIG_VALUE = 8;
private:
	bool m_isInvincible;
	float m_invincibleTime;
	const float MAX_INVINCIBLE_TIME = 2.f;
private:
	bool requestGameOver = false;
	const float HITBOX_RADIUS = 3.0f;
private:
	bool m_isGameOverRequest = false;
	float m_gameOverDelayTimer = 0.f; 
	const float GAME_OVER_TIME = 1.0f;
};

