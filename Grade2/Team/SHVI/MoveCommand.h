#pragma once
#include "ICommand.h"
#include "Enums.h"
class MoveCommand : public ICommand
{
public:
	MoveCommand(Dir _dir);
	virtual void Execute(GameObject* _gameobject) override;
private:
	Dir m_dir;
};

