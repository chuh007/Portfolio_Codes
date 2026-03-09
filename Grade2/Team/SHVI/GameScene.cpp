#include "GameScene.h"
#include "ICommand.h"
#include "Player.h"
#include "Muzzle.h"
#include "Console.h"
#include "ObjectManager.h"
#include "GameManager.h"
#include "EnemyCollisionManager.h"
#include "UIManager.h"
#include "MapManager.h"
#include "Mci.h"
#include <fstream>


GameScene::GameScene()
{
	std::ifstream mapFile("map.txt");
	if (mapFile.is_open())
	{
		for (int i = 0; i < MAP_HEIGHT; ++i)
		{
			mapFile >> m_map[i];
		}
		mapFile.close();
	}
	else cout << "실패했다실패했다";
	m_inputHandler = new InputHandler();
	nowScene = SCENE::Game;
}

GameScene::~GameScene()
{
	SAFE_DELETE(m_inputHandler);
}

void GameScene::Init()
{
	system("cls");
	InitAllSounds();
	GameManager::GetInst()->Init();
	UIManager::GetInst()->Init();
	MapManager::GetInst()->SetRanderDir(Dir::UP);
	PlaySoundID(SOUNDID::BGM, true);
	nowScene = SCENE::Game;
	m_player = new Player({ 12, 12 });
	m_muzzle = new Muzzle({ 12, 12 });
	Gotoxy(0, 0);
	for (int i = 0; i < MAP_HEIGHT - 1; ++i)
	{
		for (int j = 0; j < MAP_WIDTH - 1; ++j)
		{
			if (!MapManager::GetInst()
				->CanRanderThisPos({ j,i }))
				cout << "  ";
			else if (m_map[i][j] == '2') cout << "  ";
			else cout << (m_map[i][j] == '0' ? "□" : "■");
		}
		Gotoxy(0, i + 1);
	}
}

void GameScene::Update()
{
	ICommand* cmd = m_inputHandler->HandleInput();
	if (cmd)
	{
		cmd->Execute(m_muzzle);
	}
	delete cmd; 
	Gotoxy(0, 0);
	for (int i = 0; i < MAP_HEIGHT - 1; ++i)
	{
		for (int j = 0; j < MAP_WIDTH - 1; ++j)
		{
			if (!MapManager::GetInst()
				->CanRanderThisPos({ j,i }))
				cout << "  ";
			else if (m_map[i][j] == '2') cout << "  ";
			else cout << (m_map[i][j] == '0' ? "□" : "■");
		}
		Gotoxy(0, i + 1);
	}
	ObjectManager::GetInst()->Update();
	GameManager::GetInst()->Update();
	if (GameManager::GetInst()->isGameOver())
	{
		GameManager::GetInst()->SetMuzzleFireCount(m_muzzle->GetFireCount());
		isFirstLoadScene = true;
		nowScene = SCENE::GameOver;
		ReleaseAllSounds();
		ObjectManager::GetInst()->ObjectAllDie();
		EnemyCollisionManager::GetInst()->GameOver();
	}
}