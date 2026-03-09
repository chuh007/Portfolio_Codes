#include "pch.h"
#include "PlayerBoom.h"
#include "ResourceManager.h"
#include "Texture.h"
#include "EnemyProjectile.h"

PlayerBoom::PlayerBoom()
{
	m_pTex = GET_SINGLE(ResourceManager)->GetTexture(L"PlayerBomb");
	auto* col = AddComponent<Collider>();
	col->SetSize(125.f);
}

PlayerBoom::~PlayerBoom()
{
}

void PlayerBoom::Render(HDC _hdc)
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

void PlayerBoom::Update()
{
	Object::Update();
	Translate({ 0.f, -100.f * fDT });
}

void PlayerBoom::EnterCollision(Collider* _other)
{
	auto* bullet = static_cast<EnemyProjectile*>(_other->GetOwner());
	bullet->PushSelf();
}
