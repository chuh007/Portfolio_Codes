#include "ObjectManager.h"
#include "GameObject.h"
#include "Console.h"
ObjectManager* ObjectManager::m_inst = __nullptr;
ObjectManager::ObjectManager()
{

}
ObjectManager::~ObjectManager()
{
	for (auto& e : m_objects)
	{
		SAFE_DELETE(e);
	}
}

void ObjectManager::Update()
{
    for (int i = 0; i < m_objects.size(); ++i)
    {
        if (m_objects[i]->isDead)
        {
            m_objects[i]->OnDestroy();
            SAFE_DELETE(m_objects[i]);
            m_objects.erase(m_objects.begin() + i);
            continue;
        }

        m_objects[i]->Update();
        m_objects[i]->Render();
    }
}


void ObjectManager::AddObject(GameObject* obj)
{
	m_objects.push_back(obj);
}

void ObjectManager::ObjectAllDie()
{
    int i = 0;
    for (auto item : m_objects)
    {
        item->OnDestroy();
        SAFE_DELETE(item);
    }
    m_objects.clear();
}

