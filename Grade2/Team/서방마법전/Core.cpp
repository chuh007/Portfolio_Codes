#include "pch.h"
#include "Core.h"
#include "TimeManager.h"
#include "InputManager.h"
#include "SceneManager.h"
#include "ResourceManager.h"
#include "CollisionManager.h"
#include "BulletRenderManager.h"
#include "EnemySpawnManger.h"
#include "Texture.h"
bool Core::Init(HWND _hWnd)
{
    m_hWnd = _hWnd;
    m_hDC = ::GetDC(m_hWnd);
    //m_obj.SetPos({WINDOW_WIDTH / 2, WINDOW_HEIGHT / 2});
    //m_obj.SetSize({ 100,100 });
    m_hBackBit = 0;
    m_hBackDC = 0;

    // 더블버퍼링
    // 1. 생성
    m_hBackBit = ::CreateCompatibleBitmap(m_hDC, WINDOW_WIDTH, WINDOW_HEIGHT);
    m_hBackDC = ::CreateCompatibleDC(m_hDC);
    GET_SINGLE(BulletRenderManager)
        ->SetBulletBitMap(::CreateCompatibleBitmap(m_hDC, WINDOW_HEIGHT, WINDOW_HEIGHT));
    GET_SINGLE(BulletRenderManager)
        ->SetBulletDC(::CreateCompatibleDC(m_hDC));
    // 2. 연결
    ::SelectObject(m_hBackDC, m_hBackBit);

    // 1
    GET_SINGLE(TimeManager)->Init();
    // 2
    GET_SINGLE(InputManager)->Init();
    // 3
    GET_SINGLE(BulletRenderManager)->Init();
 
    if (!GET_SINGLE(ResourceManager)->Init())
        return false;
    m_backgroundTex = GET_SINGLE(ResourceManager)->GetTexture(L"Background");
    GET_SINGLE(SceneManager)->Init();

    return true;
}

void Core::MainUpdate()
{
    GET_SINGLE(TimeManager)->Update();
    {
        static float accmulator = 0.f;
        const float fixedDT = 1.f / 60.f;
        accmulator += fDT;
        while (accmulator >= fixedDT)
        {
            GET_SINGLE(SceneManager)->FixedUpdate(fixedDT);
            GET_SINGLE(CollisionManager)->Update();
            accmulator -= fixedDT;
        }
    }
    GET_SINGLE(InputManager)->Update();
    GET_SINGLE(ResourceManager)->FmodUpdate();
    GET_SINGLE(SceneManager)->Update();

    //Vec2 pos = m_obj.GetPos();
    //
    ////if (GetAsyncKeyState(VK_LEFT) & 0x8000)
    ////if(GET_SINGLE(InputManager)->IsPress(KEY_TYPE::LEFT))
    //if(GET_KEY(KEY_TYPE::LEFT))
    //    pos.x -= 200.f * fDT;
    ////if (GetAsyncKeyState(VK_RIGHT) & 0x8000)
    ////if (GET_SINGLE(InputManager)->IsUp(KEY_TYPE::RIGHT))
    //if(GET_KEYUP(KEY_TYPE::RIGHT))
    //    pos.x += 200.f;// *fDT;
    //if(GET_KEYWIDTH(KEY_TYPE::CTRL, KEY_TYPE::SPACE))
    //    pos.y += 200.f * fDT;// *fDT;
    //m_obj.SetPos(pos);

}


void Core::MainRender()
{
    //::Rectangle(m_hBackDC, -1, -1, WINDOW_WIDTH +1 , WINDOW_HEIGHT +1 );

    // 1. clear
    ::PatBlt(m_hBackDC, 0,0, GAME_WIDTH, GAME_HEIGHT, WHITENESS);
    GDISelector hBrush(GET_SINGLE(BulletRenderManager)->GetBulletDC(), BrushType::MAGENTA);
    ::PatBlt(GET_SINGLE(BulletRenderManager)->GetBulletDC(), 0, 0, GAME_WIDTH, GAME_HEIGHT, PATCOPY);
    
    //Vec2 pos = m_obj.GetPos();
    //Vec2 size = m_obj.GetSize();
    //RECT_RENDER(m_hBackDC, pos.x, pos.y, size.x, size.y);
    
    // 배경처리
    //::StretchBlt(m_hBackgroundDC
    //    , 0, 0
    //    , GAME_WIDTH, GAME_HEIGHT
    //    , m_backgroundTex->GetTextureDC()
    //    , 0, 0,
    //    m_backgroundTex->GetWidth(),
    //    m_backgroundTex->GetHeight(),
    //    SRCCOPY);

    // 2. draw
    GET_SINGLE(SceneManager)->Render(m_hBackDC);

    // 3. display
    //::TransparentBlt(
    //    m_hDC, 0, 0, GAME_WIDTH, GAME_HEIGHT,
    //    m_hBackDC, 0, 0, GAME_WIDTH, GAME_HEIGHT,
    //    RGB(255, 0, 255));

    //::TransparentBlt(
    //    m_hBackgroundDC, 0, 0, GAME_WIDTH, GAME_HEIGHT,
    //    m_hBackDC, 0, 0, GAME_WIDTH, GAME_HEIGHT,
    //    RGB(255, 0, 255));

    ::BitBlt(m_hDC, 0, 0, GAME_WIDTH, GAME_HEIGHT, m_hBackDC, 0, 0, SRCCOPY);

    ::BitBlt(m_hDC,
        GAME_WIDTH, 0,
        UI_WIDTH, WINDOW_HEIGHT,
        m_hBackDC, GAME_WIDTH, 0,
        SRCCOPY); 
}


void Core::GameLoop()
{
    INT A = 0;
    //static int cnt = 0;
    //++cnt;
    //static int prev = GetTickCount64();
    //int cur = GetTickCount64();
    //if (cur - prev >= 1000)
    //{
    //    prev = cur;
    //    cnt = 0;
    //}

    MainUpdate();
    MainRender();
    GET_SINGLE(SceneManager)->GetCurScene()->FlushEvent();
 }

void Core::CleanUp()
{
    ::DeleteObject(m_hBackBit);
    ::DeleteDC(m_hBackDC);
    ::ReleaseDC(m_hWnd, m_hDC);
    GET_SINGLE(ResourceManager)->Release();
    GET_SINGLE(EnemySpawnManger)->Realese();
    GET_SINGLE(SceneManager)->GetCurScene()->Release();
}


