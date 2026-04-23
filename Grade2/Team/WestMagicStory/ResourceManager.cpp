#include "pch.h"
#include "ResourceManager.h"
#include "Texture.h"
bool ResourceManager::Init()
{
	//fs::path curPath = fs::current_path();
	//m_resourcePath = curPath.parent_path() / L"Output\\build\\Resource\\";

	wchar_t buf[MAX_PATH] = {}; // windows 최대 경로 길이
	::GetModuleFileNameW(nullptr, buf, MAX_PATH); // 현재 실행중인 exe 경로 buf에 저장   
	fs::path exeDir = fs::path(buf).parent_path();                //  buf 전체 경로를 path 객체로 가서 디렉토리만 추출
	fs::path resourceDir = exeDir.parent_path() / L"build" / L"Resource\\"; // release모드일때 build 한번더 붙이는거 무시
	m_resourcePath = resourceDir.native();

	RegisterTexture();
	RegisterGDI();

	FMOD::System_Create(&m_pSoundSystem); // 시스템 생성함수
	if (m_pSoundSystem != nullptr)
		m_pSoundSystem->init(64, FMOD_INIT_NORMAL, nullptr);

	RegisterSound();
	return true;
}

void ResourceManager::Release()
{
	// texture delete
	std::unordered_map<wstring, Texture*>::iterator iter;
	for (iter = m_mapTexture.begin(); iter != m_mapTexture.end(); ++iter)
		SAFE_DELETE(iter->second);
	m_mapTexture.clear();

	ReleaseGDI();
	ReleaseFonts();
	std::unordered_map<wstring, SoundInfo*>::iterator iterSound;
	for (iterSound = m_mapSounds.begin(); iterSound != m_mapSounds.end(); ++iterSound)
		SAFE_DELETE(iterSound->second);
	m_mapSounds.clear();

	m_pSoundSystem->close();
	m_pSoundSystem->release();
}

void ResourceManager::RegisterSound()
{
	LoadSound(L"BossBGM", L"Sound\\Necrofantasia.mp3", true);
	LoadSound(L"BGM", L"Sound\\Chinese Tea.mp3", true);
	LoadSound(L"TitleBGM", L"Sound\\ghkstkdgid.mp3", true);
	LoadSound(L"FireSound", L"Sound\\FireSound.mp3", false);
	LoadSound(L"FireSound2", L"Sound\\FireSound2.mp3", false);
	LoadSound(L"SwordSound", L"Sound\\SwordSound.mp3", false);
	LoadSound(L"CircleSound", L"Sound\\CircleSound.mp3", false);
	LoadSound(L"SpellEndSound", L"Sound\\se_enep00.wav", false);
	LoadSound(L"SpellSound", L"Sound\\se_cardget.wav", false);
	LoadSound(L"PlayerHit", L"Sound\\playerHit.wav", false);
	LoadSound(L"Bomb", L"Sound\\bomb.wav", false);
}

void ResourceManager::FmodUpdate()
{
	if (m_pSoundSystem)
		m_pSoundSystem->update();
}
void ResourceManager::LoadSound(const wstring& _key, const wstring& _path, bool _isLoop)
{
	if (FindSound(_key) || !m_pSoundSystem)
		return;
	wstring strFilePath = m_resourcePath;
	strFilePath += _path;

	// wstring to string
	std::string str;
	str.assign(strFilePath.begin(), strFilePath.end());

	// 루프할지 말지 결정
	FMOD_MODE eMode = FMOD_LOOP_NORMAL; // 반복 출력
	if (!_isLoop)
		eMode = FMOD_DEFAULT; // 사운드 1번만 출력
	FMOD::Sound* p = nullptr;

	// BGM면 stream, 아니면 sound
	// 팩토리함수
	//// 사운드 객체를 만드는 것은 system임.
	//						//파일경로,  FMOD_MODE, NULL, &sound
	FMOD_RESULT r = _isLoop
		? m_pSoundSystem->createStream(str.c_str(), eMode, nullptr, &p)
		: m_pSoundSystem->createSound(str.c_str(), eMode, nullptr, &p);

	if (r != FMOD_OK || !p)
		return;

	SoundInfo* pSound = new SoundInfo;
	pSound->IsLoop = _isLoop;
	pSound->pSound = p;
	m_mapSounds.insert({ _key, pSound });

}

