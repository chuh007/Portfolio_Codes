#include "Muzzle.h"
#include "Console.h"
#include "Bullet.h"
#include "MapManager.h"
#include "UpgradeManager.h"
#include "UIManager.h"
#include "Mci.h"

Muzzle::Muzzle(Position _pos)
	: m_curDir(Dir::UP)
	, m_playerPos(_pos)
	, m_fireCount(0)
	, m_damage(10)
{
	m_renderIcon = "¡ã";
	m_pos = { _pos.x , _pos.y - 1 };
	m_delay = 25;
	m_curdelay = 0;
}

void Muzzle::Update()
{
	Base::Update();
	m_curdelay--;
}

void Muzzle::Render()
{
	Base::Render();
}

void Muzzle::Move(Dir _dir)
{
	m_curDir = _dir;
	MapManager::GetInst()->SetRanderDir(_dir);
	switch (_dir)
	{
	case Dir::UP:
		m_renderIcon = "¡ã";
		m_pos = { m_playerPos.x, 
			m_playerPos.y - 1 };
		break;
	case Dir::DOWN:
		m_renderIcon = "¡å";
		m_pos = { m_playerPos.x,
	m_playerPos.y + 1 };
		break;
	case Dir::LEFT:
		m_renderIcon = "¢¸";
		m_pos = { m_playerPos.x - 1,
m_playerPos.y };
		break;
	case Dir::RIGHT:
		m_renderIcon = "¢º";
		m_pos = { m_playerPos.x + 1,
m_playerPos.y };
		break;
	}
}

bool Muzzle::CanFire()
{
	return m_curdelay <= 0;
}

void Muzzle::Fire()
{
	m_curdelay = m_delay;
	m_fireCount++;
	new Bullet(m_pos, m_curDir, 10, m_damage);
	PlaySoundID(SOUNDID::ATTACK);
}

int Muzzle::GetFireCount()
{
	return m_fireCount;
}

void Muzzle::Upgrade(Key _key)
{
	if (UpgradeManager::GetInst()->CanUpgrade())
	{
		UpgradeManager::GetInst()->Upgrade(_key);
		m_damage = UpgradeManager::GetInst()->GetDamage();
		m_delay = UpgradeManager::GetInst()->GetFireDelay();
	}
	UIManager::GetInst()
		->UpdateUI(UIType::CHOICE1, "                ");
	UIManager::GetInst()
		->UpdateUI(UIType::CHOICE2, "                ");
	UIManager::GetInst()
		->UpdateUI(UIType::CHOICE3, "                ");
}
