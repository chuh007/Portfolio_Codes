#include "pch.h"
#include "SpellNameText.h"

SpellNameText::SpellNameText()
	: m_movePos(0, 0)
	, m_startPos(0, 0)
	, m_timer(0.f)
	, m_sec(0.f)
	, m_isStop(true)
{
}

SpellNameText::~SpellNameText()
{
	Object::~Object();
}

void SpellNameText::Render(HDC _hdc)
{
	if (m_name.empty()) return;
	GDISelector font(_hdc, FontType::SKILLTEXT);
	SetTextColor(_hdc, RGB(255, 255, 255));
	SetBkColor(_hdc, RGB(0, 0, 255));
	int oldMode = SetBkMode(_hdc, OPAQUE);
	TextOut(_hdc, GetPos().x, GetPos().y, m_name.c_str(), m_name.size());
	SetBkMode(_hdc, oldMode);
}

void SpellNameText::Update()
{
	Object::Update();
	if (m_isStop) return;
	m_timer += fDT;
	float t = m_timer / m_sec;
	if (t >= 1) Stop();
	else SetPos(m_startPos + (m_movePos - m_startPos) * t);
}

void SpellNameText::MoveTo(Vec2 _pos, float _sec)
{
	m_timer = 0;
	m_startPos = GetPos();
	m_movePos = _pos;
	m_sec = _sec;
	m_isStop = false;
}

void SpellNameText::Stop()
{
	m_timer = 0;
	m_sec = 0.f;
	m_isStop = true;
	m_movePos = GetPos();
}
