#pragma once
class Object;
class Component
{
public:
	Component();
	virtual ~Component();
public:
	virtual void Init() abstract;
	virtual void LateUpdate() abstract;
	virtual void Render(HDC hDC) abstract;
public:
	void SetOwner(Object* owner) { m_owner = owner; }
	Object* GetOwner() const { return m_owner; }
protected:
	Object* m_owner;
};


