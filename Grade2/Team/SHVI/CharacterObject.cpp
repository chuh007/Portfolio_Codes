#include "CharacterObject.h"
CharacterObject::CharacterObject(Position _pos)
	:GameObject(_pos)
{
	m_pos = _pos;
}

void CharacterObject::Update()
{
	Base::Update();
}

void CharacterObject::Render()
{
	Base::Render();
}

