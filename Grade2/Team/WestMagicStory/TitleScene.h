#pragma once
#include "Scene.h"
class TitleScene : public Scene
{
public:
    ~TitleScene();
	// Scene을(를) 통해 상속됨
	void Init() override;
	void Render(HDC _hdc) override;
    void Release() override;

private:

    HDC m_hdc = nullptr;
    HBITMAP m_hUIBitmap = nullptr;
    HBITMAP m_hOldBitmap = nullptr;
    int m_uiWidth = 0;
    int m_uiHeight = 0;
};

