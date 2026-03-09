#pragma once
#include "Enemy.h"
#include "Enums.h"
#include "Position.h"

class FlipEnemy : public Enemy
{
public:
	FlipEnemy(Dir myDir, float timeMultiply, int lifeMultiply);
public:
	virtual void Update() override;
	virtual void Render() override;
	void FlipNowPos();

private:
	int dirMove;
	Position flipPos;
};