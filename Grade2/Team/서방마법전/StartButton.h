#pragma once
#include "Button.h"
class StartButton :
    public Button
{
public: 
    ~StartButton() {};
    void OnClick() override;
    void SetSceneName(const wstring name);
private:
    wstring m_SceneName;
};

