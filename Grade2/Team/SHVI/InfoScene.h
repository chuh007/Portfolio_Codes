#pragma once
#include"Scene.h"
#include "InputHandler.h"

class InfoScene : public Scene
{
public:
	InfoScene();
	~InfoScene();
public:
	virtual void Init() override;
	virtual void Update() override;
public:
	void RenderInfo();
private:
	InputHandler* m_inputHandler;
};

