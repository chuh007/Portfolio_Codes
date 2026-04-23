#include "pch.h"
#include "SceneManager.h"
#include "DevScene.h"
#include "TestScene.h"
#include "Collider.h"
#include "Object.h"
#include "BakBakDevScene.h"
#include "TitleScene.h"
#include "GameScene.h"
#include "GameOverScene.h"
#include "ClearScene.h"
#include "SettingScene.h"
void SceneManager::Init()
{
	m_curScene = nullptr;
	
	// 등록
	RegisterScene(L"DevScene", std::make_shared<DevScene>());
	RegisterScene(L"TestScene", std::make_shared<TestScene>());
	RegisterScene(L"BakBakDev", std::make_shared<BakBakDevScene>());
	RegisterScene(L"Game", std::make_shared<GameScene>());
	RegisterScene(L"GameOver", std::make_shared<GameOverScene>());
	RegisterScene(L"GameClear", std::make_shared<ClearScene>());
	RegisterScene(L"Title", std::make_shared<TitleScene>());
	RegisterScene(L"Setting", std::make_shared<SettingScene>());
	// Scene 추가
	
	// 로드
	LoadScene(L"Title");

	// todo
	//dynamic_cast<> 
	//std::dynamic_pointer_cast<>
}

void SceneManager::RegisterScene(const wstring& _name, std::shared_ptr<Scene> _scene)
{
	if (_name.empty() || _scene == nullptr)
		return;
	m_mapScene.insert(m_mapScene.end(), { _name, _scene });
}

void SceneManager::Update()
{
	if (m_curScene == nullptr)
		return;
	m_curScene->Update();
	m_curScene->LateUpdate();
}

void SceneManager::FixedUpdate(float _fixedDT)
{
	// 여기부터
	if (m_curScene == nullptr)
		return;
	m_curScene->FixedUpdate(_fixedDT);
	PhysicsSyncColliders();
}
void SceneManager::PhysicsSyncColliders()
{
	for (UINT i = 0; i < (UINT)Layer::END; ++i)
	{
		const auto& objects = m_curScene->GetLayerObjects((Layer)i);
		for (Object* obj : objects)
		{
			if (!obj)
				continue;

			if (auto* col = obj->GetComponent<Collider>())
				col->LateUpdate(); // sync  
		}
	}
}
void SceneManager::Render(HDC _hdc)
{
	if (m_curScene == nullptr)
		return;
	m_curScene->Render(_hdc);
}


void SceneManager::LoadScene(const wstring& _name)
{
	// change
	if (m_curScene != nullptr)
	{
		m_curScene->Release();
		m_curScene = nullptr;
	}

	auto iter = m_mapScene.find(_name);
	if (iter != m_mapScene.end())
	{
		m_curScene = iter->second;
		m_curScene->Init();
	}
}
