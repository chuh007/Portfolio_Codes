#include "MoveCommand.h"
#include "IMovable.h"
#include "GameObject.h"

MoveCommand::MoveCommand(Dir _dir)
	: m_dir(_dir)
{
}

void MoveCommand::Execute(GameObject* _gameobject)
{
	IMovable* mover = dynamic_cast<IMovable*>(_gameobject);
	if (mover)
	{
		mover->Move(m_dir);
	}
}
