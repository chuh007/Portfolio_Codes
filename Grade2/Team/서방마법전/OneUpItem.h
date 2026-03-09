#pragma once
#include "Item.h"
class OneUpItem :
    public Item
{
public:
	OneUpItem();
    ~OneUpItem();
    // Item을(를) 통해 상속됨
    void OnCollect() override;
	void Render(HDC _hdc) override;
};

