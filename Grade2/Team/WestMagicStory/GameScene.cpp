#include "pch.h"
#include "GameScene.h"
#include "Player.h"
#include "CollisionManager.h"
#include "PlayerManager.h"
#include "TestEnemy.h"
#include "ResourceManager.h"
#include "EnemySpawnManger.h"
#include "EnemyProjectile.h"
#include "CircleMoveEnemy.h"
#include "TripleShotEnemy.h"
#include "ShotToPlayerEnemy.h"
#include "InputManager.h"
#include "ItemDropCompo.h"
#include "BombItem.h"
#include "PowerItem.h"
#include "OneUpItem.h"
#include "Background.h"
#include "Effect.h"
#include "EnemyMovement.h"
#include "Health.h"
#include "Boss.h"
#include "Texture.h"

#pragma region 땜방용 매크로


#define CREATE_ENEMY(enemyType, posX, posY, sizeW, sizeH, collSize, maxHP, itemType, itemSizeW, itemSizeH) \
    Enemy* pEnemy = enemyType; \
    pEnemy->SetPos({ (float)posX, (float)posY }); \
    pEnemy->SetSize({ (float)sizeW, (float)sizeH }); \
\
    auto* coll = pEnemy->GetComponent<Collider>(); \
    if (coll) { \
        coll->SetSize((float)collSize); \
    } \
\
    auto* health = pEnemy->GetComponent<Health>(); \
    if (health) { \
        health->SetMaxHP((float)maxHP); \
        health->SetCurrentHP((float)maxHP); \
    } \
\
    Item* item = new itemType; \
    item->SetSize({ (float)itemSizeW, (float)itemSizeH }); \
\
    auto* itemDrop = pEnemy->AddComponent<ItemDropCompo>(); \
    if (itemDrop) { \
        itemDrop->SetItem(item); \
    }

#pragma endregion

GameScene::~GameScene()
{
	Release();
}

