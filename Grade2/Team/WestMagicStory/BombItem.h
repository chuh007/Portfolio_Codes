#pragma once
#include "Item.h"
class BombItem :
    public Item
{
public:
    BombItem();
    ~BombItem();
    void OnCollect() override;
	void Render(HDC _hdc) override;
};

