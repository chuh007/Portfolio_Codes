#include "FasterEnemy.h"
#include "EnemyData.h"
#include "Console.h"

FasterEnemy::FasterEnemy(Dir myDir, float timeMultiply, int lifeMultiply) : Enemy(myDir, timeMultiply, lifeMultiply)
{
	dirMove = (int)dir;
	m_renderIcon = "กฺ";
	m_pos = { SPAWN_X[dirMove] + ( MOVE_X[dirMove] * -3), SPAWN_Y[dirMove] + (MOVE_Y[dirMove] * -3) };
	nowMove = false;
}

FasterEnemy::~FasterEnemy()
{
	Gotoxy(SPAWN_X[dirMove], SPAWN_Y[dirMove]);
	cout << "  ";
}

void FasterEnemy::Update()
{
	currentTime = clock();
	if (currentTime - oldTime >= 1000)
	{
		oldTime = currentTime;
		nowMove = true;
		m_pos = { SPAWN_X[dirMove] , SPAWN_Y[dirMove] };
		Gotoxy(SPAWN_X[dirMove], SPAWN_Y[dirMove]);
		cout << "  ";
	}
	if (nowMove)
	{
		Enemy::Update();
	}
}

void FasterEnemy::Render()
{
	if (nowMove)
	{
		GameObject::Render();
	}
	else
	{
		SetColor(COLOR::RED);
		Gotoxy(SPAWN_X[dirMove], SPAWN_Y[dirMove]);
		cout << "ขอ";
		SetColor();
	}
}
