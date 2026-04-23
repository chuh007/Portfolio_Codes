#include "pch.h"
#include "ButtonSelector.h"
#include "InputManager.h"
#include "Button.h"
#include "Texture.h"
#include "ResourceManager.h"
ButtonSelector::ButtonSelector()
	:m_pTexture(nullptr)
{
	m_pTexture = GET_SINGLE(ResourceManager)->GetTexture(L"SelectIcon");
	btns = vector<Button*>(0);
	curSelectedIdx = 0;
	m_lastSelectTime = 0;
	m_isActive = true;
}
ButtonSelector::~ButtonSelector()
{
	btns.erase(btns.begin(), btns.end());
}
void ButtonSelector::Render(HDC _hdc)
{
	if (m_isActive)
	{
		if (m_pTexture == NULL) return;
		Vec2 pos = GetPos();
		Vec2 size = GetSize();
		int texX = m_pTexture->GetWidth();
		int texY = m_pTexture->GetHeight();
		::TransparentBlt(
			_hdc,
			pos.x - size.x / 2,
			pos.y - size.y / 2,
			size.x, size.y,
			m_pTexture->GetTextureDC(),
			0, 0, texX, texY,
			RGB(255, 0, 255));
	}
}

void ButtonSelector::Update()
{
	m_lastSelectTime += fDT;
	if (GET_KEYDOWN(KEY_TYPE::S) || GET_KEYDOWN(KEY_TYPE::DOWN))
	{
		curSelectedIdx = (curSelectedIdx + 1) % btns.size();
		MoveToCurrentSelect();
		m_isActive = true;
		m_lastSelectTime = 0;
	}
	else if (GET_KEYDOWN(KEY_TYPE::W) || GET_KEYDOWN(KEY_TYPE::UP))
	{
		curSelectedIdx = (curSelectedIdx - 1) < 0 ? btns.size()-1 : curSelectedIdx - 1;
		MoveToCurrentSelect();
		m_isActive = true;
		m_lastSelectTime = 0;
	}
	else if (GET_KEYDOWN(KEY_TYPE::ENTER) || GET_KEYDOWN(KEY_TYPE::SPACE))
	{
		cout << curSelectedIdx << endl;
		btns[curSelectedIdx]->OnClick();
	}
}

void ButtonSelector::AssignButton(Button* button)
{
	btns.push_back(button);
	curSelectedIdx = 0;
	MoveToCurrentSelect();
}

void ButtonSelector::MoveToCurrentSelect()
{
	float y = btns[curSelectedIdx]->GetPos().y;
	float x = btns[curSelectedIdx]->GetPos().x;
	x -= btns[curSelectedIdx]->GetSize().x / 2;
	x -= 25.f;
	SetPos({x,y});
}
