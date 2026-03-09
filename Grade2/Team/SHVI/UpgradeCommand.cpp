#include "UpgradeCommand.h"
#include "IUpgradeable.h"
#include "GameObject.h"
UpgradeCommand::UpgradeCommand(Key _key)
	: m_key(_key)
{
}

void UpgradeCommand::Execute(GameObject* _gameobject)
{
	IUpgradeable* upgrader = dynamic_cast<IUpgradeable*>(_gameobject);
	if (upgrader)
	{
		upgrader->Upgrade(m_key);
	}
}
