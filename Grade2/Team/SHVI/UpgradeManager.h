#pragma once
#include "defines.h"
#include "Enums.h"

class UpgradeManager
{
private:
	UpgradeManager();
	~UpgradeManager();
public:
	static UpgradeManager* GetInst()
	{
		if (m_inst == nullptr)
		{
			m_inst = new UpgradeManager;
		}
		return m_inst;
	}
	static void DestoryInst()
	{
		SAFE_DELETE(m_inst);
	}
	bool CanUpgrade();
	void Upgrade(Key _key);
public:
	bool GetBarrier();
	void OffBarrier();
	int GetFireDelay();
	int GetDamage();
	void PlusUpgradeCount();
private:
	bool m_isbarrier;
	int m_firedelay;
	int m_damage;
	int m_CanupgradeCount;
private:
	static UpgradeManager* m_inst;
};

