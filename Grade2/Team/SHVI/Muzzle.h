#pragma once
#include "CharacterObject.h"
#include "IFireable.h"
#include "IUpgradeable.h"
class Muzzle : public CharacterObject
	, public IFireable, public IUpgradeable
{
public:
	using Base = CharacterObject;
	Muzzle(Position _pos);
	void Update() override;
	void Render() override;
	void Move(Dir _dir) override;
	bool CanFire() override;
	void Fire();
	int GetFireCount();
	void Upgrade(Key _key) override;
private:
	Dir m_curDir;
	Position m_playerPos;
	int m_fireCount;
	int m_damage;


};

