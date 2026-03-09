#pragma once
#include "Console.h"
#include "Define.h"
#include "GameObject.h"
#include "ICommand.h"
#include "Enums.h"
#include "GameScene.h"
#include "TitleScene.h"
#include "InfoScene.h"
#include "GameOverScene.h"


class GameObject;
class InputHandler;

class Core
{
public:
	Core();
	~Core();
public:
	void Run();
private:
	void Init();
	void Update();
private:
	SCENE ecurScene;
	bool m_isRunning;
	Position m_resoulution;

private:
	GameScene* gameScene;
	TitleScene* titleScene;
	InfoScene* infoScene;
	GameOverScene* gameOverScene;
};
