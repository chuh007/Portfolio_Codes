#include "pch.h"
#include "PatternCompo.h"
#include "SceneManager.h"
#include "DeleteBullet.h"
#include "Boss.h"
#include "CirclePattern.h"
#include "CircleToPlayerPattern.h"
#include "IcicleFallPattern.h"
#include "GateOfBabylonPattern.h"
#include "PlayerManager.h"
#include "SpiralPattern.h";
#include "PureBulletHellPattern.h"
#include "SecondMagicPattern.h"
#include "MultiSpeedRadialPattern.h";
#include "TripleCirclePattern.h";

PatternCompo::PatternCompo()
	: m_curPattern(nullptr)
	, m_spellNameText(nullptr)
	, m_usingPattern(false)
	, m_isUseSpell(false)
	, m_phase(0)
	, m_target(nullptr)
	, m_mover(nullptr)
{
}

PatternCompo::~PatternCompo()
{
	for (auto* pattern : m_nomalPatternList)
	{
		SAFE_DELETE(pattern);
	}
	m_nomalPatternList.clear();
	for (auto* pattern : m_spellPatternList)
	{
		SAFE_DELETE(pattern);
	}
	m_spellPatternList.clear();
}

void PatternCompo::Init()
{
	m_spellNameText = GET_SINGLE(SceneManager)->GetCurScene()
		->Spawn<SpellNameText>(Layer::UI, { 0,0 }, { 10.f,10.f });
}

void PatternCompo::LateUpdate()
{
	if (m_curPattern == nullptr) return;
	m_curPattern->Update();
}

void PatternCompo::Render(HDC hDC)
{
}

void PatternCompo::UseNomalPattern()
{
	DeleteProjectile();
	auto owner = dynamic_cast<Boss*>(GetOwner());
	owner->GetBackground()
		->SetTexture(GET_SINGLE(ResourceManager)->GetTexture(L"Background"));
	GET_SINGLE(ResourceManager)->Play(L"SpellEndSound");
	m_phase++;
	m_curPattern = m_nomalPatternList[m_phase];
	m_isUseSpell = false;
	auto* text = m_spellNameText;
	text->MoveTo({ GAME_WIDTH + 20, 25 }, 0.25f);
}

void PatternCompo::UseSpellPattern()
{
	DeleteProjectile();
	GET_SINGLE(ResourceManager)->Play(L"SpellSound");
	auto owner = dynamic_cast<Boss*>(GetOwner());
	owner->GetBackground()
		->SetTexture(GET_SINGLE(ResourceManager)->GetTexture(L"SpellBackground"));
	m_curPattern = m_spellPatternList[m_phase];
	m_isUseSpell = true;
	auto* text = m_spellNameText;
	int textSize = m_curPattern->GetName().size();
	text->SetName(m_curPattern->GetName());
	text->SetPos({ GAME_WIDTH + 20, GAME_HEIGHT - 50 });
	text->MoveTo({ GAME_WIDTH - 18 * textSize, GAME_HEIGHT - 50 }, 0.5f);
	text->Coroutine([=]()
		{
			text->MoveTo({ GAME_WIDTH - 18 * textSize, 25 }, 1.f);
		}, 0.75f);
	
}

void PatternCompo::DeleteProjectile()
{
	auto* obj = GET_SINGLE(SceneManager)->GetCurScene()
		->Spawn<DeleteBullet>(Layer::PROJECTILEDELETER,
			{ GAME_WIDTH / 2 , GAME_HEIGHT / 2 },
			{ 500,500 });
	obj->Coroutine([=]()
		{
			GET_SINGLE(SceneManager)->GetCurScene()->RequestDestroy(obj);
		}, 0.2f);
}

void PatternCompo::SetUpPattern()
{
	auto* pattern1 = new CirclePattern(m_owner, m_target, 1.2f, m_mover, L"");
	AddNomalPattern(1, pattern1);
	auto* spell1 = new CircleToPlayerPattern(m_owner, m_target, 0.75f, m_mover, L"구속「부여잡는 올가미」 ");
	AddSpellPattern(1, spell1);
	auto* pattern2 = new TripleCirclePattern(m_owner, m_target, 0.9f, m_mover, L"");
	AddNomalPattern(2, pattern2);
	auto* spell2 = new IcicleFallPattern(m_owner, m_target, 0.5f, m_mover, L"빙설「아이시클 폴」 ");
	AddSpellPattern(2, spell2);
	auto* pattern3 = new MultiSpeedRadialPattern(m_owner, m_target, 1.5f, m_mover, L"");
	AddNomalPattern(3, pattern3);
	auto* spell3 = new GateOfBabylonPattern(m_owner, m_target, 0.7f, m_mover, L"보구「게이트 오브 바빌론」 ");
	AddSpellPattern(3, spell3);
	auto* pattern4 = new CirclePattern(m_owner, m_target, 0.7f, m_mover, L"");
	AddNomalPattern(4, pattern4);
	auto* spell4 = new SecondMagicPattern(m_owner, m_target, 2.5f, m_mover, L"제 2마법「보석검 젤레치」 ");
	AddSpellPattern(4, spell4);
	auto* pattern5 = new SpiralPattern(m_owner, m_target, 1.9f, m_mover, L"");
	AddNomalPattern(5, pattern5);
	auto* spell5 = new PureBulletHellPattern(m_owner, m_target, 0.5f, m_mover, L"종막「순수한 탄막 지옥」 ");
	AddSpellPattern(5, spell5);
}
