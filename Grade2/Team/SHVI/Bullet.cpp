#include "Bullet.h"
#include "Console.h"
#include "EnemyCollisionManager.h"


Bullet::Bullet(const Position& _pos
	, const Dir& _dir
	, const int& _lifeTick
	, const int& _damage)
	: CharacterObject(_pos)
	, m_dir(_dir)
	, m_lifetick(_lifeTick)
	, m_damage(_damage)
{
	m_renderIcon = "*";
}

Bullet::~Bullet()
{
}

void Bullet::Update()
{
	Base::Update();
	Move(m_dir);
	m_lifetick--;
	if (m_lifetick <= 0) isDead = true;
}

void Bullet::Render()
{
	Base::Render();
}

void Bullet::Move(Dir _dir)
{
	switch (_dir)
	{
	case Dir::UP:
		--m_pos.y;
		break;
	case Dir::DOWN:
		++m_pos.y;
		break;
	case Dir::LEFT:
		--m_pos.x;
		break;
	case Dir::RIGHT:
		++m_pos.x;
		break;
	}
	EnemyCollisionManager::GetInst()->CheckCollision(this);
}

int Bullet::GetDamage()
{
	return m_damage;
}
