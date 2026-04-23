#include "pch.h"
#include "BakBakDevScene.h"
#include "Object.h"
#include "Player.h"
#include "InputManager.h"
#include "SceneManager.h"
#include "Boss.h"
#include "CollisionManager.h"
#include "ResourceManager.h"
#include "EnemyProjectile.h"
#include "EnemySpawnManger.h"
#include "PoolManager.h"
#include "TestEnemy.h"
#include "CircleMoveEnemy.h"
#include "TripleShotEnemy.h"
#include "PlayerManager.h"
void BakBakDevScene::Init()
{
	Player* obj = new Player;
	obj->SetPos({ WINDOW_WIDTH / 2, 300 });
	obj->SetSize({ 100.f, 100.f });
	// obj->SetScene(this);
	AddObject(obj, Layer::PLAYER);

	GET_SINGLE(PlayerManager)->SetPlayer(obj);
	//Spawn<Boss>(Layer::ENEMY, { WINDOW_WIDTH / 2, WINDOW_HEIGHT / 4 }, { 100.f,100.f });
	GET_SINGLE(CollisionManager)->CheckLayer(Layer::PROJECTILE, Layer::ENEMY);
	GET_SINGLE(CollisionManager)->CheckLayer(Layer::PLAYER, Layer::DEFAULT);
	GET_SINGLE(ResourceManager)->Play(L"BGM");

	GET_SINGLE(EnemySpawnManger)->Init();
	GET_SINGLE(PoolManager)->AddPool<EnemyProjectile>
		(PoolType::EnemyProjectile, 100, Layer::ENEMYPROJECTILE);
	GET_SINGLE(PoolManager)->AddPool<PlayerProjectile>
		(PoolType::PlayerProj, 100, Layer::PROJECTILE );
	
	TestEnemy* testEnemy = new TestEnemy;
	CircleMoveEnemy* circleEnemy = new CircleMoveEnemy;
	TripleShotEnemy* tripleshot = new TripleShotEnemy;
	testEnemy->SetPos({ 100, 100 });
	testEnemy->SetSize({ 75,75 });
	circleEnemy->SetPos({ 100, 100 });
	circleEnemy->SetSize({ 50,50 });
	tripleshot->SetPos({ 100,100 });
	tripleshot->SetSize({ 100,100 });
	GET_SINGLE(EnemySpawnManger)->SetSpawnScene(this);
	GET_SINGLE(EnemySpawnManger)->AddEnemySpawnQueue({ 3.f, testEnemy });
	GET_SINGLE(EnemySpawnManger)->AddEnemySpawnQueue({ 6.f, circleEnemy });
	GET_SINGLE(EnemySpawnManger)->AddEnemySpawnQueue({ 9.f, tripleshot });
}

void BakBakDevScene::Update()
{
	GET_SINGLE(EnemySpawnManger)->Update();
	Scene::Update();
	if (GET_KEYDOWN(KEY_TYPE::ENTER))
		GET_SINGLE(SceneManager)->LoadScene(L"TestScene");
}
