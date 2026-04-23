#include "pch.h"
#include "BombItem.h"
#include "PlayerManager.h"
#include "Texture.h"
#include "ResourceManager.h"
BombItem::BombItem()
{
	m_Tex = GET_SINGLE(ResourceManager)->GetTexture(L"BombIcon");
}
BombItem::~BombItem()
{
Item:: ~Item();
}
void BombItem::OnCollect()
{
	Player* p = GET_SINGLE(PlayerManager)->GetPlayer();
	p->PlusBombCount(1);
}

void BombItem::Render(HDC _hdc)
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
