#include "Player.h"
#include "Console.h"
#include "UpgradeManager.h"

Player::Player(const Position& _pos)
{
	m_renderIcon = "¡×";
	m_pos = _pos;
}

void Player::Update()
{

}

void Player::Render()
{
	if (UpgradeManager::GetInst()->GetBarrier())
	{
		SetColor(COLOR::WHITE,COLOR::LIGHT_BLUE);
	}
	Base::Render();
	Gotoxy(m_pos.x, m_pos.y);
	cout << "¡×";
	SetColor();
}
