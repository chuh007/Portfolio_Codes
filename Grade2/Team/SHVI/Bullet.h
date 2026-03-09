#pragma once
#include "CharacterObject.h"
#include <vector>
class Bullet : public CharacterObject
{
public:
	Bullet(const Position& _pos, const Dir& _dir, const int& _lifeTick, const int& _damage);
	~Bullet();
public:
	virtual void Update() override;
	virtual void Render() override;
	void Move(Dir _dir) override;
	int GetDamage();
private:
	Dir m_dir;
	int m_lifetick;
	int m_damage;
};