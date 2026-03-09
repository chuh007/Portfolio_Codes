#pragma once
#include "GameObject.h"
#include "IMovable.h"
class CharacterObject : public GameObject, public IMovable
{
public:
	using Base = GameObject;
	CharacterObject(Position _pos = { 0,0 });
	virtual ~CharacterObject() = default;
public:
	virtual void Update() override;
	virtual void Render() override;
	virtual void Move(Dir _dir) abstract;
};

