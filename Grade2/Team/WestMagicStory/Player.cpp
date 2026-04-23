#include "pch.h"
#include "Player.h"
#include "InputManager.h"
#include "Projectile.h"
#include "SceneManager.h"
#include "Scene.h"
#include "Texture.h"
#include "ResourceManager.h"
#include "Collider.h"
#include "Animator.h"
#include "Animation.h"
#include "Rigidbody.h"
#include "StateMachine.h"
#include "IdleState.h"
#include "PoolManager.h"
#include "Health.h"
#include "PlayerHitState.h"
#include "PlayerDeadState.h"
#include "PlayerBoom.h"
#include "TimeManager.h"
#include "PowerItem.h"
#include "BoomEffect.h"

Player::Player() : m_isDead(false), m_life(3), m_powerLevel(0), m_isInvincible(false),
m_projCooldown(0.f), m_bombCnt(0), m_invincibleTime(0.f), m_bombDurationTimer(0.f),
m_amountDmg(0.f),
col(nullptr), fsm(nullptr), m_health(nullptr),  m_proj(nullptr)
{
	m_pTex = GET_SINGLE(ResourceManager)->GetTexture(L"Player");
	auto* rb = AddComponent<Rigidbody>();
	rb->SetUseGravity(false);
	col = AddComponent<Collider>();
	col->SetName(L"Player");
	col->SetSize(3.0f);
	/*auto* animator = AddComponent<Animator>();
	animator->CreateAnimation
	(L"JiwooFront",
		m_pTex, 
		{0.f,150.f},
		{50.f,50.f},
		{50.f,0.f},
		5,0.1f
	);
	animator->Play(L"JiwooFront");*/
	
	m_health = AddComponent<Health>();
	m_health->SetMaxHP(3);
	m_health->SetCurrentHP(3);
	SetBombCount(MAX_BOMB_COUNT);

	fsm = AddComponent<StateMachine>();
	assert(fsm != nullptr && "fsm is null in player");
	fsm->ChangeState(PlayerIdleState::GetInstance());
}

Player::~Player()
{

}

void Player::Render(HDC _hdc)
{
	Vec2 pos = GetPos();
	Vec2 size = GetSize();
	LONG width = m_pTex->GetWidth();
	LONG height = m_pTex->GetHeight();

	int render_x = (int)(pos.x - width / 2.f);
	int render_y = (int)(pos.y - height / 2.f);

	::TransparentBlt(
		_hdc, render_x, render_y, width , height,
		m_pTex->GetTextureDC(),
		0, 0, width, height,
		RGB(255, 0, 255));

	ComponentRender(_hdc);

	HPEN hRedPen = CreatePen(PS_SOLID, 2, RGB(255, 255, 255));
	HPEN hOldPen = (HPEN)SelectObject(_hdc, hRedPen);

	HBRUSH hNullBrush = (HBRUSH)GetStockObject(WHITE_BRUSH);
	HBRUSH hOldBrush = (HBRUSH)SelectObject(_hdc, hNullBrush);

	float centerX = pos.x;
	float centerY = pos.y;

	int x1 = (int)(centerX - HITBOX_RADIUS);
	int y1 = (int)(centerY - HITBOX_RADIUS);
	int x2 = (int)(centerX + HITBOX_RADIUS);
	int y2 = (int)(centerY + HITBOX_RADIUS);

	Ellipse(_hdc, x1, y1, x2, y2);

	SelectObject(_hdc, hOldPen);
	DeleteObject(hRedPen);

	SelectObject(_hdc, hOldBrush);
}

void Player::StayCollision(Collider* _other)
{
}

void Player::EnterCollision(Collider* _other)
{

}


void Player::ExitCollision(Collider* _other)
{
}


