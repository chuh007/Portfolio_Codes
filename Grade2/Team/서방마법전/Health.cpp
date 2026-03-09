#include "pch.h"
#include "Health.h"
#include "Object.h"
#include "IDamageable.h"
#include "SceneManager.h"
Health::Health()
	: m_maxHp(100)
	, m_currentHp(100)
	, m_isDead(false)
{
}

Health::~Health()
{
	Component::~Component();
}

void Health::Init()
{
}

void Health::LateUpdate()
{
}

void Health::Render(HDC hDC)
{
}

void Health::Dead()
{
	m_isDead = true;
	Object* owner = GetOwner();
	IDamageable* damageable = dynamic_cast<IDamageable*>(owner);
	damageable->HPZero();
}
	
