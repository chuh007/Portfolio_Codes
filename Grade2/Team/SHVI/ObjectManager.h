#pragma once
#include "defines.h"
#include "GameObject.h"
#include <vector>

class GameObject;

class ObjectManager
{
private:
	ObjectManager();
	~ObjectManager();
public:
	static ObjectManager* GetInst()
	{
		if (m_inst == nullptr)
			m_inst = new ObjectManager;
		return m_inst;
	}
	static void DestoryInst()
	{
		SAFE_DELETE(m_inst);
	}
	void Update();
	void AddObject(GameObject* obj);
	void ObjectAllDie();
private:
	static ObjectManager* m_inst;
	std::vector<GameObject*> m_objects;
};

