#pragma once
#include "Player.h"
class PlayerManager
{
	DECLARE_SINGLE(PlayerManager);

public:
	Player* GetPlayer() { return m_player; }
	void SetPlayer(Player* _player)
	{
		m_player = _player;
	}

private:
	Player* m_player;
};

