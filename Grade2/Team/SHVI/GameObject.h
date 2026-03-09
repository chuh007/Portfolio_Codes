#pragma once
#include "Position.h"
#include <iostream>
using std::cout;
using std::string;
class ObjectManager;

class GameObject
{
public:
	GameObject(Position _pos = { 0,0 });
	virtual ~GameObject();
	virtual void Update();
	virtual void Render();
	virtual void OnDestroy();
public:
	const Position& GetPos() const { return m_pos; }
public:
	bool isDead;
protected:
	Position m_pos;
	Position m_beforepos;
	string m_renderIcon;
};

