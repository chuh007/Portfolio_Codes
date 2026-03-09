#include "Scene.h"

Scene::Scene()
	: isFirstLoadScene(true)
{
}

void Scene::Init()
{
}

void Scene::Update()
{
}

void Scene::FirstLoadInit()
{
	if (isFirstLoadScene)
	{
		Init();
		isFirstLoadScene = false;
	}
}
