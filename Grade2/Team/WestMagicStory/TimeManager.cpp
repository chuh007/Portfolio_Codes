#include "pch.h"
#include "TimeManager.h"
#include "Core.h"
#include "InputManager.h"
void TimeManager::Init()
{
	//time(NULL);
	//clock();
	
	// 현재 카운트의 틱을 가져온다.
	::QueryPerformanceCounter(&m_llPrevCnt);

	// 초당 카운트 1000만을 반환
	::QueryPerformanceFrequency(&m_llFrequency);
}

void TimeManager::Update()
{
	::QueryPerformanceCounter(&m_llCurCnt);
	m_deltaTime = 
		(float)(m_llCurCnt.QuadPart - m_llPrevCnt.QuadPart)
		/ (float)m_llFrequency.QuadPart;
	m_llPrevCnt = m_llCurCnt;

	// 이번프레임(이전 - 현재)에 몇초가 흘렀는가

	// 1초에 몇프레임이 흘러가는가
	//1.f / m_deltaTime;
	m_frameCnt++;
	m_frameTime += m_deltaTime;
	if (m_frameTime >= 1.f)
	{
		m_fps = (UINT)(m_frameCnt / m_frameTime);
		m_frameTime = 0.f;
		m_frameCnt = 0;
		//wstring strDt = std::to_wstring(m_deltaTime);
		//wstring strFps = std::to_wstring(m_fps);
		//wstring str = L"FPS: " + strFps + L" DT: " + strDt;
		//::TextOut(GET_SINGLE(Core)->GetMainDC(),0,0, str.c_str(), str.length());
		POINT mousepos = GET_MOUSEPOS;
		wstring str =  std::format(L"FPS: {0}, DT: {1:.6f}, Mouse: {2}, {3} ",m_fps, m_deltaTime,mousepos.x, mousepos.y);
		::SetWindowText(GET_SINGLE(Core)->GetHwnd(), str.c_str());
	}

}
