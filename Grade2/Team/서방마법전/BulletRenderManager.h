#pragma once

class Texture;
class BulletRenderManager
{
	DECLARE_SINGLE(BulletRenderManager);
public:
	void Init();
	void Release();

public:
	HDC GetBulletDC()
	{ return m_hBackgroundDC; }
	HBITMAP GetBulletBitMap()
	{ return m_hBackgroundBit; }
	void SetBulletDC(HDC _dc)
	{ m_hBackgroundDC = _dc; }
	void SetBulletBitMap(HBITMAP _bitMap)
	{ m_hBackgroundBit = _bitMap; }
private:

	HDC m_hBackgroundDC;
	HBITMAP m_hBackgroundBit;
};

