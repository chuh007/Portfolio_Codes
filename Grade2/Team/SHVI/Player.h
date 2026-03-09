#pragma once
#include "GameObject.h"
class Player : public GameObject
{
public:
	using Base = GameObject;
	Player(const Position& _pos);
	void Update() override;
	void Render() override;

private:

};