void Player::Update()
{
	Object::Update();
	float _fDT = GET_SINGLE(TimeManager)->GetDT();

	if (GET_KEYDOWN(KEY_TYPE::Q) && !m_isInvokedBomb) {
		UseBomb();
	}
	//if (GET_KEY(KEY_TYPE::Z)) {
	//	if (m_powerLevel <= MAX_POWER) {
	//		GainPower(1);
	//	}
	//}
	//if (GET_KEYDOWN(KEY_TYPE::R)) {
	//	if (m_powerLevel < MAX_POWER) {
	//		m_powerLevel = MAX_POWER;
	//		GainPower(100);
	//	}
	//}
	Object::LateUpdate();

	if (m_isDead) {
		m_gameOverDelayTimer += _fDT;
		if (m_gameOverDelayTimer >= GAME_OVER_TIME) {
			GET_SINGLE(SceneManager)->LoadScene(L"GameOver");
		}
	}
}

void Player::CreateProjectile()
{
	int num_proj = 1;
	if (m_powerLevel >= MAX_POWER) {
		num_proj = 5;
	}
	else if (m_powerLevel >= 96) {
		num_proj = 4;
	}
	else if (m_powerLevel >= 64) {
		num_proj = 3;
	}
	else if (m_powerLevel >= 32) {
		num_proj = 2;
	}

	float baseDmg = 4.f;
	float totalDmg = baseDmg + m_amountDmg;
	if (totalDmg < 0.f) totalDmg = baseDmg;

	const float POS_OFFSET_Y = GetSize().y / 2.f;
	const float HORIZONTAL_SPREAD = 25.f;
	const float Y_SPREAD_OFFSET = 10.f;

	const std::wstring MIDDLE_BULLET_TEX = L"MiddleBullet";
	const std::wstring SIDE_BULLET_TEX = L"AngleBullet";

	for (int i = 1; i <= num_proj; ++i)
	{
		PlayerProjectile* proj = GET_SINGLE(PoolManager)->
			Pop<PlayerProjectile>(PoolType::PlayerProj);

		proj->Reset();
		proj->SetSize({ 30.f,30.f });

		float damage_multiplier = 1.0f;
		float current_angle_deg = 0.f;
		float pos_x_offset = 0.f;
		float pos_y_offset_extra = 0.f;
		std::wstring textureName = MIDDLE_BULLET_TEX;

		switch (i)
		{
		case 1:
			damage_multiplier = 1.0f;
			pos_x_offset = 0.f;
			current_angle_deg = 0.f;
			break;

		case 2:
			damage_multiplier = 0.75f;
			pos_x_offset = -HORIZONTAL_SPREAD;
			current_angle_deg = -1.5f;
			pos_y_offset_extra = Y_SPREAD_OFFSET;
			break;

		case 3:
			damage_multiplier = 0.75f;
			pos_x_offset = HORIZONTAL_SPREAD;
			current_angle_deg = 1.5f;
			pos_y_offset_extra = Y_SPREAD_OFFSET;
			break;

		case 4:
			damage_multiplier = 0.5f;
			pos_x_offset = -HORIZONTAL_SPREAD * 1.5f;
			current_angle_deg = -20.f;
			textureName = SIDE_BULLET_TEX;
			pos_y_offset_extra = Y_SPREAD_OFFSET + 20;

			break;

		case 5:
			damage_multiplier = 0.5f;
			pos_x_offset = HORIZONTAL_SPREAD * 1.5f;
			current_angle_deg = 20.f;
			textureName = SIDE_BULLET_TEX;
			pos_y_offset_extra = Y_SPREAD_OFFSET + 20;
			break;

		default:
			break;
		}

		Vec2 pos = GetPos();
		pos.y -= POS_OFFSET_Y - pos_y_offset_extra;
		pos.x += pos_x_offset;

		proj->SetTextureByName(textureName);
		proj->SetPos(pos);

		float finalDmg = totalDmg * damage_multiplier;
		proj->SetDamage(finalDmg);

		float current_angle_rad = current_angle_deg * PI / 180.f;
		proj->SetDir({ sinf(current_angle_rad), -cosf(current_angle_rad) });
	}
}

StateMachine* Player::GetStateMachine() const {
	return fsm;
}

bool Player::IsMovingInputProcessed() const {
	return GET_KEY(KEY_TYPE::W) || GET_KEY(KEY_TYPE::A) ||
		GET_KEY(KEY_TYPE::S) || GET_KEY(KEY_TYPE::D);
}

