#include "pch.h"
#include "Background.h"
#include "SettingScene.h"
#include "ResourceManager.h"
#include "VolumeButton.h"
#include "StartButton.h"

void SettingScene::Init()
{
	auto* background = Spawn<Background>(Layer::BACKGROUND, { GAME_WIDTH * 0.5f, GAME_HEIGHT * 0.5f }, { GAME_WIDTH, GAME_HEIGHT });
	background->SetTexture(GET_SINGLE(ResourceManager)->GetTexture(L"TitleBackground"));
	//Spawn<Boss>(Layer::ENEMY, { WINDOW_WIDTH / 2, WINDOW_HEIGHT / 4 }, { 100.f,100.f });

	srand(time(NULL));

	m_uiWidth = WINDOW_WIDTH - GAME_WIDTH;
	m_uiHeight = WINDOW_HEIGHT;

	HWND hWnd = GetActiveWindow();
	HDC hScreenDC = GetDC(hWnd);
	m_hdc = CreateCompatibleDC(hScreenDC);

	m_hUIBitmap = CreateCompatibleBitmap(hScreenDC, m_uiWidth, m_uiHeight);
	ReleaseDC(hWnd, hScreenDC);

	m_hOldBitmap = (HBITMAP)SelectObject(m_hdc, m_hUIBitmap);


	HBRUSH hUIBrush = CreateSolidBrush(RGB(230, 230, 230));
	RECT rect = { 0, 0, m_uiWidth, m_uiHeight };

	FillRect(m_hdc, &rect, hUIBrush);

	DeleteObject(hUIBrush);

	StartButton* exit = Spawn<StartButton>(Layer::UI, { 75,75 }, { 50,50 });
	exit->SetSceneName(L"Title");
	exit->SetText(L"X");

	Vec2 upBtnPos = { GAME_WIDTH - 100, GAME_HEIGHT / 2 };
	Vec2 downBtnPos = { 100, GAME_HEIGHT / 2 };
	VolumeButton* bgmUp = Spawn<VolumeButton>(Layer::UI, upBtnPos, { 50,50 });
	bgmUp->SetVolumeChannel({ SOUND_CHANNEL::BGM});
	bgmUp->SetVolumeValue(0.1f);

	VolumeButton* bgmDown = Spawn<VolumeButton>(Layer::UI, downBtnPos, { 50,50 });
	bgmDown->SetVolumeChannel({ SOUND_CHANNEL::BGM});
	bgmDown->SetVolumeValue(-0.1f);

	upBtnPos.y += 60;
	downBtnPos.y += 60;

	VolumeButton * sfxUp = Spawn<VolumeButton>(Layer::UI, upBtnPos, { 50,50 });
	sfxUp->SetVolumeChannel({ SOUND_CHANNEL::EFFECT });
	sfxUp->SetVolumeValue(0.1f);

	VolumeButton* sfxDown = Spawn<VolumeButton>(Layer::UI, downBtnPos, { 50,50 });
	sfxDown->SetVolumeChannel({ SOUND_CHANNEL::EFFECT });
	sfxDown->SetVolumeValue(-0.1f);
}
