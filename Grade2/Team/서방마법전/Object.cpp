#include "pch.h"
#include "Object.h"
#include "InputManager.h"
#include "Component.h"
Object::Object()
	: m_pos{}
	, m_size{}
	, m_isDie(false)
	, m_isActive(true)
{
	
}

Object::~Object()
{
	// 컴포넌트 삭제
	for (Component* com : m_vecComponents)
		SAFE_DELETE(com);
	m_vecComponents.clear();
}

void Object::Update()
{
	if (m_corutines.size() == 0) return;
	for (auto iter = m_corutines.begin(); iter != m_corutines.end();)
	{
		iter->second -= fDT;
		if (iter->second <= 0)
		{
			iter->first();
			iter = m_corutines.erase(iter);
			continue;
		}
		iter++;
	}
}

void Object::LateUpdate()
{
	for (Component* com : m_vecComponents)
	{
		if (com != nullptr)
			com->LateUpdate();
	}
}
void Object::ComponentRender(HDC _hdc)
{
	for (Component* com : m_vecComponents)
	{
		if (com != nullptr)
			com->Render(_hdc);
	}
}

void Object::Coroutine(std::function<void()> func, float delay)
{
	m_corutines.push_back({ func, delay });
}
