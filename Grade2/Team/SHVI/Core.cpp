#include "Core.h"
#include "Console.h"
#include "Mci.h"
#include "ObjectManager.h"
#include "UIManager.h"
#include "MapManager.h"
#include "GameManager.h"
#include "EnemyManager.h"
#include "UpgradeManager.h"
#include "EnemyCollisionManager.h"
#include <Windows.h>
#include <fstream>

Core::Core()
	: m_isRunning(true)
	, m_resoulution()
{
	ecurScene = SCENE::Title;
	SetConsoleSettings(1000, 650, false, TEXT("GAME"));
	m_resoulution = GetConsoleResolution();
	gameScene = new GameScene();
	titleScene = new TitleScene();
	infoScene = new InfoScene();
	gameOverScene = new GameOverScene();
}

Core::~Core()
{

	ObjectManager::GetInst()->DestoryInst();
	UIManager::GetInst()->DestoryInst();
	MapManager::GetInst()->DestoryInst();
	GameManager::GetInst()->DestoryInst();
	EnemyManager::GetInst()->DestoryInst();
	UpgradeManager::GetInst()->DestoryInst();
	EnemyCollisionManager::GetInst()->DestoryInst();
	SAFE_DELETE(gameScene);
	SAFE_DELETE(titleScene);
	SAFE_DELETE(infoScene);
	SAFE_DELETE(gameOverScene);
}

void Core::Run()
{
	Init();

	while (m_isRunning)
	{
		Update();
		FrameSync(60);
	}
	ObjectManager::GetInst()->ObjectAllDie();
	ReleaseAllSounds();
}

void Core::Init()
{
	SetCursorVisual(false, 50);
	SetLockResize();
}

void Core::Update()
{
	switch (ecurScene)
	{
	case SCENE::Title:
		titleScene->FirstLoadInit();
		titleScene->Update();
		ecurScene = titleScene->GetNowScene();
		break;
	case SCENE::Game:
		gameScene->FirstLoadInit();
		gameScene->Update();
		ecurScene = gameScene->GetNowScene();
		break;
	case SCENE::INFO:
		infoScene->FirstLoadInit();
		infoScene->Update();
		ecurScene = infoScene->GetNowScene();
		break;
	case SCENE::GameOver:
		gameOverScene->FirstLoadInit();
		gameOverScene->Update();
		ecurScene = gameOverScene->GetNowScene();
		break;
	case SCENE::QUIT:
		m_isRunning = false;
		break;
	default:
		m_isRunning = false;
		break;
	}
}

