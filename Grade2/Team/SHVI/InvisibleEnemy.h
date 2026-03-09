#pragma once
#include "Enemy.h"
#include "Enums.h"
class InvisibleEnemy : public Enemy
{
public:
	InvisibleEnemy(Dir myDir, float timeMultiply, int lifeMultiply);
	~InvisibleEnemy();
public:
	virtual void Render() override;
};

