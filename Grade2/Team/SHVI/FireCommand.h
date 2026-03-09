#pragma once
#include "ICommand.h"
#include "Enums.h"
class FireCommand : public ICommand
{
public:
	FireCommand(Key _key);
	virtual void Execute(GameObject* _gameobject) override;
private:
	Key m_key;

};

