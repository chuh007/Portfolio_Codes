#include "pch.h"
#include "Enemy.h"
#include "Collider.h"
#include "SceneManager.h"
#include "Rigidbody.h"
#include "Health.h"
#include "Texture.h"
#include "ItemDropCompo.h"
Enemy::Enemy()
	: m_pTex(nullptr)
{
	AddComponent<Collider>();
	auto* health = AddComponent<Health>();
	health->SetMaxHP(100);
	health->SetCurrentHP(100);
}
Enemy::~Enemy()
{
	
}
void Enemy::Update()
{
}

void Enemy::Render(HDC _hdc)
{
	Vec2 pos = GetPos();
	Vec2 size = GetSize();

	if (m_pTex == NULL) return;

	int texX = m_pTex->GetWidth();
	int texY = m_pTex->GetHeight();
	::TransparentBlt(
		_hdc,
		pos.x - size.x/2,
		pos.y - size.y/2,
		size.x, size.y,
		m_pTex->GetTextureDC(),
		0, 0, texX, texY,
		RGB(255, 0, 255));
	ComponentRender(_hdc);
}

void Enemy::EnterCollision(Collider* _other)
{
	cout << "Enter" << endl;
}

void Enemy::StayCollision(Collider* _other)
{
	cout << "Stay" << endl;
}

void Enemy::ExitCollision(Collider* _other)
{
	cout << "Exit" << endl;
}

void Enemy::TakeDamage(int _damage)
{
	Health* health = GetComponent<Health>();
	health->TakeDamage(_damage);
	cout << _damage << endl;
}

void Enemy::HPZero()
{
	cout << "dho";
	GET_SINGLE(SceneManager)->RequestDestroy(this);
	ItemDropCompo* dropCompo = GetComponent<ItemDropCompo>();
	if (dropCompo != nullptr)
		dropCompo->SpawnItem();
}
