#pragma once
#include "Enums.h"
#include "Define.h"
#include "Position.h"
#include <string>

class UIManager
{
private:
	UIManager();
	~UIManager();
public:
	static UIManager* GetInst()
	{
		if (m_inst == nullptr)
			m_inst = new UIManager;
		return m_inst;
	}
	static void DestoryInst()
	{
		SAFE_DELETE(m_inst);
	}
	void UpdateUI(const UIType& _type,
		const std::string& _str);
	void Init();
private:
	static UIManager* m_inst;
	Position m_waveCntPos;
	Position m_modeTxtPos;
	Position m_Choice1Pos;
	Position m_Choice2Pos;
	Position m_Choice3Pos;
	Position m_descriptionPos;
	Position m_attackPowerPos;
	Position m_attackDelayPos;
};

