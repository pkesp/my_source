#include "cDisplay.h"

cDisplay::cDisplay()
{
}


cDisplay::~cDisplay()
{
}

void cDisplay::Init()
{
	WindowCreate_UTCTime();

	WindowCreate_Arm();

	WindowCreate_WeaponLine();

}

void cDisplay::Release()
{
	WidnowDestroy_UTCTime();
	WidnowDestroy_Arm();
	WidnowDestroy_WeaponLine();
}

void cDisplay::Update()
{
	// 창 사라지면 다시 켜지게 하는 코드 넣자
	XPLMGetScreenSize(&_width, &_height);

	XPLMGetScreenBoundsGlobal(&_left, &_top, &_right, &_bottom);


}

// ======================================================================================= UTC Time Function START
void cDisplay::WindowCreate_UTCTime()
{
	g_struct_winUtc.structSize = sizeof(g_struct_winUtc);
	g_struct_winUtc.visible = 1;
	g_struct_winUtc.refcon = NULL;
	g_struct_winUtc.decorateAsFloatingWindow = 1;				// test end : 창을 없앨거냐 0 이면 사라짐
	g_struct_winUtc.layer = 1;									// test end : current status is basic //choose the 0, 1, 3
	g_struct_winUtc.drawWindowFunc = Utc_draw;					// window draw fuction pointer
	g_struct_winUtc.handleMouseClickFunc = Utc_click;
	g_struct_winUtc.handleCursorFunc = Dummy_cursor_status;
	g_struct_winUtc.handleKeyFunc = Dummy_keyControl;
	g_struct_winUtc.handleRightClickFunc = Dummy_right_click;
	g_struct_winUtc.handleMouseWheelFunc = Dummy_MouseWheel;

	int winRect[4];
	XPLMGetScreenBoundsGlobal(&winRect[0], &winRect[1], &winRect[2], &winRect[3]);
	g_struct_winUtc.left = winRect[0] + 550;
	g_struct_winUtc.bottom = winRect[1] - 200;
	g_struct_winUtc.right = winRect[0] + 950;
	g_struct_winUtc.top = winRect[1] - 100;

	g_Window_UTC = XPLMCreateWindowEx(&g_struct_winUtc);

	XPLMSetWindowTitle(g_Window_UTC, "- UTC Time -");
}

void cDisplay::WidnowDestroy_UTCTime()
{
	XPLMDestroyWindow(g_Window_UTC);
	g_Window_UTC = NULL;
}

void cDisplay::Utc_draw(XPLMWindowID inWindowID, void * inRefcon)
{
	// ================================================ Window Dynamic Setting START ================================================

	int l, t, r, b;
	XPLMGetWindowGeometry(inWindowID, &l, &t, &r, &b);
	int width, height;
	width = r - l;
	height = t - b;
	//XPLMDrawTranslucentDarkBox(l, t, r, b);
	// ================================================ Window Dynamic Setting END ================================================

	// This is going to delete // I will delete this code // I must delete this underline
	//g_plugin->UTCTime(); 

	static float col_white[] = { 1.0, 1.0, 1.0 };
	static char buffer[150];

	sprintf(buffer, "- UTC Time -");
	XPLMDrawString(col_white, l, t - 20, buffer, NULL, xplmFont_Proportional);
	
	//int text_width = XPLMMeasureString(xplmFont_Proportional, buffer, strlen(buffer));
	//float text_midpoint_x = (b[2] + b[0]) / 2;

	sprintf(buffer, "%d / %d / %d",
		g_plugin->GetUTCTime()->wYear,
		g_plugin->GetUTCTime()->wMonth,
		g_plugin->GetUTCTime()->wDay);
	XPLMDrawString(col_white, l , t - 40, buffer, NULL, xplmFont_Proportional);

	sprintf(buffer, "%d : %d : %d",
		g_plugin->GetUTCTime()->wHour,
		g_plugin->GetUTCTime()->wMinute,
		g_plugin->GetUTCTime()->wSecond);
	XPLMDrawString(col_white, l, t - 60, buffer, NULL, xplmFont_Proportional);
}

int cDisplay::Utc_click(XPLMWindowID inWindowID, int x, int y, XPLMMouseStatus inMouse, void * inRefcon) 
{ 
	/*enum {
		xplm_MouseDown = 1

		, xplm_MouseDrag = 2

		, xplm_MouseUp = 3


	};
	typedef int XPLMMouseStatus;*/



	return 0; 
}
// ======================================================================================= UTC Time Function END


