#pragma once

#include "Utill.h"
#include <XPLMDisplay.h>
#include "cPlugin.h"

static XPLMWindowID	g_Window_UTC = NULL;
static XPLMWindowID	g_Window_Arm = NULL;
static XPLMWindowID	g_Window_Weapon = NULL;

static XPLMCreateWindow_t g_struct_winUtc;
static XPLMCreateWindow_t g_struct_winArm;
static XPLMCreateWindow_t g_sturct_winWeapon;

static std::string _strArry[12];

extern cPlugin * g_plugin;

class cDisplay
{
private:
	int _width;
	int _height;

	int _left;
	int _top;
	int _right;
	int _bottom;

public:
	cDisplay();
	~cDisplay();

	void Init();
	void Release();
	void Update();


	// Link Function
	//void SetPluginLink(cPlugin * plugin) { _pPlugin = plugin; }

	// dummy Function
	static void Dummy_draw(XPLMWindowID inWindowID, void * inRefcon) {}														//정보 채우기용 빈 함수. 꼭 채워넣어야 함.
	static int Dummy_click(XPLMWindowID inWindowID, int x, int y, XPLMMouseStatus inMouse, void * inRefcon) { return 0; }
	static int Dummy_right_click(XPLMWindowID inWindowID, int x, int y, XPLMMouseStatus inMouse, void * inRefcon) { return 0; }
	static XPLMCursorStatus Dummy_cursor_status(XPLMWindowID inWindowID, int x, int y, void * inRefcon) { return 0; }
	static void Dummy_keyControl(XPLMWindowID inWindowID, char inKey, XPLMKeyFlags inFlags, char inVirtualKey, void * inRefcon, int losingFocus) {}
	static int Dummy_MouseWheel(XPLMWindowID inWindowID, int x, int y, int wheel, int clicks, void * inRefcon) { return 0; }

	// UTC Window Function
	void WindowCreate_UTCTime();
	void WidnowDestroy_UTCTime();
	static void Utc_draw(XPLMWindowID inWindowID, void * inRefcon);															//x-plane draw call back function thread
	static int Utc_click(XPLMWindowID inWindowID, int x, int y, XPLMMouseStatus inMouse, void * inRefcon);
	//클릭함수는 창 위에서 클릭을 했을 때 인식한다고 되어 있는데 해봤지만 인식이 되지 않았던 것으로 기억함. 더 해봐야 함.


	// Arm Window Function
	void WindowCreate_Arm();
	void WidnowDestroy_Arm();
	static void Arm_draw(XPLMWindowID inWindowID, void * inRefcon);															//x-plane draw call back function thread
	static int Arm_click(XPLMWindowID inWindowID, int x, int y, XPLMMouseStatus inMouse, void * inRefcon);

	// Weapon Window Function (따로 만들려고 했던 것이지만 차후 지워질 것으로 예상함)
	void WindowCreate_WeaponLine();
	void WidnowDestroy_WeaponLine();
	static void Weapon_draw(XPLMWindowID inWindowID, void * inRefcon);														//x-plane draw call back function thread
	static int Weapon_click(XPLMWindowID inWindowID, int x, int y, XPLMMouseStatus inMouse, void * inRefcon);

	static bool ReternZero(int num) { return (num == 0) ? true : false; }

};