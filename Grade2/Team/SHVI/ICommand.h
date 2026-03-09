#pragma once
class GameObject;
class ICommand
{
public:
	virtual ~ICommand() = default;
	virtual void Execute(GameObject* _actor) abstract;
};