void GameScene::Init()
{
	auto* player = Spawn<Player>(Layer::PLAYER, { GAME_WIDTH / 2, 500 }, { 100.f, 100.f });
	// obj->SetScene(this);
	GET_SINGLE(PlayerManager)->SetPlayer(player);

	auto* background = Spawn<Background>(Layer::BACKGROUND, { GAME_WIDTH * 0.5f, GAME_HEIGHT * 0.5f }, { GAME_WIDTH, GAME_HEIGHT });
	//Spawn<Boss>(Layer::ENEMY, { WINDOW_WIDTH / 2, WINDOW_HEIGHT / 4 }, { 100.f,100.f });
	
	GET_SINGLE(CollisionManager)->CheckLayer(Layer::PROJECTILE, Layer::ENEMY);
	GET_SINGLE(CollisionManager)->CheckLayer(Layer::PLAYER, Layer::DEFAULT);
	GET_SINGLE(CollisionManager)->CheckLayer(Layer::PLAYER, Layer::ENEMYPROJECTILE);
	GET_SINGLE(CollisionManager)->CheckLayer(Layer::ENEMYPROJECTILE, Layer::PROJECTILEDELETER);
	GET_SINGLE(CollisionManager)->CheckLayer(Layer::PLAYER, Layer::ITEM);

	GET_SINGLE(ResourceManager)->Stop(SOUND_CHANNEL::BGM);
	GET_SINGLE(ResourceManager)->Play(L"BGM");

	GET_SINGLE(EnemySpawnManger)->Init();
	GET_SINGLE(PoolManager)->AddPool<EnemyProjectile>
		(PoolType::EnemyProjectile, 1000, Layer::ENEMYPROJECTILE);
	GET_SINGLE(PoolManager)->AddPool<PlayerProjectile>
		(PoolType::PlayerProj, 100, Layer::PROJECTILE);
	GET_SINGLE(PoolManager)->AddPool<Effect>
		(PoolType::Effect, 10, Layer::ENEMYPROJECTILE);
	
	srand(time(NULL));

	m_uiWidth = WINDOW_WIDTH - GAME_WIDTH;
	m_uiHeight = WINDOW_HEIGHT;

	HWND hWnd = GetActiveWindow();
	HDC hScreenDC = GetDC(hWnd);
	m_hdc = CreateCompatibleDC(hScreenDC);

	m_hUIBitmap = CreateCompatibleBitmap(hScreenDC, m_uiWidth, m_uiHeight);
	ReleaseDC(hWnd, hScreenDC);

	m_hOldBitmap = (HBITMAP)SelectObject(m_hdc, m_hUIBitmap);

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
	else
	{
		HBRUSH hUIBrush = CreateSolidBrush(RGB(230, 230, 230));
		RECT rect = { 0, 0, m_uiWidth, m_uiHeight };
		FillRect(m_hdc, &rect, hUIBrush);
		DeleteObject(hUIBrush);
	}

	//여기서부터 적 세팅

	ResourceManager* resourceManager = GET_SINGLE(ResourceManager);
	
	EnemySpawnManger* enemyManager = GET_SINGLE(EnemySpawnManger);
	enemyManager->SetSpawnScene(this);
	BezierPathData* wave1path = enemyManager->GetPath(L"Left-Right");
	BezierPathData* wave2path = enemyManager->GetPath(L"Right-Left");

	for(float i = 0; i < 3; i += 0.25f)
	{
		TestEnemy* testEnemy = new TestEnemy;
		CREATE_ENEMY(testEnemy, -50, 100, 95, 55, 25.f, 12.f, PowerItem, 25.f, 25.f);
		testEnemy->SetTexture(resourceManager->GetTexture(L"NormalEnemy"));

		auto* movecompo = testEnemy->AddComponent<EnemyMovement>();
		movecompo->SetRepeatType(MoveRepeatType::Stop);
		movecompo->AddPathData(wave1path);
		movecompo->SetSpeed(200.f);

		enemyManager->AddEnemySpawnQueue({ i , testEnemy });
	}

	for(float i = 6; i < 9; i += 0.25f)
	{

		TestEnemy* testEnemy = new TestEnemy;
		CREATE_ENEMY(testEnemy, 800, 100, 95, 55, 25.f, 12.f, PowerItem, 25.f, 25.f);

		testEnemy->SetTexture(resourceManager->GetTexture(L"NormalEnemy"));
		auto* movecompo = testEnemy->AddComponent<EnemyMovement>();
		movecompo->SetRepeatType(MoveRepeatType::Stop);
		movecompo->AddPathData(wave2path);
		movecompo->SetSpeed(200.f);

		enemyManager->AddEnemySpawnQueue({i , testEnemy });
	}
	for(float i = 9; i < 12; i += 0.25f)
	{
		TestEnemy* testEnemy = new TestEnemy;
		CREATE_ENEMY(testEnemy, -50, 100, 95, 55, 25.f, 12.f, PowerItem, 25.f, 25.f);
		testEnemy->SetTexture(resourceManager->GetTexture(L"NormalEnemy"));

		auto* movecompo = testEnemy->AddComponent<EnemyMovement>();
		movecompo->SetRepeatType(MoveRepeatType::Stop);
		movecompo->AddPathData(wave1path);
		movecompo->SetSpeed(200.f);

		enemyManager->AddEnemySpawnQueue({ i , testEnemy });
	}

	for(float i = 12; i < 15; i += 0.25f)
	{

		TestEnemy* testEnemy = new TestEnemy;
		CREATE_ENEMY(testEnemy, 800, 100, 95, 55, 25.f, 12.f, PowerItem, 25.f, 25.f);

		testEnemy->SetTexture(resourceManager->GetTexture(L"NormalEnemy"));
		auto* movecompo = testEnemy->AddComponent<EnemyMovement>();
		movecompo->SetRepeatType(MoveRepeatType::Stop);
		movecompo->AddPathData(wave2path);
		movecompo->SetSpeed(200.f);

		enemyManager->AddEnemySpawnQueue({i , testEnemy });
	}
	BezierPathData* down = enemyManager->GetPath(L"Up");
	BezierPathData* zigzagR = enemyManager->GetPath(L"ZigzagR");
	BezierPathData* zigzagL = enemyManager->GetPath(L"ZigzagL");

	for (int i = 0; i < 4; ++i)
	{
		ShotToPlayerEnemy* fireEnemy = new ShotToPlayerEnemy;
		CREATE_ENEMY(fireEnemy, 100 + i * 200, -50, 75,75,75.f/2,20.f, PowerItem, 25.f,25.f);
		fireEnemy->SetFireTime(1.5f, 0.05f, 12, true);

		auto* movecompo = fireEnemy->AddComponent<EnemyMovement>();
		movecompo->SetRepeatType(MoveRepeatType::Stop);
		movecompo->AddPathData(down);
		movecompo->AddPathData(down);
		movecompo->AddPathData(i < 2 ? wave2path : wave1path);
		movecompo->SetSpeed(125.f);

		enemyManager->AddEnemySpawnQueue({ 15.f + i , fireEnemy });
	}

	for (int i = 0; i < 9; ++i)
	{
		CircleMoveEnemy* fireEnemy = new CircleMoveEnemy;
		if (i  == 0)
		{
			CREATE_ENEMY(fireEnemy, 100 + (i % 3) * 175, 0, 75, 75, 75.f / 2, 20.0f, BombItem, 25.f, 25.f);
		}
		else
		{
			CREATE_ENEMY(fireEnemy, 100 + (i % 3) * 175, 0, 75,75,75.f/2,20.0f, PowerItem, 25.f,25.f);
		}
		fireEnemy->SetShotCount(25);


		auto* movecompo = fireEnemy->AddComponent<EnemyMovement>();
		movecompo->SetRepeatType(MoveRepeatType::Stop);

		for (int j = 0; j < 6; ++j)
		{
			movecompo->AddPathData(zigzagR);
			movecompo->AddPathData(zigzagL);
		}
		if (i%3 == 0)
		{
			movecompo->AddPathData(wave2path);
		}
		else
		{
			movecompo->AddPathData(wave1path);
		}
		movecompo->SetSpeed(125.f);

		enemyManager->AddEnemySpawnQueue({ 23.f + 1.5f*i , fireEnemy });
	}

	for (float i = 40; i < 43; i += 0.25f)
	{
		TestEnemy* testEnemy = new TestEnemy;
		CREATE_ENEMY(testEnemy, -50, 100, 92, 55, 25.f, 12.f, PowerItem, 25.f, 25.f);

		testEnemy->SetTexture(resourceManager->GetTexture(L"NormalEnemy"));
		auto* movecompo = testEnemy->AddComponent<EnemyMovement>();
		movecompo->SetRepeatType(MoveRepeatType::Stop);
		movecompo->AddPathData(wave1path);
		movecompo->SetSpeed(200.f);

		enemyManager->AddEnemySpawnQueue({ i , testEnemy });
	}
	for (float i = 41; i < 44; i += 0.25f)
	{

		TestEnemy* testEnemy = new TestEnemy;
		CREATE_ENEMY(testEnemy, 800, 100, 92, 55, 25.f, 12.f, PowerItem, 25.f, 25.f);

		testEnemy->SetTexture(resourceManager->GetTexture(L"NormalEnemy"));
		auto* movecompo = testEnemy->AddComponent<EnemyMovement>();
		movecompo->SetRepeatType(MoveRepeatType::Stop);
		movecompo->AddPathData(wave2path);
		movecompo->SetSpeed(200.f);

		enemyManager->AddEnemySpawnQueue({ i , testEnemy });
	}

	for (int i = 0; i < 8; ++i)
	{
		TripleShotEnemy* triple = new TripleShotEnemy;
		CREATE_ENEMY(triple, 200 + (i % 2) * 300, -50, 50, 50, 25, 10.f, PowerItem, 25.f, 25.f);

		auto* movecompo = triple->AddComponent<EnemyMovement>();
		movecompo->SetRepeatType(MoveRepeatType::Stop);
		for (int j = 0; j < 5; ++j)
		{
			movecompo->AddPathData(down);
		}
		if(i%2 == 0)
			movecompo->AddPathData(wave1path);
		else
			movecompo->AddPathData(wave2path);

		movecompo->SetSpeed(250.f);


		enemyManager->AddEnemySpawnQueue({ 50.f + i , triple });
	}

	BezierPathData* arcPathL = enemyManager->GetPath(L"ArcMoveL");
	BezierPathData* arcPathR = enemyManager->GetPath(L"ArcMoveR");
	for (int i = 0; i < 10; ++i)
	{
		TestEnemy* testEnemy = new TestEnemy;
		CREATE_ENEMY(testEnemy, 0, 500, 95, 55, 25.f, 12.f, PowerItem, 25.f, 25.f);

		testEnemy->SetTexture(resourceManager->GetTexture(L"NormalEnemy"));
		auto* movecompo = testEnemy->AddComponent<EnemyMovement>();
		movecompo->SetRepeatType(MoveRepeatType::Stop);
		movecompo->AddPathData(arcPathL);
		movecompo->SetSpeed(200);

		enemyManager->AddEnemySpawnQueue({ 50.f + i , testEnemy });
	}
	for (int i = 0; i < 10; ++i)
	{
		TestEnemy* testEnemy = new TestEnemy;
		CREATE_ENEMY(testEnemy, GAME_WIDTH, 500, 95, 55, 25.f, 12.f, PowerItem, 25.f, 25.f);

		testEnemy->SetTexture(resourceManager->GetTexture(L"NormalEnemy"));
		auto* movecompo = testEnemy->AddComponent<EnemyMovement>();
		movecompo->SetRepeatType(MoveRepeatType::Stop);
		movecompo->AddPathData(arcPathR);
		movecompo->SetSpeed(200.f);

		enemyManager->AddEnemySpawnQueue({ 50.f + i , testEnemy });
	}
	for (int i = 0; i < 12; ++i)
	{
		TripleShotEnemy* triple = new TripleShotEnemy;
		if (i == 0)
		{
			CREATE_ENEMY(triple, 200 + (i % 2) * 300, -50, 50, 50, 25, 15.f, OneUpItem, 25.f, 25.f);
		}
		else
		{
			CREATE_ENEMY(triple, 200 + (i % 2) * 300, -50, 50, 50, 25, 15.f, PowerItem, 25.f, 25.f);
		}

		auto* movecompo = triple->AddComponent<EnemyMovement>();
		movecompo->SetRepeatType(MoveRepeatType::Stop);

		for (int i = 0; i < 10; ++i)
		{
			movecompo->AddPathData(down);
		}

		movecompo->SetSpeed(150.f);

		enemyManager->AddEnemySpawnQueue({ 57.f + (i / 2) *1.f, triple });
	}

	for (int i = 0; i < 4; i++)
	{
		ShotToPlayerEnemy* shotEnemy = new ShotToPlayerEnemy;
		CREATE_ENEMY(shotEnemy, 200 + i * 150, -50, 50, 50, 25, 20.f, PowerItem, 25.f, 25.f);

		auto* movecompo = shotEnemy->AddComponent<EnemyMovement>();

		for (int i = 0; i < 10; ++i)
		{
			movecompo->AddPathData(down);
		}

		shotEnemy->SetFireTime(2, 0.1, 5, false);
		movecompo->SetRepeatType(MoveRepeatType::Stop);
		movecompo->SetSpeed(75.f);
		enemyManager->AddEnemySpawnQueue({ 62 + i * 0.5f, shotEnemy });
	}

	for (int i = 0; i < 9; ++i)
	{
		CircleMoveEnemy* fireEnemy = new CircleMoveEnemy;
		CREATE_ENEMY(fireEnemy, 100 + (i % 3) * 175, 0, 75, 75, 75.f / 2, 5.0f, PowerItem, 25.f, 25.f);
		fireEnemy->SetShotCount(25);

		auto* movecompo = fireEnemy->AddComponent<EnemyMovement>();
		movecompo->SetRepeatType(MoveRepeatType::Stop);

		for (int j = 0; j < 12; ++j)
		{
			movecompo->AddPathData(down);
		}
		movecompo->SetSpeed(125.f);

		enemyManager->AddEnemySpawnQueue({ 68.f + 1.5f * i , fireEnemy });
	}
	// 1분대 채움

	for (float i = 64; i < 67; i += 0.25f)
	{
		TestEnemy* testEnemy = new TestEnemy;
		CREATE_ENEMY(testEnemy, -50, 100, 95, 55, 75.f / 2, 9.f, PowerItem, 25.f, 25.f);

		testEnemy->SetTexture(resourceManager->GetTexture(L"NormalEnemy"));

		auto* movecompo = testEnemy->AddComponent<EnemyMovement>();
		movecompo->SetRepeatType(MoveRepeatType::Stop);
		movecompo->AddPathData(wave1path);
		movecompo->SetSpeed(200.f);

		enemyManager->AddEnemySpawnQueue({ i , testEnemy });
	}
	for (float i = 64; i < 67; i += 0.25f)
	{
		TestEnemy* testEnemy = new TestEnemy;
		CREATE_ENEMY(testEnemy, -50, 300, 95, 55, 75.f / 2, 9.f, PowerItem, 25.f, 25.f);

		testEnemy->SetTexture(resourceManager->GetTexture(L"NormalEnemy"));

		auto* movecompo = testEnemy->AddComponent<EnemyMovement>();
		movecompo->SetRepeatType(MoveRepeatType::Stop);
		movecompo->AddPathData(wave1path);
		movecompo->SetSpeed(200.f);

		enemyManager->AddEnemySpawnQueue({ i , testEnemy });
	}

	for (int i = 0; i < 3; ++i)
	{
		CircleMoveEnemy* fireEnemy = new CircleMoveEnemy;
		if (i % 3 == 0)
		{
			CREATE_ENEMY(fireEnemy, 100 + (i % 3) * 175, 0, 75, 75, 75.f / 2, 12.0f, BombItem, 25.f, 25.f);
		}
		else
		{
			CREATE_ENEMY(fireEnemy, 100 + (i % 3) * 175, 0, 75, 75, 75.f / 2, 12.0f, PowerItem, 25.f, 25.f);
		}
		fireEnemy->SetShotCount(25);


		auto* movecompo = fireEnemy->AddComponent<EnemyMovement>();
		movecompo->SetRepeatType(MoveRepeatType::Stop);

		for (int j = 0; j < 10; ++j)
		{
			movecompo->AddPathData(down);
		}
		movecompo->SetSpeed(250.f);

		enemyManager->AddEnemySpawnQueue({ 78.f + i , fireEnemy });
	}

	enemyManager->SetBG(background);
	enemyManager->AddBossSpawn(85.f);
	//여기까지 적 세팅
}

