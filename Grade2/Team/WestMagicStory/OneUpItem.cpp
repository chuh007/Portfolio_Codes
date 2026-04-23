#include "pch.h"
#include "OneUpItem.h"
#include "PlayerManager.h"
#include "Texture.h"
#include "ResourceManager.h"

OneUpItem::OneUpItem()
{
	m_Tex = GET_SINGLE(ResourceManager)->GetTexture(L"LifeIcon");
}

OneUpItem::~OneUpItem()
{
	Item::~Item();
}

void OneUpItem::OnCollect()
{
	GET_SINGLE(PlayerManager)->GetPlayer()->PlusLifeCount(1);
}

void OneUpItem::Render(HDC _hdc)
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
