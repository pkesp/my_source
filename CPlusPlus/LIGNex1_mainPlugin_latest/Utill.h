#pragma once

/*
- 실행할 쪽에 vs2008이상이 설치되어있지 않을 때 msvcr100d.dll 오류 -
- "프로젝트속성:구성속성:C/C++:코드생성:런타임 라이브러리"에서
- 디버그 : 다중 스레드 디버그 DLL(/MDd) => 다중 스레드 디버그(/MTd)
- 릴리즈 : 다중 스레드 DLL(/MD) => 다중 스레드(/MT)
*/
#define _CRT_SECURE_NO_WARNINGS
#define WIN32_LEAN_AND_MEAN

/***********************************************************************
* Include
***********************************************************************/
#include <Windows.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <fstream>
#include <string>
#include <time.h>
#include "XPLMDisplay.h"
#include "XPLMGraphics.h"
#include "XPLMDataAccess.h"
#include "XPLMCamera.h"
#include "XPLMUtilities.h"
#include "XPLMProcessing.h"
#include <gl\GLU.h>
#include <gl\GL.h>
#include <vector>
#include <map>
#include <thread>


/***********************************************************************
* packet
***********************************************************************/
enum enum_Packet
{
	// 비행모의 -> ETU 41 ~ 49 flight
	UTC_TIME_41 = 41,
	POSITION,
	VELOCITY,
	MOTION,

	// ETU -> 비행모의 51 ~ 59 weaponInfo
	WAEPON_POS_51 = 51,
	WEAPON_RELOAD_52,


	// 비행모의 -> MC 61 ~ 69 
	TOTAL_CNT_61 = 61, //total packet count
	WEAPON_INFO_62,	//name, remain
	INVENTORY_READY_63,


	// MC -> 비행모의 71 ~ 79 fire
	WEAPON_FIRE_71 = 71	//fireCmd
};

/***********************************************************************
* typedef
***********************************************************************/
#pragma pack(1)

typedef union stDouble3 {
	struct {
		double coordi[3];
	};

	struct {
		double x;
		double y;
		double z;
	};

	struct {
		double lat;
		double lon;
		double alt;
	};
}Double3, *pDouble3;

typedef union stFloat3 {
	struct {
		float xyz[3];
	};

	struct {
		float x;
		float y;
		float z;
	};

	struct {
		float pitch;
		float yaw;
		float roll;
	};

	struct {
		float pitch;
		float heading;
		float bank;
	};
}Float3, *pFloat3;

typedef struct st_Packet41 {
	int ID;
	SYSTEMTIME time;
	Double3 position;
	Float3 velocity;
	Float3 axis_angle;
	float magnetic_heading;
	Float3 acceleration;
	Float3 axis_rate;
	Float3 angular_acceleration;
}packet_41, *pPacket_41;

typedef struct st_Packet51 {
	int ID;
	Double3 position;
}packet_51, *pPacket51;

typedef struct st_Packet52 {
	int ID;
	int weapon[12];
}packet_52, *pPacket52;

typedef struct st_Packet63 {
	int ID;
	char *str;

	st_Packet63() {
		ID = 63;
		str = "inventory setup ready";
	}
}packet_63, *pPacket63;

typedef struct st_Packet71 {
	int ID;
	int selectNum;
}packet_71, *pPacket71;

#pragma pack(pop)

typedef std::vector<Double3> vec_ReceiveWeaponPos;
typedef std::map<int, vec_ReceiveWeaponPos> map_num_receiveWeaponPos;

/***********************************************************************
* Macro Function
***********************************************************************/
#define SAFE_DELETE(p) if((p)) { delete (p); }
#define SAFE_DELETE_ARRAY(p) if((p)) { delete[] (p); }
#define SAFE_DELETE_RELEASE(p) if ((p)) { (p)->Release(); delete (p); }

#define GET_UNDER1 (1 - (RAND_MAX - rand()))

#define BUFSIZE 256