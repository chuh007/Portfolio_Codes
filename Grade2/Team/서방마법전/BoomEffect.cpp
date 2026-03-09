#include "pch.h"
#include "BoomEffect.h"
#include "ResourceManager.h"
#include "Texture.h"
#include "SceneManager.h"

BoomEffect::BoomEffect()
	: m_duration(0)
	, m_timer(0)
	, m_endValue(0)
	, m_baseScale(0,0)
	, isTweening(false)
{
	m_pTex = GET_SINGLE(ResourceManager)->GetTexture(L"BossBoom");
}

BoomEffect::~BoomEffect()
{
}

void BoomEffect::Render(HDC _hdc)
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

void BoomEffect::Update()
{
	Object::Update();
	if (!isTweening) return;
	m_timer += fDT;
	float t = m_timer / m_duration;
	if (t >= 1.f)
	{
		SetSize(m_baseScale * m_endValue);
		isTweening = false;
		GET_SINGLE(SceneManager)->RequestDestroy(this);
	}
	SetSize(m_baseScale * m_endValue * t);
}

void BoomEffect::DoScale(float endValue, float duration)
{
	m_baseScale = GetSize();
	m_endValue = endValue;
	m_duration = duration;
	m_timer = 0;
	isTweening = true;
}
