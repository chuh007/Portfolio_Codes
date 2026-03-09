#include "GameObject.h"
#include "ObjectManager.h"
#include "Console.h"
#include "MapManager.h"

GameObject::GameObject(Position _pos)
	: m_pos(_pos)
	, m_beforepos({0,0})
	, isDead(false)
{
	ObjectManager::GetInst()->AddObject(this);
}

GameObject::~GameObject()
{
}

void GameObject::Update()
{
	m_beforepos = m_pos;
}

void GameObject::Render()
{
	if (MapManager::GetInst()
		->CanRanderThisPos({m_pos.x, m_pos.y}))
	{
		Gotoxy(m_pos.x, m_pos.y);
		cout << m_renderIcon;
	}
}

void GameObject::OnDestroy()
{
	Gotoxy(m_pos.x, m_pos.y);
	cout << "  ";
}
