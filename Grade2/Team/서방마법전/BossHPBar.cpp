#include "pch.h"
#include "BossHPBar.h"
#include "Health.h"

BossHPBar::BossHPBar()
	: m_boss(nullptr)
{
}

BossHPBar::~BossHPBar()
{
	Object::~Object();
}

void BossHPBar::Render(HDC _hdc)
{
	Health* health = m_boss->GetComponent<Health>();
	if (health == nullptr) return;
	float value = (float)health->GetHP() /
		(float)health->GetMaxHP();
	float cur = GetSize().x * value;
	GDISelector brush(_hdc, BrushType::RED);
	RECT_RENDER(_hdc, GetPos().x - (GetSize().x / 2.0f) + (cur / 2.0f), GetPos().y,
		cur, 5);
	for (int i = 0; i < m_boss->GetLifeCount() - 1; ++i)
	{
		ELLIPSE_RENDER(_hdc, 20 + i * 25, 75, 25, 25);
	}
	SetTextColor(_hdc, RGB(255, 0, 0));
	int oldMode = SetBkMode(_hdc, TRANSPARENT);
	TextOut(_hdc, 10, 35, L"Kischur Zelretch Schweinorg", 27);
}
