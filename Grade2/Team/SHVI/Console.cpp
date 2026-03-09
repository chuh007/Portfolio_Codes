#include<Windows.h>
#include "Console.h"
// 함수 정의
void SetConsoleSettings(int _width, int _height, bool
	_isFullScreen, const std::wstring& _title)
{
	SetConsoleTitle(_title.c_str());

	HWND hwnd = GetConsoleWindow();
	if (_isFullScreen)
	{
		// 위에 타이틀바 제거
		SetWindowLong(hwnd, GWL_STYLE, WS_POPUP);
		ShowWindow(hwnd, SW_MAXIMIZE);
	}
	else
	{
		// 타이틀바 없에기
		//LONG style = GetWindowLong(hwnd, GWL_STYLE);
		//style &= ~WS_CAPTION;
		//SetWindowLong(hwnd, GWL_STYLE, style);
		// 해상도 설정
		MoveWindow(hwnd, 0, 0, _width, _height, true);
	}
}
void SetLockResize()
{
	HWND hwnd = GetConsoleWindow();
	LONG style = GetWindowLong(hwnd, GWL_STYLE);
	style &= ~WS_SIZEBOX & ~WS_MAXIMIZEBOX;
	SetWindowLong(hwnd, GWL_STYLE, style);
}

void Gotoxy(int _x, int _y)
{
	HANDLE handle = GetStdHandle(STD_OUTPUT_HANDLE);
	// 2칸이 더 자연스러울 수도 있다.
	COORD Cur = { _x * 2 + GetConsoleResolution().x / 4, _y + 1 };

	SetConsoleCursorPosition(handle, Cur);
}

BOOL IsGotoxy(int _x, int _y)
{
	HANDLE handle = GetStdHandle(STD_OUTPUT_HANDLE);
	// 2칸이 더 자연스러울 수도 있다.
	COORD Cur = { _x, _y };

	return SetConsoleCursorPosition(handle, Cur);
}

COORD GetCursorPos()
{
	HANDLE handle = GetStdHandle(STD_OUTPUT_HANDLE);
	CONSOLE_SCREEN_BUFFER_INFO buf;
	GetConsoleScreenBufferInfo(handle, &buf);
	return buf.dwCursorPosition;
}

Position GetConsoleResolution()
{
	HANDLE handle = GetStdHandle(STD_OUTPUT_HANDLE);
	CONSOLE_SCREEN_BUFFER_INFO buf;
	GetConsoleScreenBufferInfo(handle, &buf);
	short width = buf.srWindow.Right -
		buf.srWindow.Left + 1;
	short height = buf.srWindow.Bottom -
		buf.srWindow.Top + 1;
	return { width, height };
}

void FrameSync(unsigned int _frame)
{
	clock_t oldtime, curtime;
	oldtime = clock();
	while (true)
	{
		curtime = clock();
		if (curtime - oldtime >= 1000 / _frame)
		{
			oldtime = curtime;
			break;
		}
	}

}

void SetCursorVisual(bool _isVis, DWORD _size)
{
	HANDLE handle = GetStdHandle(STD_OUTPUT_HANDLE);
	CONSOLE_CURSOR_INFO info = { _size, _isVis };
	SetConsoleCursorInfo(handle, &info);
}

void SetColor(COLOR _textcolor, COLOR _bgcolor)
{
	int textcolor = (int)_textcolor;
	int bgcolor = (int)_bgcolor;
	HANDLE handle = GetStdHandle(STD_OUTPUT_HANDLE);
	SetConsoleTextAttribute(handle,
		(bgcolor << 4) | textcolor);
}

void SetConsoleFont(LPCWSTR _fontname, COORD _size, UINT _weight)
{
	// 콘솔 핸들
	HANDLE handle = GetStdHandle(STD_OUTPUT_HANDLE);
	// 구조체 초기화
	CONSOLE_FONT_INFOEX cf = {};
	cf.cbSize = sizeof(CONSOLE_FONT_INFOEX);

	// 현재 폰트 정보 Get
	GetCurrentConsoleFontEx(handle, false, &cf);
	cf.dwFontSize = _size; // 폭, 높이
	cf.FontWeight = _weight; // FW~
	wcscpy_s(cf.FaceName, _fontname); // 폰트이름 복사
	// 폰트 정보 Set
	SetCurrentConsoleFontEx(handle, false, &cf);
}