#include "pch.h"
#include "DeleteBullet.h"
#include "Collider.h"
#include "PoolManager.h"
#include "EnemyProjectile.h"

DeleteBullet::DeleteBullet()
{
	auto* col = AddComponent<Collider>();
	col->SetSize(750.f);
}

DeleteBullet::~DeleteBullet()
{
	Object::~Object();
}

void DeleteBullet::Render(HDC _hdc)
{
}

void DeleteBullet::StayCollision(Collider* _other)
{
	auto* bullet = static_cast<EnemyProjectile*>(_other->GetOwner());
	bullet->PushSelf();

}
