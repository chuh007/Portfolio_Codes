#include "pch.h"
#include "BaseWindow.h"
int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPWSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
    _CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
    {
        BaseWindow game;
        game.Run(hInstance, nCmdShow);
    }
    _CrtDumpMemoryLeaks();
}

//// 애플리케이션 초기화를 수행합니다:
//if (!InitInstance(hInstance, nCmdShow))
//{
//    return FALSE;
//}