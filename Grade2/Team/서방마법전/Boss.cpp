#include "pch.h"
#include "Boss.h"
#include "Health.h"
#include "Collider.h"
#include "BossMover.h"
#include "PatternCompo.h"
#include "SceneManager.h"
#include "ResourceManager.h"
#include "Texture.h"
#include "PlayerManager.h"
#include "BoomEffect.h";
#include "PowerItem.h"
Boss::Boss()
	: m_isDie(false)
	, m_lifeCount(5)
	, m_decDamage(1.f)
	, m_target(nullptr)
	, m_backGround(nullptr)
{
	m_pTex = GET_SINGLE(ResourceManager)->GetTexture(L"Boss");
	auto* col = AddComponent<Collider>();
	col->SetSize(40.f);
	m_target = GET_SINGLE(PlayerManager)->GetPlayer();
	m_healthCompo = AddComponent<Health>();
	m_healthCompo->SetMaxHP(5000);
	m_healthCompo->SetCurrentHP(5000);
	auto* mover = AddComponent<BossMover>();

	m_patternCompo = AddComponent<PatternCompo>();
	m_patternCompo->ResizePattenList(m_lifeCount + 1);
	m_patternCompo->SetOwner(this);
	m_patternCompo->SetTarget(m_target);
	m_patternCompo->SetMover(mover);
	m_patternCompo->SetUpPattern();
}

Boss::~Boss()
{
}


void Boss::Start()
{
	m_patternCompo->UseNomalPattern();
	GET_SINGLE(ResourceManager)->Stop(SOUND_CHANNEL::BGM);
	GET_SINGLE(ResourceManager)->Play(L"BossBGM");
}


void Boss::Update()
{
	if (m_isDie) return;
	Object::Update();
}

void Boss::Render(HDC _hdc)
{
	Vec2 pos = GetPos();
	Vec2 size = GetSize();
	LONG width = m_pTex->GetWidth();
	LONG height = m_pTex->GetHeight();
	::TransparentBlt(_hdc
		, (int)(pos.x - size.x / 2)
		, (int)(pos.y - size.y / 2)
		, size.x
		, size.y
		, m_pTex->GetTextureDC()
		, 0, 0, width, height,
		RGB(255, 0, 255));

	ComponentRender(_hdc);
}

void Boss::EnterCollision(Collider* _other)
{
}

void Boss::TakeDamage(int _damage)
{
	m_healthCompo->TakeDamage(_damage * m_decDamage);
	if (m_isDie) return;
	cout << _damage << '\n';
	if (m_patternCompo->IsUseSpell())
		return;
	if(m_healthCompo->GetHP() <= m_healthCompo->GetMaxHP() * 0.5f)
	{
		m_patternCompo->UseSpellPattern();
		m_decDamage = m_patternCompo->GetCurrentPattern()->GetDecValue();
		GetComponent<BossMover>()->MoveTo({ GAME_WIDTH / 2, GAME_HEIGHT / 4 }, 0.25f);
	}
}

void Boss::HPZero()
{
	m_lifeCount--;
	if (m_lifeCount > 0)
	{
		m_healthCompo->SetCurrentHP(m_healthCompo->GetMaxHP());
		m_patternCompo->UseNomalPattern();
		m_decDamage = m_patternCompo->GetCurrentPattern()->GetDecValue();
		auto* effect = GET_SINGLE(SceneManager)->GetCurScene()
			->Spawn<BoomEffect>(Layer::PROJECTILEDELETER, GetPos(), { 25,25 });
		effect->DoScale(10.f, 0.3f);
		for (int i = 0; i < 20; ++i)
		{
			Vec2 spawnPos = GetPos();
			spawnPos += { rand() % 200 - 100, rand() % 200 - 100 };
			GET_SINGLE(SceneManager)->GetCurScene()
				->Spawn<PowerItem>(Layer::ITEM, spawnPos, { 25,25 });
		}
	}
	else
	{
		auto* effect = GET_SINGLE(SceneManager)->GetCurScene()
			->Spawn<BoomEffect>(Layer::PROJECTILEDELETER, GetPos(), { 25,25 });
		effect->DoScale(20.f, 0.5f);
		m_patternCompo->DeleteProjectile();
		m_isDie = true;
		m_backGround->ChangeScene();
		GET_SINGLE(SceneManager)->RequestDestroy(this);
	}
}
