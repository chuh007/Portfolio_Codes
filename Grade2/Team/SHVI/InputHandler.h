#pragma once
#include <vector>
#include "Enums.h"
class ICommand;
struct InputKey
{
	int vk;
	Key key;
};
class InputHandler
{
public:
	InputHandler();
public:
	Key TitleInput();
	Key InfoInput();
public:
	ICommand* HandleInput();
	std::vector<InputKey> m_vecKeys;
};

