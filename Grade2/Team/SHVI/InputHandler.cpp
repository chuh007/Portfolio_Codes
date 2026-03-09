#include <Windows.h>
#include "InputHandler.h"
#include "MoveCommand.h"
#include "FireCommand.h"
#include "UpgradeCommand.h"

InputHandler::InputHandler()
{
	m_vecKeys.push_back({ 'W', Key::UP });
	m_vecKeys.push_back({ 'S', Key::DOWN });
	m_vecKeys.push_back({ 'A', Key::LEFT });
	m_vecKeys.push_back({ 'D', Key::RIGHT });
	m_vecKeys.push_back({ VK_SPACE, Key::SPACE });
	m_vecKeys.push_back({ VK_ESCAPE, Key::ESC });
	m_vecKeys.push_back({ '1', Key::UPGRADE1 });
	m_vecKeys.push_back({ '2', Key::UPGRADE2 });
	m_vecKeys.push_back({ '3', Key::UPGRADE3 });
}

Key InputHandler::TitleInput()
{
	for (auto& key : m_vecKeys)
	{
		bool isDown = (GetAsyncKeyState(key.vk) & 0x8000) != 0;
		if (isDown)
		{
			if(key.key != Key::LEFT && key.key != Key::RIGHT && key.key != Key::ESC)
				return key.key;
		}
	}
	Sleep(30);
	return Key::NONE;
}



Key InputHandler::InfoInput()
{
	for (auto& key : m_vecKeys)
	{
		bool isDown = (GetAsyncKeyState(key.vk) & 0x8000) != 0;
		if (isDown)
		{
			if (key.key == Key::ESC)
				return key.key;
		}
	}
	Sleep(30);
	return Key::NONE;
}

ICommand* InputHandler::HandleInput()
{
	for (auto& key : m_vecKeys)
	{
		bool isDown = (GetAsyncKeyState(key.vk) & 0x8000) != 0;
		if (isDown)
		{
			if (key.key < Key::SPACE)
				return new MoveCommand
				((Dir)key.key);
			else if (key.key < Key::UPGRADE1)
				return new FireCommand
				(key.key);
			else
				return new UpgradeCommand
				(key.key);
		}

	}
	Sleep(30);
	return nullptr;
}
