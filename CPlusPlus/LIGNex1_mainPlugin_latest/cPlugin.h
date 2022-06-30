#pragma once

#include "Utill.h"

//XPLM Global variable
static XPLMDataRef weaponX, weaponY, weaponZ;
static XPLMDataRef planeX, planeY, planeZ;

// x-plane에서 static XPLMDataRef 변수를 권장함.
static XPLMDataRef planeLat, planeLon, planeAlt;
static XPLMDataRef velocityX, velocityY, velocityZ;
static XPLMDataRef roll_angle, pitch_angle, heading_angle, magnetic_heading_angle;
static XPLMDataRef accel_x, accel_y, accel_z;
static XPLMDataRef roll_rate, pitch_rate, yaw_rate;
static XPLMDataRef accel_roll, accel_pitch, accel_yaw;

// inventory variable
static XPLMDataRef weaponCount; //"sim/weapons/weapon_count"							int
static XPLMDataRef type;		//"sim/weapons/type"									int[25]
static XPLMDataRef actionMode;	//"sim/weapons/action_mode"								int[25]
static XPLMDataRef selecWeapon;	//"sim/cockpit/weapons/wpn_sel_console"					int
static XPLMDataRef reArm;		//"sim/weapons/re_arm_aircraft"							

static XPLMDataRef chaff_now;	// sim/cockpit/weapons/chaff_now
static XPLMDataRef flare_now;	// sim/cockpit/weapons/flare_now
static XPLMDataRef gun_now;		// sim/cockpit/weapons/guns_armed

//Global variable
static Double3						g_flightLocation;
static Double3						g_flightLocationXYZ;
static map_num_receiveWeaponPos		g_map_num_vecReceiveWeaponPos;

// XPLM CommandRef variable
static XPLMCommandRef fireSelectedWeapon;

class cPlugin
{
private:
	double	m_dbLatitude;
	double	m_dbLongitude;
	double	m_dbAltitude;
	
	Float3	m_fVelocityXYZ;
	Float3	m_fAxisAngle;
	float	m_fMagneticHeadingAngle;
	Float3	m_fAccelXYZ;
	Float3	m_fRate;
	Float3	m_fAccelAxis;

	SYSTEMTIME m_UTCTime;


	// inventory variable
	int _weaponCount;
	int _type[14];
	int _actionMode[14];
	int _selecWeapon;
	int _flareCount;
	int _chaffCount;
	int _gunCount;

public:
	cPlugin();
	~cPlugin();

	// SDD Document Function
	void Position();// 41번 패킷 묶음 정보들 총 갱신 함수
	void ArmCmd();	// 무기 인벤토리
	void UTCTime(); // utc time update
	static int WeaponMovingLine(XPLMDrawingPhase inPhase,	int inIsBefore,	void* inRefcon);		//x-plane draw call back function thread

	// Add Function
	void Init();
	void Release();

	void Update(); // total update function

	void ReceiveWeaponPos(const double lat, const double lon, const double alt, const int num = 0);
	void ReceiveWeaponPos(const Double3 pos, const int num = 0);
	inline void AddWeaponPosXYZ(const Double3 location, const int num);									//Add vector container
	
	//void SendWeaponPos(Double3 xyzPos);
	packet_41 Get41Packet();
	inline map_num_receiveWeaponPos sendWeaponPos() { return g_map_num_vecReceiveWeaponPos; }

	//Get, Set Function
	inline SYSTEMTIME* GetUTCTime() { return &m_UTCTime; }

	inline int GetWeaponCount() { return _weaponCount; }
	inline int* GetWeaponType() { return _type; }
	inline int* GetWeaponMode() { return _actionMode; }
	inline int GetWeaponSelec() { return _selecWeapon; }

	inline int GetChaffCount() { return _chaffCount; }
	inline int GetFlareCount() { return _flareCount; }
	inline int GetGunCount() { return _gunCount; }

	inline XPLMCommandRef GetFireSelectedWeapon() { return fireSelectedWeapon; }

	void SetSelectWeapon(int num) { XPLMSetDatai(selecWeapon, num); }
};

