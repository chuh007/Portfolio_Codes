#pragma once
#include "Object.h"
#include "PoolManager.h"
class Effect :
    public Object, IPoolable
{
public:
    Effect();
    ~Effect();

public:
    // Object을(를) 통해 상속됨
    void Render(HDC _hdc) override;
    // IPoolable을(를) 통해 상속됨
    void Reset() override;

private:
    Texture* m_pTex;

};

