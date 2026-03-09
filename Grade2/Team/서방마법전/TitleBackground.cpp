#include "pch.h"
#include "TitleBackground.h"
#include "ResourceManager.h"
#include "Texture.h"

TitleBackground::TitleBackground()
{
	m_pTex = GET_SINGLE(ResourceManager)->GetTexture(L"TitleBackground");
}

void TitleBackground::Render(HDC _hdc)
{
    Vec2 pos = GetPos();
    Vec2 size = GetSize();
    int width = m_pTex->GetWidth();
    int height = m_pTex->GetHeight();
    ::StretchBlt(_hdc
        , (pos.x - size.x / 2)
        , (pos.y - size.y / 2)
        , size.x, size.y
        , m_pTex->GetTextureDC()
        , 0, 0, width, height
        , SRCCOPY);
}