void ResourceManager::Play(const wstring& _key)
{
	SoundInfo* pSound = FindSound(_key);
	if (!pSound)
		return;
	SOUND_CHANNEL eChannel = SOUND_CHANNEL::BGM;
	if (!pSound->IsLoop)
		eChannel = SOUND_CHANNEL::EFFECT;
	// 사운드 재생 함수. &channel로 어떤 채널을 통해 재생되는지 포인터 넘김
	m_pSoundSystem->playSound(pSound->pSound, nullptr, false, &m_pChannel[(UINT)eChannel]);

}

void ResourceManager::Stop(SOUND_CHANNEL _channel)
{
	m_pChannel[(UINT)_channel]->stop();

}

void ResourceManager::Volume(SOUND_CHANNEL _channel, float _vol)
{
	// 0.0 ~ 1.0 볼륨 조절
	m_pChannel[(UINT)_channel]->setVolume(_vol);

}

void ResourceManager::Pause(SOUND_CHANNEL _channel, bool _ispause)
{
	m_pChannel[(UINT)_channel]->setPaused(_ispause);
}
SoundInfo* ResourceManager::FindSound(const wstring& _key)
{
	std::unordered_map<wstring, SoundInfo*>::iterator iter = m_mapSounds.find(_key);

	if (iter == m_mapSounds.end())
		return nullptr;
	return iter->second;
}


void ResourceManager::RegisterGDI()
{
	// BRUSH
	m_Brushs[(UINT)BrushType::HOLLOW] = (HBRUSH)::GetStockObject(HOLLOW_BRUSH);
	m_Brushs[(UINT)BrushType::RED] = (HBRUSH)::CreateSolidBrush(RGB(255, 0, 0));
	m_Brushs[(UINT)BrushType::GREEN] = (HBRUSH)::CreateSolidBrush(RGB(0, 255, 0));
	m_Brushs[(UINT)BrushType::GREY] = (HBRUSH)::CreateSolidBrush(RGB(76, 76, 77));
	m_Brushs[(UINT)BrushType::MAGENTA] = (HBRUSH)::CreateSolidBrush(RGB(255, 0, 255));

	// PEN 
	m_Pens[(UINT)PenType::RED] = ::CreatePen(PS_SOLID, 1, RGB(255, 0, 0));
	m_Pens[(UINT)PenType::GREEN] = ::CreatePen(PS_SOLID, 1, RGB(0, 255, 0));
	m_Pens[(UINT)PenType::MAGENTA] = ::CreatePen(PS_SOLID, 1, RGB(255, 0, 255));

	// 폰트 등록
	RegisterFont(FontType::TITLE, L"Eulyoo1945-Regular", 0);
	RegisterFont(FontType::SKILLTEXT, L"Eulyoo1945-Regular", 18, 18);
}

void ResourceManager::ReleaseGDI()
{
	for (int i = 0; i < (UINT)PenType::END; ++i)
		::DeleteObject(m_Pens[i]);
	for (int i = 1; i < (UINT)BrushType::END; ++i)
		// Hollow 제외하고
		::DeleteObject(m_Brushs[i]);
	for (int i = 0; i < (UINT)FontType::END; ++i)
		::DeleteObject(m_Fonts[i]);
}

bool ResourceManager::RegisterFontFile(const wstring& _path)
{
	wstring fontPath = m_resourcePath;
	fontPath += _path;
	if (!(AddFontResourceExW(fontPath.c_str(), FR_PRIVATE, 0) > 0))
		return false;
	m_vecFontFiles.push_back(fontPath);
	return true;
}

void ResourceManager::ReleaseFonts()
{
	for (const auto& path : m_vecFontFiles)
		::RemoveFontResourceExW(path.c_str(), FR_PRIVATE, 0);
	m_vecFontFiles.clear();
}

void ResourceManager::RegisterFont(FontType _type, const wstring& _name, int _height, int _weight, bool _italic, int _quality)
{

	HFONT h = ::CreateFont(_height, 0, 0, 0, _weight, _italic, false, false, HANGEUL_CHARSET, OUT_DEFAULT_PRECIS, CLIP_DEFAULT_PRECIS, _quality, DEFAULT_PITCH || FF_DONTCARE, _name.c_str());
	m_Fonts[(UINT)_type] = h;
}


