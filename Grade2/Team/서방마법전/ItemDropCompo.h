#pragma once
#include "Component.h"
class Item;
class Health;
class ItemDropCompo :
    public Component
{
public:
    // Component을(를) 통해 상속됨
    ~ItemDropCompo();
    void Init() override;
    void LateUpdate() override;
    void Render(HDC hDC) override;
public:
    void SpawnItem();
    void SetItem(Item* item);
private:
    Health* m_healthCompo;
    Item* m_item;
};

