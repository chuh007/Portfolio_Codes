#include "pch.h"
#include "Texture.h"
#include "Core.h"
Texture::Texture()
	: m_hBit(nullptr)
	, m_hDC(nullptr)
	, m_bitInfo{}
{
}

Texture::~Texture()
{
	::DeleteDC(m_hDC);
	::DeleteObject(m_hBit);
}

void Texture::Load(const wstring& _filePath)
{
	// 1. 인스턴스핸들: nullptr - 독립형 리소스
	// 2. 경로
	// 3. 파일형식 - 비트맵인지
	// 4~5. 파일 크기
	// 6. 옵션 플래그
	m_hBit = (HBITMAP)::LoadImage(nullptr, _filePath.c_str(), IMAGE_BITMAP
		, 0, 0, LR_CREATEDIBSECTION | LR_LOADFROMFILE);
	// 필수1!!!!!!!!@!@!@#$~!@#%$@!#%^#@!%@!#%!@
	assert(m_hBit);
	m_hDC = ::CreateCompatibleDC(GET_SINGLE(Core)->GetMainDC());
	::SelectObject(m_hDC, m_hBit);
	::GetObject(m_hBit, sizeof(BITMAP), &m_bitInfo);

}
