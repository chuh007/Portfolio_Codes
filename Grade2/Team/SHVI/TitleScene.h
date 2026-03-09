#pragma once
#include "Scene.h"
#include "Position.h"
#include "InputHandler.h"


class TitleScene : public Scene
{
public:
	TitleScene();
	~TitleScene();
public:
	virtual void Init() override;
	virtual void Update() override;
private:
	void KeyCheck();
	void RenderTitle();

private:
	Position resolution;
	Menu eMenu;
	InputHandler* m_inputHandler;
};

