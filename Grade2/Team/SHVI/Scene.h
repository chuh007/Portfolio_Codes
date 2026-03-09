#pragma once
#include"Enums.h"
class Scene
{
public:
	Scene();
public:
	virtual void Init() abstract;
	virtual void Update() abstract;
public:
	SCENE GetNowScene() { return nowScene; }
	void FirstLoadInit();
protected:
	SCENE nowScene;
	bool isFirstLoadScene;
};

