#include "pch.h"
#include "Texture.h"
#include "Effect.h"
#include "ResourceManager.h"

Effect::Effect()
{
	m_pTex = GET_SINGLE(ResourceManager)->GetTexture(L"Magic");
}

Effect::~Effect()
{
}

void Effect::Render(HDC _hdc)
{
	Vec2 pos = GetPos();
	Vec2 size = GetSize();
	LONG width = m_pTex->GetWidth();
	LONG height = m_pTex->GetHeight();
	::TransparentBlt(_hdc
		, (int)(pos.x - size.x / 2)
		, (int)(pos.y - size.y / 2)
		, size.x
		, size.y
		, m_pTex->GetTextureDC()
		, 0, 0, width, height,
		RGB(255, 0, 255));
}

void Effect::Reset()
{
}
