#pragma once
#include "Item.h"
class PowerItem :
    public Item
{
public:
    PowerItem();
    ~PowerItem();
    // Item을(를) 통해 상속됨
    void OnCollect() override;
    void Render(HDC _hdc) override;
};

