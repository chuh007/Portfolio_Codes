#pragma once
#include "BossMover.h"
#include "ResourceManager.h"
class Object;
class Pattern
{
public:
	Pattern(Object* _owner, Object* _target,
		float _patternUseTime, BossMover* _mover
		, wstring _name);
	virtual ~Pattern();

public:
	virtual void Update();
	virtual void BaseShoot() abstract;
public:
	float GetDecValue() { return m_decValue; }
	wstring GetName() { return m_patternName; }
protected:
	Object* m_owner;
	Object* m_target;
	float m_BaseShoutCooldown;
	BossMover* m_mover;

	float m_curTime;
	float m_patternUseTime;
	
	float m_decValue;
	wstring m_patternName;
};

