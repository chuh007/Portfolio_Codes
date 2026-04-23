#include "pch.h"
#include "PoolManager.h"

PoolManager::~PoolManager()
{
    if (m_pools.empty()) return;
    for (auto& item : m_pools)
    {
        SAFE_DELETE(item.second);
    }
    m_pools.clear();
}

void PoolManager::Release()
{
    if (m_pools.empty()) return;
    for (auto& item : m_pools)
    {
        SAFE_DELETE(item.second);
    }
    m_pools.clear();
}
