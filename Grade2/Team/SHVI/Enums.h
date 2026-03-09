#pragma once
enum class Dir
{
	UP = 0, DOWN, LEFT, RIGHT
};
enum class Menu
{
	START = 0, INFO, QUIT, FAIL
};
enum class SCENE
{
	Title, Game, INFO, GameOver , QUIT, END
};
enum class Key
{
	UP, DOWN, LEFT, RIGHT, SPACE, UPGRADE1, UPGRADE2, UPGRADE3, ESC , NONE
};
enum class UIType
{
	WAVECNT, MODETXT, CHOICE1, CHOICE2, CHOICE3, DESCRIPTION, ATTACKPOWER, ATTACKDELAY
};