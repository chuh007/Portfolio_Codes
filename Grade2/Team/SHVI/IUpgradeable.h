#pragma once
#include "Enums.h"
class IUpgradeable
{
public:
	virtual ~IUpgradeable() = default;
	virtual void Upgrade(Key _key) abstract;
};

