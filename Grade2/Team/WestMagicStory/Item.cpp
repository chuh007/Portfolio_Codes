#include "pch.h"
#include "Item.h"
#include "Collider.h"
#include "SceneManager.h"

Item::Item()
{
	auto* coll = AddComponent<Collider>();
	coll->SetSize(50.f);
	m_currentSpeed = -150;
}

Item::~Item()
{
	Object::~Object();
}

void Item::Update()
{
	Vec2 curPos = GetPos();
	SetPos({ curPos.x, curPos.y + m_currentSpeed * fDT });
	if(m_currentSpeed<= 200.f)
		m_currentSpeed += 300.f * fDT;

	if (curPos.y >= GAME_HEIGHT * 2)
	{
		GET_SINGLE(SceneManager)->RequestDestroy(this);
	}
}

void Item::EnterCollision(Collider* _other)
{
	OnCollect();
	GET_SINGLE(SceneManager)->RequestDestroy(this);
}

void Item::Render(HDC _hdc)
{
	//Vec2 pos = GetPos();
	//Vec2 size = GetSize();

	//ELLIPSE_RENDER(_hdc, pos.x, pos.y, size.x, size.y);
}
