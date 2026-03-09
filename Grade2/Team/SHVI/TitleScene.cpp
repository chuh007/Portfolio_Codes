#include "TitleScene.h"
#include "Console.h"
#include "InputHandler.h"
#include "Define.h"
#include "Mci.h"

TitleScene::TitleScene()
{
	resolution = { 0,0 };
	m_inputHandler = new InputHandler();
	resolution = GetConsoleResolution();
	eMenu = Menu::START;
	nowScene = SCENE::Title;
}

TitleScene::~TitleScene()
{
	SAFE_DELETE(m_inputHandler);
}

void TitleScene::Init()
{
	InitAllSounds();
	system("cls");
	eMenu = Menu::START;
	nowScene = SCENE::Title;
	PlaySoundID(SOUNDID::TITLE , true);
	SetColor();
}

void TitleScene::Update()
{
	RenderTitle();
	KeyCheck();
}


void TitleScene::KeyCheck()
{
	int x = resolution.x / 2.5f;
	int y = resolution.y / 3 * 2;

	Key key = m_inputHandler->TitleInput();

	int nowMenu = (int)eMenu;

	switch (key)
	{
	case Key::UP:
		nowMenu--;
		if (0 <= nowMenu && nowMenu <= 2)
		{
			IsGotoxy(x - 1, y + (int)eMenu);
			cout << " ";
			eMenu = (Menu)nowMenu;
			Sleep(75);
		}
		break;
	case Key::DOWN:
		nowMenu++;
		if (0 <= nowMenu && nowMenu <= 2)
		{
			IsGotoxy(x - 1, y + (int)eMenu);
			cout << " ";
			eMenu = (Menu)nowMenu;
			Sleep(75);
		}
		break;
	case Key::SPACE:
		if (eMenu == Menu::START)
		{
			isFirstLoadScene = true;
			nowScene = SCENE::Game;
			system("cls");
			ReleaseAllSounds();
			return;
		}
		if (eMenu == Menu::INFO)
		{
			isFirstLoadScene = true;
			nowScene = SCENE::INFO;
			system("cls");
			return;
		}
		if (eMenu == Menu::QUIT)
		{
			isFirstLoadScene = true;
			nowScene = SCENE::QUIT;
			system("cls");
			ReleaseAllSounds();
			return;
		}
		break;
	}
}

void TitleScene::RenderTitle()
{
	int y = resolution.y / 5;
	IsGotoxy(0, y);
	int coutMode = _setmode(_fileno(stdout), _O_U16TEXT);

	
	wcout << L"\t\t\t   ▄▄▄▄▄▄▄▄▄▄▄  ▄         ▄  ▄               ▄  ▄▄▄▄▄▄▄▄▄▄▄ " << '\n';
	wcout << L"\t\t\t  ▐░░░░░░░░░░░▌▐░▌       ▐░▌▐░▌             ▐░▌▐░░░░░░░░░░░▌" << '\n';
	wcout << L"\t\t\t  ▐░█▀▀▀▀▀▀▀▀▀ ▐░▌       ▐░▌ ▐░▌           ▐░▌  ▀▀▀▀█░█▀▀▀▀ " << '\n';
	wcout << L"\t\t\t  ▐░▌          ▐░▌       ▐░▌  ▐░▌         ▐░▌       ▐░▌     " << '\n';
	wcout << L"\t\t\t  ▐░█▄▄▄▄▄▄▄▄▄ ▐░█▄▄▄▄▄▄▄█░▌   ▐░▌       ▐░▌        ▐░▌     " << '\n';
	wcout << L"\t\t\t  ▐░░░░░░░░░░░▌▐░░░░░░░░░░░▌    ▐░▌     ▐░▌         ▐░▌     " << '\n';
	wcout << L"\t\t\t   ▀▀▀▀▀▀▀▀▀█░▌▐░█▀▀▀▀▀▀▀█░▌     ▐░▌   ▐░▌          ▐░▌     " << '\n';
	wcout << L"\t\t\t            ▐░▌▐░▌       ▐░▌      ▐░▌ ▐░▌           ▐░▌     " << '\n';
	wcout << L"\t\t\t   ▄▄▄▄▄▄▄▄▄█░▌▐░▌       ▐░▌       ▐░▐░▌        ▄▄▄▄█░█▄▄▄▄ " << '\n';
	wcout << L"\t\t\t  ▐░░░░░░░░░░░▌▐░▌       ▐░▌        ▐░▌        ▐░░░░░░░░░░░▌" << '\n';
	wcout << L"\t\t\t   ▀▀▀▀▀▀▀▀▀▀▀  ▀         ▀          ▀          ▀▀▀▀▀▀▀▀▀▀▀ " << '\n';
                                                          

                                            

                                                                                                                                       

	int wcoutMode = _setmode(_fileno(stdout), coutMode);

	int x = resolution.x / 2.5f;
	y = resolution.y / 3 * 2;
	IsGotoxy(x, y);
	cout << "게임 시작";
	IsGotoxy(x, y + 1);
	cout << "게임 정보";
	IsGotoxy(x, y + 2);
	cout << "게임 종료";

	IsGotoxy(x - 1, y + (int)eMenu);
	cout << ">";
}
