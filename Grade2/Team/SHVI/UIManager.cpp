#include "UIManager.h"
#include "Console.h"
UIManager* UIManager::m_inst = nullptr;

UIManager::UIManager()
	: m_waveCntPos({ GetConsoleResolution().x / 4, 10 })
	, m_modeTxtPos({ GetConsoleResolution().x / 4 - 3, 13 })
	, m_Choice1Pos({ GetConsoleResolution().x / 50 - 3, 30 })
	, m_Choice2Pos({ GetConsoleResolution().x / 8 - 1, 30 })
	, m_Choice3Pos({ GetConsoleResolution().x / 4, 30 })
	, m_descriptionPos({ GetConsoleResolution().x / 4 - 3, 8 })
	, m_attackPowerPos({ GetConsoleResolution().x / 4 + 1, 18 })
	, m_attackDelayPos({ GetConsoleResolution().x / 4 + 1, 19 })
{
}

UIManager::~UIManager()
{
}

void UIManager::UpdateUI(const UIType& _type,
	const std::string& _str)
{
	switch (_type)
	{
	case UIType::WAVECNT:
		Gotoxy(m_waveCntPos.x, m_waveCntPos.y);
		break;
	case UIType::MODETXT:
		Gotoxy(m_modeTxtPos.x, m_modeTxtPos.y);
		break;
	case UIType::CHOICE1:
		Gotoxy(m_Choice1Pos.x, m_Choice1Pos.y);
		break;
	case UIType::CHOICE2:
		Gotoxy(m_Choice2Pos.x, m_Choice2Pos.y);
		break;
	case UIType::CHOICE3:
		Gotoxy(m_Choice3Pos.x, m_Choice3Pos.y);
		break;
	case UIType::DESCRIPTION:
		Gotoxy(m_descriptionPos.x, m_descriptionPos.y);
		break;
	case UIType::ATTACKPOWER:
		Gotoxy(m_attackPowerPos.x, m_attackPowerPos.y);
		break;
	case UIType::ATTACKDELAY:
		Gotoxy(m_attackDelayPos.x, m_attackDelayPos.y);
		break;
	}
	cout << _str;
}

void UIManager::Init()
{
	Gotoxy(m_waveCntPos.x - 3, m_waveCntPos.y);
	cout << "웨이브";
	Gotoxy(m_modeTxtPos.x, m_modeTxtPos.y);
	cout << "!습격!  ";
	Gotoxy(m_attackPowerPos.x - 5, m_attackPowerPos.y);
	cout << "공격 위력: ";
	Gotoxy(m_attackDelayPos.x - 5, m_attackDelayPos.y);
	cout << "공격 간격: ";
}

