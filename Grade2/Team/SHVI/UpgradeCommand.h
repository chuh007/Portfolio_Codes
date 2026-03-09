#pragma once
#include "ICommand.h"
#include "Enums.h"
class UpgradeCommand : public ICommand
{
public:
	UpgradeCommand(Key _key);
	virtual void Execute(GameObject* _gameobject) override;
private:
	Key m_key;
};