// ======================================================================================= Flight Arm Status Function START
void cDisplay::WindowCreate_Arm()
{
	g_struct_winArm.structSize = sizeof(g_struct_winArm);
	g_struct_winArm.visible = 1;
	g_struct_winArm.refcon = NULL;
	g_struct_winArm.decorateAsFloatingWindow = 1;				// test end : 창을 없앨거냐 0 이면 사라짐
	g_struct_winArm.layer = 1;									// test end : current status is basic //choose the 0, 1, 3
	g_struct_winArm.drawWindowFunc = Arm_draw;					// window draw fuction pointer
	g_struct_winArm.handleMouseClickFunc = Arm_click;
	g_struct_winArm.handleCursorFunc = Dummy_cursor_status;
	g_struct_winArm.handleKeyFunc = Dummy_keyControl;
	g_struct_winArm.handleRightClickFunc = Dummy_right_click;
	g_struct_winArm.handleMouseWheelFunc = Dummy_MouseWheel;

	int winRect[4];
	XPLMGetScreenBoundsGlobal(&winRect[0], &winRect[1], &winRect[2], &winRect[3]);
	g_struct_winArm.left = winRect[2] - 450;
	g_struct_winArm.bottom = winRect[3] + 200;
	g_struct_winArm.right = winRect[2] - 50;
	g_struct_winArm.top = winRect[3] + 600;

	g_Window_Arm = XPLMCreateWindowEx(&g_struct_winArm);

	XPLMSetWindowTitle(g_Window_Arm, "- Flight Arms -");

	// =========================== get missile name start =========================== 
	char buf[32];
	DWORD bufsize = 32;
	GetUserName(buf, &bufsize);
	std::string path = "C:\\Users\\";
	std::string path2 = "\\Desktop\\X-Plane 11\\Output\\preferences\\Freeflight.prf";
	path.append(buf);
	path.append(path2);
	std::fstream infile;
	infile.open(path.c_str(), std::ios::in);

	std::string str;
	int count = 13;

	while (!infile.eof()) {
		getline(infile, str);
		if (10 == str.find("__AircraftFA18Fv151FA18Farmedacf_")) {
			if (count >= 12) {
				count -= 1;
			}
			else {
				size_t start = str.find(' ', 10) + 1;
				size_t end = str.rfind('.');
				_strArry[count] = str.substr(start, end - start);

				count -= 1;
			}
		}
	}

	infile.close();
	// =========================== get missile name end =========================== 
}

void cDisplay::WidnowDestroy_Arm()
{
	XPLMDestroyWindow(g_Window_Arm);
	g_Window_Arm = NULL;
}

void cDisplay::Arm_draw(XPLMWindowID inWindowID, void * inRefcon)
{
	int l, t, r, b;
	XPLMGetWindowGeometry(inWindowID, &l, &t, &r, &b);
	//XPLMDrawTranslucentDarkBox(l, t, r, b);

	static float col_white[] = { 1.0, 1.0, 1.0 };
	static char buffer[150];

	/*sprintf(buffer, "- debuging -");
	XPLMDrawString(col_white, l, t - 20, buffer, NULL, xplmFont_Proportional);

	sprintf(buffer, "Select : %d", g_plugin->GetWeaponSelec());
	XPLMDrawString(col_white, l, t - 35, buffer, NULL, xplmFont_Proportional);
	
	sprintf(buffer, "Selected Weapon Index : %d, (%02d)", g_plugin->GetWeaponSelec(), g_plugin->GetWeaponType()[g_plugin->GetWeaponSelec()]);
	XPLMDrawString(col_white, l, t - 50, buffer, NULL, xplmFont_Proportional);

	for (int i = 0; i < 14; i++) {
		sprintf(buffer, "%02d : weapon typeNumber(%02d), carriaged : %d", i, g_plugin->GetWeaponType()[i], ReternZero(g_plugin->GetWeaponMode()[i]));
		XPLMDrawString(col_white, l, t - 65 - (15 * i), buffer, NULL, xplmFont_Proportional);
	}*/

	sprintf(buffer, "Current Selected Weapon : %d", g_plugin->GetWeaponSelec());
	XPLMDrawString(col_white, l, t - 20, buffer, NULL, xplmFont_Proportional);

	for (int i = 0; i < 12; i++) {
		if (_strArry[i].find("gun") == _strArry[i].npos)
			sprintf(buffer, "%02d : %s  %d EA", i, _strArry[i].c_str(), ReternZero(g_plugin->GetWeaponMode()[i]));
		else
			sprintf(buffer, "%02d : %s  %d EA", i, _strArry[i].c_str(), g_plugin->GetGunCount());
		
		if (ReternZero(g_plugin->GetWeaponMode()[i])){
			float col_green[] = { 0.f, 1.f, 0.f };
			XPLMDrawString(col_green, l, t - 40 - (15 * i), buffer, NULL, xplmFont_Proportional);
		}
		else {
			float col_red[] = { 1.f, 0.f, 0.f };
			XPLMDrawString(col_red, l, t - 40 - (15 * i), buffer, NULL, xplmFont_Proportional);
		}
	}

	
	sprintf(buffer, "12 : chaff now : %d EA", g_plugin->GetChaffCount());
	XPLMDrawString(col_white, l, t - 40 - (15 * 12), buffer, NULL, xplmFont_Proportional);

	sprintf(buffer, "13 : chaff now : %d EA", g_plugin->GetFlareCount());
	XPLMDrawString(col_white, l, t - 40 - (15 * 13), buffer, NULL, xplmFont_Proportional);
	
	sprintf(buffer, "gun now : %d EA", g_plugin->GetGunCount());
	XPLMDrawString(col_white, l, t - 40 - (15 * 14), buffer, NULL, xplmFont_Proportional);

}

