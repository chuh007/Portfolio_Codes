#include "pch.h"
#include "Background.h"
#include "Texture.h"
#include "ResourceManager.h"
#include "SceneManager.h"

Background::Background()
    : m_curTime(0)
    , m_isChangeing(false)
    , m_str(L"GameOver")
{
	m_pTex = GET_SINGLE(ResourceManager)->GetTexture(L"Background");
}

Background::~Background()
{
}

void Background::Render(HDC _hdc)
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

void Background::Update()
{
    Object::Update();
    if (!m_isChangeing) return;
    m_curTime += fDT;
    if (m_curTime >= 1.f)
    {
        GET_SINGLE(SceneManager)->LoadScene((L"GameClear"));
    }
}

void Background::ChangeScene()
{
    m_isChangeing = true;
}
