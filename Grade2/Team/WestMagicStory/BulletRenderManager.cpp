#include "pch.h"
#include "BulletRenderManager.h"

void BulletRenderManager::Init()
{
	::SelectObject(m_hBackgroundDC, m_hBackgroundBit);
}

void BulletRenderManager::Release()
{
	::DeleteObject(m_hBackgroundBit);
	::DeleteDC(m_hBackgroundDC);
}