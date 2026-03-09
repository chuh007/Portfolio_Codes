#pragma once
// 외부에서 생성 가능? X
// inst 부를 때마다 생성되어야 해? 아니면 일치되어야해? => 똑같은 놈
// 언제든지 부를 수 있어야돼? => #include 
//#include "Defines.h"
//#include "Object.h"
class Texture;
class Core
{
	DECLARE_SINGLE(Core);
public:
	bool Init(HWND _hWnd);
	void GameLoop();
	void CleanUp();
public:
	const HDC& GetMainDC() const { return m_hDC; }
	const HWND& GetHwnd() const { return m_hWnd; }
private:
	void MainUpdate();
	void MainRender();
private:
	HDC  m_hDC;
	HWND m_hWnd;
	//Object m_obj;

	HBITMAP m_hBackBit; 
	HDC  m_hBackDC;

	Texture* m_backgroundTex;
};


//DECLARE_SINGLE(Core);
//private:
//	Core() {}
//public:
//	static Core* GetInst()
//	{
//		static Core inst;
//		return &inst;
//	}

//#include "bullet.h"
//class Player;
//class Core
//{
//private:
//	Core() {}
//public:
//	static Core* GetInst()
//	{
//		if (nullptr == m_pInst)
//			m_pInst = new Core;
//		return m_pInst;
//	}
//	static void DestroyInst()
//	{
//
//	}
//private:
//	Bullet m_bullet;
//	Player* m_player;
//	static Core* m_pInst;
//};
