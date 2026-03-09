#pragma once
#include "Enemy.h"
#include "Enums.h"

class FasterEnemy : public Enemy
{
public:
	FasterEnemy(Dir myDir, float timeMultiply, int lifeMultiply);
	~FasterEnemy();
public:
	virtual void Update() override;
	virtual void Render() override;

private:
	bool nowMove;
	int dirMove;
};

