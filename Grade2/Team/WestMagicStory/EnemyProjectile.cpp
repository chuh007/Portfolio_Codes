#include "pch.h"
#include "EnemyProjectile.h"
#include "Texture.h"
#include "ResourceManager.h"
#include "Collider.h"
#include "IDamageable.h"
#include "SceneManager.h"
#include "BulletRenderManager.h"

EnemyProjectile::EnemyProjectile()
	: m_angle(0.f)
	, m_dir(0.f, 0.f)
	, m_speed(500.f)
{
	m_pTex = GET_SINGLE(ResourceManager)->GetTexture(L"BlueBullet");
	auto* col = AddComponent<Collider>();
}

EnemyProjectile::~EnemyProjectile()
{
}

void EnemyProjectile::Reset()
{
	m_corutines.clear();
	GetComponent<Collider>()->SetActive(true);
}

void EnemyProjectile::SetColliderSize(float _size)
{
	auto* col = GetComponent<Collider>();
	col->SetSize(_size);
}

void EnemyProjectile::PushSelf()
{
	GetComponent<Collider>()->SetActive(false);
	GET_SINGLE(PoolManager)->Push<EnemyProjectile>(PoolType::EnemyProjectile, this);
}

void EnemyProjectile::SetTexture(Texture* _texture)
{
	m_pTex = _texture;
}

void EnemyProjectile::Update()
{
	Object::Update();
	Vec2 pos = GetPos();
	SetPos(pos + m_dir * m_speed * fDT);
	if (GetPos().x < -200 || GAME_WIDTH + 200 < GetPos().x ||
		GetPos().y < -200 || GAME_HEIGHT + 200 < GetPos().y)
	{
		PushSelf();
	}
}


// È¸Àü °ø½Ä
// x = x*cos - y*sin
// y = x*sin + y*cos
void EnemyProjectile::Render(HDC _hdc)
{
	
	if (m_pTex == nullptr) return;

	Vec2 pos = GetPos();
	Vec2 size = GetSize();

	LONG width = m_pTex->GetWidth();
	LONG height = m_pTex->GetHeight();

	float angle = atan2(m_dir.y, m_dir.x);
	angle += PI / 2;
	float cosA = cosf(angle);
	float sinA = sinf(angle);

	POINT vertices[3];

	float hW = size.x * 0.5f;
	float hH = size.y * 0.5f;

	//ÁÂ»ó
	vertices[0].x = (LONG)(pos.x + (-hW * cosA - -hH * sinA));
	vertices[0].y = (LONG)(pos.y + (-hW * sinA + -hH * cosA));

	//¿ì»ó
	vertices[1].x = (LONG)(pos.x + (hW * cosA - -hH * sinA));
	vertices[1].y = (LONG)(pos.y + (hW * sinA + -hH * cosA));
	
	//ÁÂÇÏ
	vertices[2].x = (LONG)(pos.x + (-hW * cosA - hH * sinA));
	vertices[2].y = (LONG)(pos.y + (-hW * sinA + hH * cosA));

	::PlgBlt(GET_SINGLE(BulletRenderManager)->GetBulletDC(), vertices, m_pTex->GetTextureDC(),
		0, 0, width, height,
		NULL, 0, 0);

	ComponentRender(GET_SINGLE(BulletRenderManager)->GetBulletDC());
}

void EnemyProjectile::EnterCollision(Collider* _other)
{
	IDamageable* damageable = dynamic_cast<IDamageable*>(_other->GetOwner());
	if (damageable)
	{
		damageable->TakeDamage(1);
	}
}
