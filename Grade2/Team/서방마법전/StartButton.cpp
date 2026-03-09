#include "pch.h"
#include "SceneManager.h"
#include "StartButton.h"

void StartButton::OnClick()
{
	wstring sceneName = m_SceneName;
	GET_SINGLE(SceneManager)->LoadScene(sceneName);
}

void StartButton::SetSceneName(const wstring name)
{
	m_SceneName = name;
}
