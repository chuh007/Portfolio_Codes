#pragma once
#include "Position.h"
const int SPAWN_X[4] = { 12,12,22,2 };
const int SPAWN_Y[4] = { 22,2,12,12 };
const int MOVE_X[4] = { 0,0,-1,1 };
const int MOVE_Y[4] = { -1,1,0,0 };
const float MOVE_TIME = 0.3f;
const int LIFE_INIT = 1;
const float SPAWN_TIME = 2.0f;
const Position PLAYER_POS = { 12,12 };