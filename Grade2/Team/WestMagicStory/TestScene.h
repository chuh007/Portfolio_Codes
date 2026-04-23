#pragma once
#include "Scene.h"
class TestScene :
    public Scene
{
public:
    // Scene을(를) 통해 상속됨
    void Init() override;
};
// DevScene에서 Enter 누르면 TestScene으로 넘어감
// TestScene에는 Enemy 100마리가 랜덤 위치의 랜덤색상으로
// 계속 반짝임.