int cDisplay::Arm_click(XPLMWindowID inWindowID, int x, int y, XPLMMouseStatus inMouse, void * inRefcon) 
{


	return 0;
}


// ======================================================================================= Flight Arm Status Function END

// ======================================================================================= Weapon Moving Draw Function START
void cDisplay::WindowCreate_WeaponLine() 
{
	g_sturct_winWeapon.structSize = sizeof(g_sturct_winWeapon);
	g_sturct_winWeapon.visible = 0; // test ing	: default 0			// The weapon moving widnow is activation after the weapon fire
	g_sturct_winWeapon.refcon = NULL;
	g_sturct_winWeapon.decorateAsFloatingWindow = 1;				// test end : 창을 없앨거냐 0 이면 사라짐
	g_sturct_winWeapon.layer = xplm_WindowLayerFlightOverlay;		// test end : current status is basic //choose the 0, 1, 3
	g_sturct_winWeapon.drawWindowFunc = Weapon_draw;				// window draw fuction pointer
	g_sturct_winWeapon.handleMouseClickFunc = Weapon_click;
	g_sturct_winWeapon.handleCursorFunc = Dummy_cursor_status;
	g_sturct_winWeapon.handleKeyFunc = Dummy_keyControl;
	g_sturct_winWeapon.handleRightClickFunc = Dummy_right_click;
	g_sturct_winWeapon.handleMouseWheelFunc = Dummy_MouseWheel;

	int winRect[4];
	XPLMGetScreenBoundsGlobal(&winRect[0], &winRect[1], &winRect[2], &winRect[3]);
	g_sturct_winWeapon.left = winRect[0] + 50;
	g_sturct_winWeapon.bottom = winRect[3] + 200;
	g_sturct_winWeapon.right = winRect[0] + 550;
	g_sturct_winWeapon.top = winRect[3] + 700;

	g_Window_Weapon = XPLMCreateWindowEx(&g_sturct_winWeapon);

	XPLMSetWindowTitle(g_Window_Weapon, "- Weapon Road Thumbnail -");
}

void cDisplay::WidnowDestroy_WeaponLine() 
{
	XPLMDestroyWindow(g_Window_Weapon);
	g_Window_Weapon = NULL;
}

// Waapon road Thumbnail
void cDisplay::Weapon_draw(XPLMWindowID inWindowID, void * inRefcon) 
{
	int l, t, r, b;
	XPLMGetWindowGeometry(inWindowID, &l, &t, &r, &b);
	XPLMDrawTranslucentDarkBox(l, t, r, b);
}

int cDisplay::Weapon_click(XPLMWindowID inWindowID, int x, int y, XPLMMouseStatus inMouse, void * inRefcon) 
{ 
	return 0; 
}


// ======================================================================================= Weapon Moving Draw Function END
