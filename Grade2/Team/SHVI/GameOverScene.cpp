#include "GameOverScene.h"
#include "GameManager.h"
#include "ObjectManager.h"
#include "EnemyCollisionManager.h"
#include "Console.h"
#include "Define.h"
#include "Enums.h"
#include "Mci.h"
#include <string>
#include <sstream>
#include <iomanip>

GameOverScene::GameOverScene()
{
	m_inputHandler = new InputHandler();
	resolution = { 0,0 };
	resolution = GetConsoleResolution();
	nowScene = SCENE::GameOver;
}

GameOverScene::~GameOverScene()
{
	SAFE_DELETE(m_inputHandler);
}

void GameOverScene::Init()
{
	InitAllSounds();
	system("cls");
	nowScene = SCENE::GameOver;
	Render();
	EnemyCollisionManager::GetInst()->SetHit(0);
	GameManager::GetInst()->SetMuzzleFireCount(0);
	PlaySoundID(SOUNDID::GAMEOVERBGM,true);
	WaveMessage();
}

void GameOverScene::WaveMessage()
{
	int waveCount = GameManager::GetInst()->GetWaveCount();
	if (waveCount >= 100)
	{
		Message("어?");
	}
	else if (waveCount >= 75)
	{
		Message("어떻게 하신거에요?");
	}
	else if (waveCount >= 50)
	{
		Message("50웨이브를 넘기시다니... 이제 이게임의 모든 컨텐츠를 즐겼다고 봐도 됩니다.");
	}
	else if (waveCount >= 40)
	{
		Message("40웨이브를 넘기시다니 이제는 이게임의 대부분의 컨텐츠를 하셨네요.");
	}
	else if (waveCount >= 30)
	{
		Message("30웨이브를 넘기다니 실력자시네요!!");
	}
	else if (waveCount >= 20)
	{
		Message("20웨이브부터는 이게임의 진정한 시작이라고 할 수 있죠");
	}
	else if (waveCount >= 15)
	{
		Message("이제 슬슬 적들이 까다로워지기 시작할거에요");
	}
	else if (waveCount >= 10)
	{
		Message("팁을 드리자면 강화를 적절히 배분하면 게임의 난이도가 줄어들 수 있어요!!");
	}
	else if (waveCount >= 5)
	{
		Message("시야가 좁아지는것이 너무 불편했나요? 하지만 익숙해지면 잘 하실수 있을거에요!!");
	}
	else if (waveCount >= 4)
	{
		Message("딱 한 웨이브만 넘기면 이게임이 재밌어지기 시작할거에요");
	}
	else if (waveCount >= 3)
	{
		Message("너무 집중이 흐려진다면 방어막을 얻어서 한번을 막는것도 방법이에요");
	}
	else if (waveCount >= 2)
	{
		Message("처음엔 누구나 실수할 수 있죠. 화이팅 합시다!!");
	}
	else if (waveCount >= 1)
	{
		Message("이게임은 처음이신가요? 그렇다면 타이틀에서 조작법 및 게임방법이 있어요. 읽어보면 유익할거에요!");
	}
}


void GameOverScene::Update()
{
	Key key = m_inputHandler->InfoInput();
	if (key == Key::ESC)
	{
		ReleaseAllSounds();
		isFirstLoadScene = true;
		nowScene = SCENE::Title;
		system("cls");
	}
}

void GameOverScene::Render()
{
	int coutMode = _setmode(_fileno(stdout), _O_U16TEXT);

	wcout << L"\t\t\t ██████╗  █████╗ ███╗   ███╗███████╗ ██████╗ ██╗   ██╗███████╗██████╗ " << '\n';
	wcout << L"\t\t\t██╔════╝ ██╔══██╗████╗ ████║██╔════╝██╔═══██╗██║   ██║██╔════╝██╔══██╗" << '\n';
	wcout << L"\t\t\t██║  ███╗███████║██╔████╔██║█████╗  ██║   ██║██║   ██║█████╗  ██████╔╝" << '\n';
	wcout << L"\t\t\t██║   ██║██╔══██║██║╚██╔╝██║██╔══╝  ██║   ██║╚██╗ ██╔╝██╔══╝  ██╔══██╗" << '\n';
	wcout << L"\t\t\t╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗╚██████╔╝ ╚████╔╝ ███████╗██║  ██║" << '\n';
	wcout << L"\t\t\t ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝ ╚═════╝   ╚═══╝  ╚══════╝╚═╝  ╚═╝" << '\n';

	int wcoutMode = _setmode(_fileno(stdout), coutMode);


	cout << "\t\t\t\t\t========================================" << '\n';
	cout << "\t\t\t\t\t=                                      =" << '\n';
	cout << GetWaveString() << '\n';
	cout << GetAccuracyString() << '\n';
	cout << "\t\t\t\t\t=                                      =" << '\n';
	cout << "\t\t\t\t\t========================================" << '\n';
	cout << "\n\n\n \t\t\t\t\t      ESC를 눌러 타이틀로 돌아가기" << '\n';
}

void GameOverScene::Message(std::string msg)
{
	SetColor(COLOR::MINT);
	int consoleWidth = resolution.x;
	int messageLength = msg.length();
	int xPos = (consoleWidth - messageLength) / 2;

	IsGotoxy(xPos, GetCursorPos().Y + 2);
	cout << msg;
	SetColor();
}


std::string GameOverScene::GetWaveString()
{
	int waveCount = GameManager::GetInst()->GetWaveCount();
	std::string waveStr = "살아남은 웨이브 : " + std::to_string(waveCount);
	const int totalSpace = 38;

	int blankTotal = totalSpace - waveStr.length();
	int leftPadding = blankTotal / 2;
	int rightPadding = blankTotal - leftPadding;

	std::string wave = "\t\t\t\t\t=";
	wave.append(leftPadding, ' ');
	wave += waveStr;
	wave.append(rightPadding, ' ');
	wave += '=';

	return wave;
}

std::string GameOverScene::GetAccuracyString()
{
	int hit = EnemyCollisionManager::GetInst()->GetHit();
	int fire = GameManager::GetInst()->GetMuzzleFireCount();

	float accuracyCount = 0.0f;
	if (hit > 0 && fire > 0)
	{
		accuracyCount = (float(hit) / fire) * 100.0f;
	}

	std::stringstream ss;
	ss << std::fixed << std::setprecision(2) << accuracyCount;
	std::string accuracyStr = "명중률 : " + ss.str() + "%";

	const int totalSpace = 38;
	int blankTotal = totalSpace - accuracyStr.length();
	int leftPadding = blankTotal / 2;
	int rightPadding = blankTotal - leftPadding;

	std::string accuracy = "\t\t\t\t\t=";
	accuracy.append(leftPadding, ' ');
	accuracy += accuracyStr;
	accuracy.append(rightPadding, ' ');
	accuracy += '=';

	return accuracy;
}