void GameScene::Update()
{
	Scene::Update();
	GET_SINGLE(EnemySpawnManger)->Update();
}

void GameScene::Render(HDC _hdc) {
	Scene::Render(_hdc);

	if (m_hdc != nullptr && m_hUIBitmap != nullptr)
	{
		BitBlt(_hdc,
			GAME_WIDTH, 0,
			m_uiWidth, m_uiHeight,
			m_hdc, 0, 0,
			SRCCOPY);
	}

	Player* player = GET_SINGLE(PlayerManager)->GetPlayer();
	if (!player) return;

	int life = player->GetLifeCount();
	int bombCnt = player->GetBombCount();
	int power = player->GetPowerLevel();

	SetTextColor(_hdc, RGB(255, 255, 255));
	SetBkMode(_hdc, TRANSPARENT);

	HFONT hFont = GET_SINGLE(ResourceManager)->GetFont(FontType::TITLE);
	HFONT hOldFont = nullptr;
	if (hFont != nullptr)
	{
		hOldFont = (HFONT)SelectObject(_hdc, hFont);
	}

	const int UI_START_X = GAME_WIDTH + 10;

	Texture* lTex = GET_SINGLE(ResourceManager)->GetTexture(L"LifeIcon");
	Texture* bTex = GET_SINGLE(ResourceManager)->GetTexture(L"BombIcon");
	Texture* gTex = GET_SINGLE(ResourceManager)->GetTexture(L"GameIcon");

	LONG lifeWidth = lTex->GetWidth();
	LONG lifeHeight = lTex->GetHeight();
	LONG bombWidth = bTex->GetWidth();
	LONG bombHeight = bTex->GetHeight();

	const int ICON_GAP_LIFE = lifeWidth + 5;
	const int ICON_GAP_BOMB = bombWidth + 5;

	const int LIFE_TEXT_WIDTH_OFFSET = 80;
	const int BOMB_TEXT_WIDTH_OFFSET = 80;

	const int LIFE_START_Y = 50;

	wstring lifePrefix = L"LIFE : ";
	TextOut(_hdc, UI_START_X, LIFE_START_Y, lifePrefix.c_str(), (int)lifePrefix.length());

	for (int i = 0; i < life; ++i)
	{
		::TransparentBlt(
			_hdc,
			UI_START_X + LIFE_TEXT_WIDTH_OFFSET + (i * ICON_GAP_LIFE),
			LIFE_START_Y,
			lifeWidth, lifeHeight,
			lTex->GetTextureDC(),
			0, 0, lifeWidth, lifeHeight,
			RGB(255, 0, 255));
	}

	const int BOMB_START_Y = 100;

	wstring bombPrefix = L"BOMB : ";
	TextOut(_hdc, UI_START_X, BOMB_START_Y, bombPrefix.c_str(), (int)bombPrefix.length());

	for (int i = 0; i < bombCnt; ++i)
	{
		::TransparentBlt(
			_hdc,
			UI_START_X + BOMB_TEXT_WIDTH_OFFSET + (i * ICON_GAP_BOMB),
			BOMB_START_Y,
			bombWidth, bombHeight,
			bTex->GetTextureDC(),
			0, 0, bombWidth, bombHeight,
			RGB(255, 0, 255));
	}

	// 여기부터 파워
	const int POWER_START_Y = 150;
	const int MAX_POWER = 128;
	const int BAR_HEIGHT = 18;
	const int BAR_CENTER_Y = POWER_START_Y + (BAR_HEIGHT / 2.0f);
	const float BAR_LEFT_X = (float)UI_START_X + 10;
	const int TEXT_START_Y = POWER_START_Y + 1; 

	wstring powerStr = std::format(L"POWER: {} / {}", power, MAX_POWER);
	SIZE textSize;

	GetTextExtentPoint32(_hdc, powerStr.c_str(), (int)powerStr.length(), &textSize);

	const int BAR_WIDTH = textSize.cx + 10; 
	const float BAR_CENTER_X = BAR_LEFT_X + (BAR_WIDTH / 2.0f);

	GDISelector bar(_hdc, BrushType::HOLLOW);
	RECT_RENDER(_hdc, BAR_CENTER_X, BAR_CENTER_Y,
		BAR_WIDTH, BAR_HEIGHT);

	float powerRatio = (float)power / (float)MAX_POWER;
	float currentBarWidth = (float)BAR_WIDTH * powerRatio;

	GDISelector barFill(_hdc, BrushType::GREY);

	float filledBarCenterX = BAR_LEFT_X + (currentBarWidth / 2.0f);

	if (power > 0)
	{
		RECT_RENDER(_hdc, filledBarCenterX, BAR_CENTER_Y,
			currentBarWidth, BAR_HEIGHT);
	}

	TextOut(_hdc, (int)BAR_LEFT_X + 5, TEXT_START_Y, powerStr.c_str(), (int)powerStr.length());

	if (gTex != nullptr)
	{
		LONG gWidth = gTex->GetWidth();
		LONG gHeight = gTex->GetHeight();

		const int RENDER_WIDTH = 340;  
		const int RENDER_HEIGHT = 340;

		const int UI_END_X = GAME_WIDTH + m_uiWidth;

		const int RENDER_X = UI_END_X - RENDER_WIDTH - 10;
		const int RENDER_Y = m_uiHeight - RENDER_HEIGHT - 10;

		::TransparentBlt(
			_hdc,
			RENDER_X,
			RENDER_Y,
			RENDER_WIDTH, RENDER_HEIGHT,
			gTex->GetTextureDC(),
			0, 0, gWidth, gHeight,
			RGB(255, 0, 255));
	}

	if (hOldFont != nullptr)
	{
		SelectObject(_hdc, hOldFont);
	}
}

void GameScene::Release()
{
	Scene::Release();
	if (m_hdc != nullptr) {
		SelectObject(m_hdc, m_hOldBitmap);
		DeleteDC(m_hdc);
		m_hdc = nullptr;
	}
	if (m_hUIBitmap != nullptr) {
		DeleteObject(m_hUIBitmap);
		m_hUIBitmap = nullptr;
	}
}