void ResourceManager::RegisterTexture()
{
	// texture load
	LoadTexture(L"Plane", L"Texture\\plane.bmp");
	LoadTexture(L"MiddleBullet", L"Texture\\Player_Bullet_Middle.bmp"); // 1 ~ 3번째 총알
	LoadTexture(L"AngleBullet", L"Texture\\Player_Bullet_Angle.bmp"); // 4, 5번째 총알
	LoadTexture(L"Player", L"Texture\\Player.bmp");
	LoadTexture(L"BlueBullet", L"Texture\\2_Circle_Blue_2.bmp");
	LoadTexture(L"BlueBullet3", L"Texture\\2_Circle_Blue_3.bmp");
	LoadTexture(L"IceBullet", L"Texture\\IceBullet.bmp");
	LoadTexture(L"Magic", L"Texture\\magic.bmp");
	LoadTexture(L"Background", L"Texture\\Background.bmp");
	LoadTexture(L"OraneBullet3", L"Texture\\2_Circle_Orange_3.bmp");
	LoadTexture(L"GreenBullet1", L"Texture\\2_Circle_Green_1.bmp");
	LoadTexture(L"GreenBullet3", L"Texture\\2_Circle_Green_3.bmp");
	LoadTexture(L"BlueBullet1", L"Texture\\2_Circle_Blue_1.bmp");
	LoadTexture(L"RedBullet1", L"Texture\\2_Circle_Red_1.bmp");
	LoadTexture(L"RedBullet2", L"Texture\\2_Circle_Red_2.bmp");
	LoadTexture(L"PurpleBullet1", L"Texture\\2_Circle_Purple_1.bmp");
	LoadTexture(L"GreenSword", L"Texture\\Sword_BrightGreen.bmp");
	LoadTexture(L"RedSword", L"Texture\\Sword_Red.bmp");
	LoadTexture(L"BlueSword", L"Texture\\Sword_Blue.bmp");
	LoadTexture(L"YellowSword", L"Texture\\Sword_Yellow.bmp");
	LoadTexture(L"Boss", L"Texture\\Zelretch.bmp");
	LoadTexture(L"BossBoom", L"Texture\\Boss_Circle.bmp");
	LoadTexture(L"SpellBackground", L"Texture\\SpellBackground.bmp");
	LoadTexture(L"BombIcon", L"Texture\\Player_Icon_Bomb_Green.bmp");
	LoadTexture(L"PlayerBomb", L"Texture\\Bomb.bmp");
	LoadTexture(L"LifeIcon", L"Texture\\Player_Icon_Hp_Red.bmp");
	LoadTexture(L"TitleBackground", L"Texture\\TitleBackground.bmp");
	LoadTexture(L"NormalEnemy", L"Texture\\enemy_1.bmp");
	LoadTexture(L"DownEnemy", L"Texture\\Enemy_Down.bmp");
	LoadTexture(L"Enemy_2", L"Texture\\Enemy_2.bmp");
	LoadTexture(L"Enemy_3", L"Texture\\Enemy_3.bmp");
	LoadTexture(L"PowerIcon", L"Texture\\Player_Bullet_Middle.bmp");

	LoadTexture(L"StartBtn", L"Texture\\btn_start.bmp");
	LoadTexture(L"SettingBtn", L"Texture\\btn_Setting.bmp");
	LoadTexture(L"ToBossBtn", L"Texture\\btn_ToBoss.bmp");
	LoadTexture(L"ExitBtn", L"Texture\\btn_Exit.bmp");
	LoadTexture(L"SelectIcon", L"Texture\\SelectIcon.bmp");
	LoadTexture(L"GameIcon", L"Texture\\icon.bmp");
	LoadTexture(L"UIBackground", L"Texture\\UI_Bg.bmp");
	LoadTexture(L"TitleText", L"Texture\\TitleText.bmp");
	LoadTexture(L"TitleBtn", L"Texture\\Btn_Title.bmp");
	LoadTexture(L"GameOver", L"Texture\\GameOver.bmp");
	LoadTexture(L"GameClearText", L"Texture\\GameClear.bmp");
}

void ResourceManager::LoadTexture(const wstring& _key, const wstring& _path)
{
	Texture* pTex = GetTexture(_key);
	// 찾았으면 리턴
	if (nullptr != pTex)
		return;
	// 처음에 없을거니 경로 찾아서
	wstring texPath = m_resourcePath;
	texPath += _path;

	// 만들어서
	pTex = new Texture;
	pTex->Load(texPath); // 텍스처 자체 로드
	pTex->SetKey(_key); // 키 경로 세팅
	pTex->SetRelativePath(texPath);
	m_mapTexture.insert({ _key,pTex }); // 맵에 저장
}

Texture* ResourceManager::GetTexture(const wstring& _key)
{
	auto iter = m_mapTexture.find(_key);
	if (iter != m_mapTexture.end())
		return iter->second;
	return nullptr;
}


