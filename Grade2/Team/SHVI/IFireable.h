#pragma once
#include "Enums.h"
class IFireable
{
public:
	virtual ~IFireable() = default;
	virtual void Fire() abstract;
	virtual bool CanFire() abstract;
	protected:
	int m_delay;
	int m_curdelay;
};

