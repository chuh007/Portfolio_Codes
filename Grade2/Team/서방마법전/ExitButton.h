#pragma once
#include "Button.h"
class ExitButton :
    public Button
{
public:
    ~ExitButton() { Button::~Button(); };
    void OnClick() override;
};

