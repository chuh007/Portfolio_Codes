#include "pch.h"
#include "Pattern.h"
#include "Object.h"

Pattern::Pattern(Object* _owner, Object* _target, float _patternUseTime, BossMover* _mover, wstring _name)
	: m_curTime(0.f)
	, m_decValue(1.f)
	, m_patternUseTime(0.f)
{
	m_owner = _owner;
	m_target = _target;
	m_BaseShoutCooldown = _patternUseTime;
	m_mover = _mover;
	m_patternName = _name;
}

Pattern::~Pattern()
{
}

void Pattern::Update()
{
	m_patternUseTime += fDT;
}