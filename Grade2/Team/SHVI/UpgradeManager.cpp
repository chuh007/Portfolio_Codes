#include "UpgradeManager.h"
#include "UIManager.h"
#include "Mci.h"

UpgradeManager* UpgradeManager::m_inst = nullptr;
UpgradeManager::UpgradeManager()
	: m_isbarrier(false)
	, m_firedelay(20)
	, m_damage(10)
	, m_CanupgradeCount(0)
{
	UIManager::GetInst()->
		UpdateUI(UIType::ATTACKPOWER, std::to_string(m_damage));
	UIManager::GetInst()->
		UpdateUI(UIType::ATTACKDELAY, std::to_string(m_firedelay));
}

UpgradeManager::~UpgradeManager()
{
}

bool UpgradeManager::CanUpgrade()
{
	return m_CanupgradeCount > 0;
}

void UpgradeManager::Upgrade(Key _key)
{
	m_CanupgradeCount--;
	switch (_key)
	{
	case Key::UPGRADE1:
		PlaySoundID(SOUNDID::UPGRADE);
		m_damage++;
		break;
	case Key::UPGRADE2:
		PlaySoundID(SOUNDID::UPGRADE);
		if (m_firedelay > 5) m_firedelay--;
		break;
	case Key::UPGRADE3:
		PlaySoundID(SOUNDID::UPGRADE);
		m_isbarrier = true;
		break;
	}
	UIManager::GetInst()->
		UpdateUI(UIType::ATTACKPOWER, std::to_string(m_damage) + " ");
	UIManager::GetInst()->
		UpdateUI(UIType::ATTACKDELAY, std::to_string(m_firedelay) + " ");
}

bool UpgradeManager::GetBarrier()
{
	return m_isbarrier;
}

void UpgradeManager::OffBarrier()
{
	m_isbarrier = false;
}

int UpgradeManager::GetFireDelay()
{
	return m_firedelay;
}

int UpgradeManager::GetDamage()
{
	return m_damage;
}

void UpgradeManager::PlusUpgradeCount()
{
	m_CanupgradeCount++;
}
