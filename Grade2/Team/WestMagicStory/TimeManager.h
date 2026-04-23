#pragma once
class TimeManager
{
	DECLARE_SINGLE(TimeManager);

public:
	void Init();
	void Update();
public:
	float GetDT() const { return m_deltaTime; }
private:
	// delta time
	LARGE_INTEGER m_llPrevCnt = {};
	LARGE_INTEGER m_llFrequency = {};
	LARGE_INTEGER m_llCurCnt = {};

	float m_deltaTime = 0.f;
	
	// fps
	UINT m_fps = 0;
	UINT m_frameCnt = 0;
	float m_frameTime = 0.f;

};

