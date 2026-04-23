#include "pch.h"
#include "Button.h"
#include "InputManager.h"
#include "Texture.h"

Button::Button()
	:m_pTex(nullptr)
{
}

void Button::Render(HDC _hdc)
{
	if (m_pTex == NULL) return;
	Vec2 pos = GetPos();
	Vec2 size = GetSize();
	int texX = m_pTex->GetWidth();
	int texY = m_pTex->GetHeight();
	::TransparentBlt(
		_hdc,
		pos.x - size.x / 2,
		pos.y - size.y / 2,
		size.x, size.y,
		m_pTex->GetTextureDC(),
		0, 0, texX, texY,
		RGB(255, 0, 255));

	ComponentRender(_hdc);
}

void Button::Update()
{
	Vec2 pos = GetPos();
	Vec2 size = GetSize();
	RECT r = RECT_MAKE(pos.x, pos.y, size.x, size.y);
	Vec2 mousePos = GET_MOUSEPOS;

	if (GET_KEYDOWN(KEY_TYPE::LBUTTON)
		&& mousePos.x >= r.left 
		&& mousePos.x <= r.right 
		&& mousePos.y <= r.bottom 
		&& mousePos.y >= r.top)
	{
		OnClick();
	}
}

void Button::OnClick()
{
	cout << "Clicked" << endl;
}

void Button::SetText(const wstring& str)
{
	m_text = str;
}
