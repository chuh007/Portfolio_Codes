#pragma once
#include "CharacterObject.h"
#include "Bullet.h"
#include <Time.h>


class Enemy : public CharacterObject
{
public:
	Enemy(Dir myDir, float speed, int lifeSet);
public:
	virtual void Update() override;
	virtual void Render() override;
	void Move(Dir _dir) override;
	void CheckFeedback(const int& _damage);
	bool CheckFinded(const Dir playerDir);
protected:
	bool PlayerFeedback();
	void Hit();
	void DeadBlink();
protected:
	Dir dir;
	int life;
	clock_t oldTime;
	clock_t currentTime;
	float time;
};