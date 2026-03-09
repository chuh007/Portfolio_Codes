#include "FireCommand.h"
#include "IFireable.h"
#include "GameObject.h"

FireCommand::FireCommand(Key _key)
	: m_key(_key)

{
}

void FireCommand::Execute(GameObject* _gameobject)
{
	IFireable* firer = dynamic_cast<IFireable*>(_gameobject);
	if (firer)
	{
		if (!firer->CanFire()) return;
		firer->Fire();
	}
}
