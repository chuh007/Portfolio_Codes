#include "Enemy.h"
#include "Console.h"
#include "EnemyData.h"
#include "EnemyCollisionManager.h"
#include "GameManager.h"
#include "Mci.h"

Enemy::Enemy(Dir myDir, float timeMultiply, int lifeMultiply) : CharacterObject({ 0, 0 })
{
	EnemyCollisionManager::GetInst()->Add(this);
	dir = myDir;
	oldTime = clock();
	currentTime = clock();
	int dirMove = (int)myDir;
	m_pos = { SPAWN_X[dirMove] , SPAWN_Y[dirMove] };
	time = MOVE_TIME * 1000 * timeMultiply;
	life = LIFE_INIT * lifeMultiply;
	m_renderIcon = "¡Ø";
}

void Enemy::CheckFeedback(const int& _damage)
{
	life -= _damage;
	Hit();
	if (life <= 0)
	{
		PlaySoundID(SOUNDID::HIT);
		isDead = true;
		EnemyCollisionManager::GetInst()->UpdateEnemyList();
	}
}

void Enemy::Hit()
{
	const int BLINK_COUNT = 2;
	const float BLINK_INTERVAL = 0.05;

	clock_t blinkStart = clock();
	bool isViolet = true;
	int blinkTimes = 0;

	while (blinkTimes < BLINK_COUNT * 2)
	{
		float blinkTimer = (clock() - blinkStart) / (float)CLOCKS_PER_SEC;

		if (blinkTimer >= BLINK_INTERVAL)
		{
			Gotoxy(m_pos.x, m_pos.y);
			SetColor(isViolet ? COLOR::LIGHT_VIOLET : COLOR::WHITE);
			cout << m_renderIcon;

			blinkStart = clock();
			isViolet = !isViolet;
			blinkTimes++;
		}
	}

	SetColor();
}

bool Enemy::PlayerFeedback()
{
	switch (dir)
	{
		case Dir::UP:
			return  m_pos.y <= PLAYER_POS.y;
		case Dir::DOWN:
			return  m_pos.y >= PLAYER_POS.y;
		case Dir::LEFT:
			return  m_pos.x <= PLAYER_POS.x;
		case Dir::RIGHT:
			return  m_pos.x >= PLAYER_POS.x;
		default:
		return false;
	}
}

bool Enemy::CheckFinded(const Dir playerDir)
{
	switch (dir)
	{
	case Dir::UP:
		return playerDir == Dir::DOWN;
	case Dir::DOWN:
		return playerDir == Dir::UP;
	case Dir::LEFT:
		return playerDir == Dir::RIGHT;
	case Dir::RIGHT:
		return playerDir == Dir::LEFT;
	default:
		return false;
	}
}

void Enemy::Update()
{
	currentTime = clock();
	if (currentTime - oldTime >= time)
	{
		oldTime = currentTime;
		Move(dir);
	}
	if (PlayerFeedback())
	{
		isDead = true;
		EnemyCollisionManager::GetInst()->UpdateEnemyList();
		DeadBlink();
		GameManager::GetInst()->CheckGameOver();
	}
	//for (auto& i : BulletManager::GetInst()->GetBullets())
	//{
	//	if (CheckFeedback(*i))
	//	{
	//		life--;
	//		i->isDead = true;
	//		if (life <= 0)
	//		{
	//			isDead = true;
	//			continue;
	//		}
	//	}
	//}
}

void Enemy::DeadBlink()
{
	PlaySoundID(SOUNDID::GAMEOVER);
	const int BLINK_COUNT = 10;
	const double BLINK_INTERVAL = 0.1;

	clock_t blinkStart = clock();
	bool isBlink = true;
	int blinkTimes = 0;

	while (blinkTimes < BLINK_COUNT * 2)
	{
		double blinkTimer = (clock() - blinkStart) / (double)CLOCKS_PER_SEC;

		if (blinkTimer >= BLINK_INTERVAL)
		{
			Gotoxy(m_pos.x, m_pos.y);
			SetColor(isBlink ? COLOR::GRAY : COLOR::WHITE);
			cout << m_renderIcon;

			blinkStart = clock();
			isBlink = !isBlink;
			blinkTimes++;
		}
	}
}

void Enemy::Render()
{
	Base::Render();	
}

void Enemy::Move(Dir _dir)
{
	switch (_dir)
	{
	case Dir::UP:
		--m_pos.y;
		break;
	case Dir::DOWN:
		++m_pos.y;
		break;
	case Dir::LEFT:
		--m_pos.x;
		break;
	case Dir::RIGHT:
		++m_pos.x;
		break;
	}
}
  