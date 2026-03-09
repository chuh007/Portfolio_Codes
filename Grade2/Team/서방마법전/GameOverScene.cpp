#include "pch.h"
#include "GameOverScene.h"
#include "StartButton.h"
#include "ExitButton.h"
#include "Background.h"
#include "ResourceManager.h"
#include "Texture.h"
#include "ButtonSelector.h"

void GameOverScene::Init()
{
	m_uiWidth = WINDOW_WIDTH - GAME_WIDTH;
	m_uiHeight = WINDOW_HEIGHT;

	HWND hWnd = GetActiveWindow();
	HDC hScreenDC = GetDC(hWnd);
	m_hdc = CreateCompatibleDC(hScreenDC);

	Background* bg = Spawn<Background>(Layer::BACKGROUND, { GAME_WIDTH / 2, GAME_HEIGHT / 2 }, { GAME_WIDTH, GAME_HEIGHT });

	m_hUIBitmap = CreateCompatibleBitmap(hScreenDC, m_uiWidth, m_uiHeight);
	ReleaseDC(hWnd, hScreenDC);

	m_hOldBitmap = (HBITMAP)SelectObject(m_hdc, m_hUIBitmap);

	HBRUSH hUIBrush = CreateSolidBrush(RGB(230, 230, 230));
	RECT rect = { 0, 0, m_uiWidth, m_uiHeight };

	FillRect(m_hdc, &rect, hUIBrush);

	DeleteObject(hUIBrush);
	
	ButtonSelector* selector = new ButtonSelector;
	selector->SetSize({ 50.f,50.f });
	AddObject(selector, Layer::UI);

	float btnpositionY = GAME_HEIGHT * 2 / 3;
	StartButton* button = new StartButton;
	button->SetSize({ 200.f, 65.f });
	button->SetPos({ GAME_WIDTH/2.0f , btnpositionY });
	button->SetTexture(GET_SINGLE(ResourceManager)->GetTexture(L"TitleBtn"));
	button->SetSceneName(L"Title");

	btnpositionY += 80.f;

	Button* exit = new ExitButton;
	exit->SetSize({ 200.f, 65.f });
	exit->SetPos({ GAME_WIDTH / 2.0f, btnpositionY });
	exit->SetTexture(GET_SINGLE(ResourceManager)->GetTexture(L"ExitBtn"));
	exit->SetText(L"Exit");

	AddObject(button, Layer::UI);
	AddObject(exit, Layer::UI);
	selector->AssignButton(button);
	selector->AssignButton(exit);

	Texture* bgTex = GET_SINGLE(ResourceManager)->GetTexture(L"UIBackground");

	if (bgTex != nullptr)
	{
		LONG bgWidth = bgTex->GetWidth();
		LONG bgHeight = bgTex->GetHeight();

		::TransparentBlt(
			m_hdc,
			0, 0, m_uiWidth, m_uiHeight,
			bgTex->GetTextureDC(),
			0, 0, bgWidth, bgHeight,
			RGB(255, 0, 255));
	}
}

void GameOverScene::Update()
{
	Scene::Update();
}

void GameOverScene::Release()
{
	Scene::Release();
}

void GameOverScene::Render(HDC _hdc)
{
	Scene::Render(_hdc);

	if (m_hdc != nullptr && m_hUIBitmap != nullptr)
	{
		BitBlt(_hdc,
			GAME_WIDTH, 0,
			m_uiWidth, m_uiHeight,
			m_hdc, 0, 0,
			SRCCOPY);
	}

	Texture* gTex = GET_SINGLE(ResourceManager)->GetTexture(L"GameOver");

	if (gTex != nullptr)
	{
		LONG logoWidth = gTex->GetWidth();
		LONG logoHeight = gTex->GetHeight();

		const int RENDER_X = 100;
		const int RENDER_Y = 10;
		const int RENDER_WIDTH = 512;
		const int RENDER_HEIGHT = 512;

		::TransparentBlt(
			_hdc,
			RENDER_X,
			RENDER_Y,
			RENDER_WIDTH, RENDER_HEIGHT,
			gTex->GetTextureDC(),
			0, 0, logoWidth, logoHeight,
			RGB(255, 0, 255));
	}
}
