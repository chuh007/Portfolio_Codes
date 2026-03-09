#include "MapManager.h"
#include <fstream>

MapManager* MapManager::m_inst = __nullptr;

// ¡ª æ∆¥‘ ¿Ã∞«
MapManager::MapManager()
	: m_upMap()
	, m_downMap()
	, m_leftMap()
	, m_rightMap()
	, m_secondupMap()
	, m_seconddownMap()
	, m_secondleftMap()
	, m_secondrightMap()
	, m_threeupMap()
	, m_threedownMap()
	, m_threeleftMap()
	, m_threerightMap()
	, m_fullMap()
	, m_fovLevel(4)
{
	std::ifstream mapFile1("randerup.txt");
	if (mapFile1.is_open())
	{
		for (int i = 0; i < MAP_HEIGHT; ++i)
		{
			mapFile1 >> m_upMap[i];
		}
		mapFile1.close();
	}
	std::ifstream mapFile2("randerdown.txt");
	if (mapFile2.is_open())
	{
		for (int i = 0; i < MAP_HEIGHT; ++i)
		{
			mapFile2 >> m_downMap[i];
		}
		mapFile2.close();
	}
	std::ifstream mapFile3("randerleft.txt");
	if (mapFile3.is_open())
	{
		for (int i = 0; i < MAP_HEIGHT; ++i)
		{
			mapFile3 >> m_leftMap[i];
		}
		mapFile3.close();
	}
	std::ifstream mapFile4("randerright.txt");
	if (mapFile4.is_open())
	{
		for (int i = 0; i < MAP_HEIGHT; ++i)
		{
			mapFile4 >> m_rightMap[i];
		}
		mapFile4.close();
	}

	std::ifstream mapFile5("randersecondup.txt");
	if (mapFile5.is_open())
	{
		for (int i = 0; i < MAP_HEIGHT; ++i)
		{
			mapFile5 >> m_secondupMap[i];
		}
		mapFile5.close();
	}
	std::ifstream mapFile6("randerseconddown.txt");
	if (mapFile6.is_open())
	{
		for (int i = 0; i < MAP_HEIGHT; ++i)
		{
			mapFile6 >> m_seconddownMap[i];
		}
		mapFile6.close();
	}
	std::ifstream mapFile12("randersecondleft.txt");
	if (mapFile12.is_open())
	{
		for (int i = 0; i < MAP_HEIGHT; ++i)
		{
			mapFile12 >> m_secondleftMap[i];
		}
		mapFile12.close();
	}
	std::ifstream mapFile13("randersecondright.txt");
	if (mapFile13.is_open())
	{
		for (int i = 0; i < MAP_HEIGHT; ++i)
		{
			mapFile13 >> m_secondrightMap[i];
		}
		mapFile13.close();
	}

	std::ifstream mapFile7("randerthreeup.txt");
	if (mapFile7.is_open())
	{
		for (int i = 0; i < MAP_HEIGHT; ++i)
		{
			mapFile7 >> m_threeupMap[i];
		}
		mapFile7.close();
	}
	std::ifstream mapFile8("randerthreedown.txt");
	if (mapFile8.is_open())
	{
		for (int i = 0; i < MAP_HEIGHT; ++i)
		{
			mapFile8 >> m_threedownMap[i];
		}
		mapFile8.close();
	}
	std::ifstream mapFile9("randerthreeleft.txt");
	if (mapFile9.is_open())
	{
		for (int i = 0; i < MAP_HEIGHT; ++i)
		{
			mapFile9 >> m_threeleftMap[i];
		}
		mapFile9.close();
	}
	std::ifstream mapFile10("randerthreeright.txt");
	if (mapFile10.is_open())
	{
		for (int i = 0; i < MAP_HEIGHT; ++i)
		{
			mapFile10 >> m_threerightMap[i];
		}
		mapFile10.close();
	}

	std::ifstream mapFile11("randerfull.txt");
	if (mapFile11.is_open())
	{
		for (int i = 0; i < MAP_HEIGHT; ++i)
		{
			mapFile11 >> m_fullMap[i];
		}
		mapFile11.close();
	}

	m_curRanderableMap = m_fullMap;
}

MapManager::~MapManager()
{
}

bool MapManager::CanRanderThisPos(const Position& _pos)
{
	return m_curRanderableMap[_pos.y][_pos.x] == '1';
}

void MapManager::SetRanderDir(const Dir& _dir)
{
	if (m_fovLevel >= 4)
	{
		m_curRanderableMap = m_fullMap;
		return;
	}
	switch (_dir)
	{
	case Dir::UP:
		switch (m_fovLevel)
		{
		case 1:
			m_curRanderableMap = m_upMap;
			break;
		case 2:
			m_curRanderableMap = m_secondupMap;
			break;
		case 3:
			m_curRanderableMap = m_threeupMap;
			break;
		}
		break;
	case Dir::DOWN:
		switch (m_fovLevel)
		{
		case 1:
			m_curRanderableMap = m_downMap;
			break;
		case 2:
			m_curRanderableMap = m_seconddownMap;
			break;
		case 3:
			m_curRanderableMap = m_threedownMap;
			break;
		}
		break;
	case Dir::LEFT:
		switch (m_fovLevel)
		{
		case 1:
			m_curRanderableMap = m_leftMap;
			break;
		case 2:
			m_curRanderableMap = m_secondleftMap;
			break;
		case 3:
			m_curRanderableMap = m_threeleftMap;
			break;
		}
		break;
	case Dir::RIGHT:
		switch (m_fovLevel)
		{
		case 1:
			m_curRanderableMap = m_rightMap;
			break;
		case 2:
			m_curRanderableMap = m_secondrightMap;
			break;
		case 3:
			m_curRanderableMap = m_threerightMap;
			break;
		}
		break;
	}
}

void MapManager::FovLock()
{
	m_fovLevel = (m_fovLevel == 1 ? 1 : m_fovLevel - 1);
}

void MapManager::Init()
{
	m_fovLevel = 4;
}
