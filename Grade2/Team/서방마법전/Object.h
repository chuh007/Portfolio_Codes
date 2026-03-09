#pragma once
#include "Collider.h"
#include <functional>
class Component;

class Object
{
public:
	Object();
	virtual ~Object(); // 가상 소멸자 
public:
	virtual void Update();
	virtual void LateUpdate();
	void ComponentRender(HDC _hdc);
	virtual void Render(HDC _hdc) abstract;
	virtual void EnterCollision(Collider* _other) {}
	virtual void StayCollision(Collider* _other) {}
	virtual void ExitCollision(Collider* _other) {}
	void Coroutine(std::function<void()> func, float delay);
public:
	void SetPos(Vec2 _pos) { m_pos = _pos; }
	void SetSize(Vec2 _size) { m_size = _size; }
	const Vec2& GetPos() const { return m_pos; }
	const Vec2& GetSize()const { return m_size; }
	bool GetIsDead() const { return m_isDie; }
	void SetDead() { m_isDie = true; }
	bool IsActive() const { return m_isActive; }
	void SetActive(bool _active) { m_isActive = _active; }
protected:
	void Translate(Vec2 _delta)
	{
		m_pos.x += _delta.x;
		m_pos.y += _delta.y;
	}
	void Scale(Vec2 _s)
	{
		m_size.x *= _s.x;
		m_size.y *= _s.y;
	}
public:
	template<typename T>
	T* AddComponent()
	{
		static_assert(std::is_base_of<Component, T>::value, "Component로부터 상속받아야 합니다.");
		T* compo = new T;
		compo->SetOwner(this); // 주인 찾기
		// 자기 자신의 기본 세팅 완료
		// setowner하고 이 owner로 getcom를 하던지 할 수 있음
		compo->Init();
		m_vecComponents.push_back(compo);
		return compo; // 리턴값을 나중에 setter 등 유연하게 사용
	}
	template<typename T>
	T* GetComponent()
	{
		T* component = nullptr;
		for (Component* com : m_vecComponents)
		{
			component = dynamic_cast<T*>(com);
			if (component)
				break;
		}
		return component;
	}
private:
	bool m_isActive;
	bool m_isDie;
	Vec2 m_pos;
	Vec2 m_size;
	vector<Component*> m_vecComponents;
	protected:
	vector<std::pair<std::function<void()>, float>> m_corutines;
};

