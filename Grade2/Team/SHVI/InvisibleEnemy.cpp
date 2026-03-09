#include "InvisibleEnemy.h"
#include "Console.h"

InvisibleEnemy::InvisibleEnemy(Dir myDir, float timeMultiply, int lifeMultiply)
	:Enemy(myDir, timeMultiply, lifeMultiply)
{
	m_renderIcon = "◎";
}

InvisibleEnemy::~InvisibleEnemy()
{
	Gotoxy(30, 5);
	cout << "  ";
}



void InvisibleEnemy::Render()
{
	//SetColor(COLOR::WHITE, COLOR::YELLOW);
	//Gotoxy(m_pos.x, m_pos.y);
	//cout << " ";

	SetColor(COLOR::YELLOW);
	Gotoxy(30, 5); //임시로 넣어둔것
	cout << "♨";
	SetColor();
}
