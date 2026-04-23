#include "pch.h"
#include "ItemDropCompo.h"
#include "Object.h"
#include "Health.h"
#include "Item.h"
#include "SceneManager.h"

ItemDropCompo::~ItemDropCompo()
{
	Component::~Component();
	SAFE_DELETE(m_item)
}

void ItemDropCompo::Init()
{
	m_healthCompo = GetOwner()->GetComponent<Health>();
}

void ItemDropCompo::LateUpdate()
{
	
}

void ItemDropCompo::Render(HDC hDC)
{
}

void ItemDropCompo::SpawnItem()
{
	Vec2 position = GetOwner()->GetPos();
	m_item->SetPos(position);
	GET_SINGLE(SceneManager)->GetCurScene()->AddObject(m_item, Layer::ITEM);
	m_item = nullptr;
}

void ItemDropCompo::SetItem(Item* item)
{
	m_item = item;
}
