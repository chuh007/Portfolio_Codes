#pragma once
//#include "Object.h"
class Object;
struct SpawnObject
{
	Object* obj;
	Layer type;
};
class Scene
{
public:
	Scene();
	virtual ~Scene();
public:
	//virtual void Init() =0;
	virtual void Init() abstract;
	virtual void Update();
	virtual void LateUpdate();
	void FixedUpdate(float _fixedDT);
	virtual void Render(HDC _hdc);
	virtual void Release();
public:
	const vector<Object*>& GetLayerObjects(Layer _type) const
	{
		return m_vecObj[(UINT)_type];
	}
public:
	void AddObject(Object* _obj, Layer _type)
	{
		m_vecObj[(UINT)_type].push_back(_obj);
	}
	template<typename T>
	T* Spawn(Layer _type, Vec2 _pos, Vec2 _size)
	{
		static_assert(std::is_base_of<Object, T>::value, "Object로부터 상속받아야 함");
		T* obj = new T;
		obj->SetPos(_pos);
		obj->SetSize(_size);
		AddObject(obj, _type);
		return obj;
	}
	void RequestDestroy(Object* obj);
	void RequestSpawn(Object* obj, Layer _type);
	void FlushEvent();
private:
	void RemoveObject(Object* _obj);
private:
private:
	vector<Object*> m_killObject;
	vector<SpawnObject> m_spawnObject;
	// ?? 
	vector<Object*> m_vecObj[(UINT)Layer::END];
};