void Player::TakeDamage(int _damage) {
	if (m_isInvincible) return;
	if (m_isDead) return;

	m_life--;
	m_health->TakeDamage(_damage);
	if (m_life > -1) {
		SetInvincible(true);
		auto* effect = GET_SINGLE(SceneManager)->GetCurScene()
			->Spawn<BoomEffect>(Layer::PROJECTILEDELETER, GetPos(), { 25,25 });
		effect->DoScale(10.f, 0.3f);
		m_health->SetCurrentHP(m_health->GetMaxHP());
		SetPos({ GAME_WIDTH / 2.f, 600.f });
		fsm->ChangeState(PlayerHitState::GetInstance());
	}
	else {
		m_isDead = true;
		/*this->Coroutine([=]()
			{
				cout << "1sec end" << endl;
			}, 1.0f);*/
		//SetActive(false);
		m_gameOverDelayTimer = 0.f;
	}
}

void Player::HPZero() {
	m_isDead = true;
}

void Player::SetInvincible(bool isInvincible) {
	m_isInvincible = isInvincible;
}

float& Player::GetInvincibleTime() {
	return m_invincibleTime;
}

float Player::GetMaxInvincibleTime() const {
	return MAX_INVINCIBLE_TIME;
}

void Player::TryContinueFire(float _fDT) {
	if (GET_KEY(KEY_TYPE::SPACE) || GET_KEY(KEY_TYPE::UP)) {
		m_projCooldown += _fDT;
		while (m_projCooldown >= PROJECTILE_INTERVAL) {
			CreateProjectile();
			m_projCooldown -= PROJECTILE_INTERVAL;
		}
	}
	else {
		m_projCooldown = 0.f;
	}
}

bool Player::UseBomb() {
	if (m_bombCnt > 0) {
		m_isInvokedBomb = true;
		m_bombCnt--;

		PlayerBoom* delBullet = GET_SINGLE(SceneManager)->GetCurScene()->
			Spawn<PlayerBoom>(Layer::PROJECTILEDELETER, GetPos(), { 200,200 });

		GET_SINGLE(ResourceManager)->Play(L"Bomb");

		delBullet->Coroutine([=]()
			{
				GET_SINGLE(SceneManager)->RequestDestroy(delBullet);
				m_isInvokedBomb = false;
			}, 2.5f);

		return true;
	}

	return false;
}

void Player::InvokeBomb() {
	DeleteBullet* delBullet = GET_SINGLE(SceneManager)->GetCurScene()->
		Spawn<DeleteBullet>(Layer::PROJECTILEDELETER, GetPos(), GetSize());

	if (delBullet == nullptr) return;

	if (m_bombCnt < 3) {
		m_bombCnt = 3;
	}

	GET_SINGLE(ResourceManager)->Play(L"PlayerHit");

	SpawnPower();

	delBullet->Coroutine([=]()
		{
			GET_SINGLE(SceneManager)->RequestDestroy(delBullet);
		}, 2.f);
}

void Player::GainPower(float _amount) {
	m_powerLevel += _amount;
	m_amountDmg += _amount / 10.f;
	if (m_powerLevel < 0) {
		m_powerLevel = 0;
	}

	m_powerLevel = std::min(m_powerLevel, MAX_POWER);
	m_amountDmg = std::max(0.f, m_amountDmg);
	m_amountDmg = std::min(m_amountDmg, m_powerLevel * 0.1f);
}

void Player::SpawnPower() {
	int itemCnt = 0;

	const float Y_SPAWN_MIN = -20.f;
	const float Y_SPAWN_MAX = 50.f;

	Vec2 itemSize = { 25.f, 25.f };

	if (m_powerLevel < 15) {
		itemCnt = m_powerLevel;
	}
	else {
		itemCnt = 13;
	}

	for (int i = 0; i < itemCnt; ++i) {
		float randX = (float)rand() / RAND_MAX * GAME_WIDTH;
		float randY = (float)rand() / RAND_MAX * (Y_SPAWN_MAX - Y_SPAWN_MIN) + Y_SPAWN_MIN;

		Vec2 spawnPos = { randX, randY };

		PowerItem* power = GET_SINGLE(SceneManager)->GetCurScene()->
			Spawn<PowerItem>(Layer::ITEM, spawnPos, itemSize);
	}
}
