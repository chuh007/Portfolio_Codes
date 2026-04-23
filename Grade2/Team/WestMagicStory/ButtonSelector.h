#pragma once
#include "Object.h"
class Texture;
class Button;
class ButtonSelector : public Object
{
public:
	ButtonSelector();
	~ButtonSelector();
	// Object을(를) 통해 상속됨
	void Render(HDC _hdc) override;
	void Update() override;

public:
	void AssignButton(Button* button);
private:
	void MoveToCurrentSelect();
private:
	vector<Button*> btns;
	int curSelectedIdx;
	float m_lastSelectTime;
	bool m_isActive;
	Texture* m_pTexture;
};

