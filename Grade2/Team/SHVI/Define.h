#pragma once
#define SAFE_DELETE(p) if(p) {delete p; p=nullptr;}

const int MAP_HEIGHT = 26;
const int MAP_WIDTH = 26;