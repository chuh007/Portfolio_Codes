#pragma once
class IDamageable
{
public:
	virtual void TakeDamage(int _damage) abstract;
	virtual void HPZero() abstract;
};

