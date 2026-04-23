#include "pch.h"
#include "PowerItem.h"
#include "PlayerManager.h"
#include "Texture.h"
#include "ResourceManager.h"

PowerItem::PowerItem()
{
	m_Tex = GET_SINGLE(ResourceManager)->GetTexture(L"PowerIcon");
}

PowerItem::~PowerItem()
{
	Item::~Item();
}

void PowerItem::OnCollect()
{
	Player* p = GET_SINGLE(PlayerManager)->GetPlayer();
	p->GainPower(1);
}

void PowerItem::Render(HDC _hdc)
{
	Vec2 pos = GetPos();
	Vec2 size = GetSize();

	LONG width = m_Tex->GetWidth();
	LONG height = m_Tex->GetHeight();

	::TransparentBlt(_hdc
		, (int)(pos.x - size.x / 2.f)
		, (int)(pos.y - size.y / 2.f)
		, size.x
		, size.y
		, m_Tex->GetTextureDC()
		, 0, 0, width, height,
		RGB(255, 0, 255));
}
