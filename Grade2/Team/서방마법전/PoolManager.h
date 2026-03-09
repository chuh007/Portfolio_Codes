#pragma once
#include "Scene.h"
#include "SceneManager.h"
#include <unordered_map>
#include <stack>

class IPool
{
public:
	virtual ~IPool() {};
};
class IPoolable
{
public:
	virtual void Reset() abstract;
};

template <typename T>
class Pool : public IPool
{
public:
	Pool(int _initSize, Layer _layer)
	{
		m_layer = _layer;
		auto curScene = GET_SINGLE(SceneManager)->GetCurScene();
		for (int i = 0; i < _initSize; ++i)
		{
			T* obj = curScene->Spawn<T>(_layer, { 0, 0 }, { 10.f,10.f });
			obj->SetActive(false);
			m_pool.push(obj);
		}
	}

public:
	T* Pop()
	{
		T* obj = nullptr;
		if (m_pool.empty())
		{
			obj = GET_SINGLE(SceneManager)->GetCurScene()->Spawn<T>(m_layer, { 0, 0 }, { 10.f,10.f });
		}
		else
		{
			obj = m_pool.top();
			m_pool.pop();
		}
		obj->SetActive(true);
		obj->Reset();
		return obj;
	}
	void Push(T* _obj)
	{
		m_pool.push(_obj);
		_obj->SetActive(false);
	}
private:
	std::stack<T*> m_pool;
	Layer m_layer;
};

class PoolManager
{
	DECLARE_SINGLE(PoolManager);
public:
	~PoolManager();
	void Release();
public:
	template <typename T>
	void AddPool(PoolType _type, int _initCount, Layer _layer)
	{
		Pool<T>* newPool = new Pool<T>(_initCount, _layer);
		m_pools[_type] = newPool;
	}
	template <typename T>
	T* Pop(const PoolType& _type)
	{
		Pool<T>* pool = static_cast<Pool<T>*>(m_pools[_type]);
		return pool->Pop();
	}
	template <typename T>
	void Push(const PoolType& _type, T* obj)
	{
		Pool<T>* pool = static_cast<Pool<T>*>(m_pools[_type]);
		return pool->Push(obj);
	}
	
private:
	std::unordered_map<PoolType, IPool*> m_pools;
};

