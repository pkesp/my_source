#include "cPlugin.h"

cPlugin::cPlugin() : _weaponCount(0), _selecWeapon(0)
{
}


cPlugin::~cPlugin()
{
}


void cPlugin::Init()
{
	// position dataref
	planeX					= XPLMFindDataRef("sim/flightmodel/position/local_x");
	planeY					= XPLMFindDataRef("sim/flightmodel/position/local_y");
	planeZ					= XPLMFindDataRef("sim/flightmodel/position/local_z");

	weaponX					= XPLMFindDataRef("sim/weapons/x");
	weaponY					= XPLMFindDataRef("sim/weapons/y");
	weaponZ					= XPLMFindDataRef("sim/weapons/z");

	planeLat				= XPLMFindDataRef("sim/flightmodel/position/latitude");
	planeLon				= XPLMFindDataRef("sim/flightmodel/position/longitude");
	planeAlt				= XPLMFindDataRef("sim/flightmodel/position/elevation"); // Units : meters sea level

	velocityX				= XPLMFindDataRef("sim/flightmodel/forces/vx_acf_axis");
	velocityY				= XPLMFindDataRef("sim/flightmodel/forces/vy_acf_axis");
	velocityZ				= XPLMFindDataRef("sim/flightmodel/forces/vz_acf_axis");

	roll_angle				= XPLMFindDataRef("sim/flightmodel/position/true_phi");
	pitch_angle				= XPLMFindDataRef("sim/flightmodel/position/true_theta");
	heading_angle			= XPLMFindDataRef("sim/flightmodel/position/true_psi");
	magnetic_heading_angle	= XPLMFindDataRef("sim/flightmodel/position/mag_psi");

	accel_x					= XPLMFindDataRef("sim/flightmodel/position/local_ax");
	accel_y					= XPLMFindDataRef("sim/flightmodel/position/local_ay");
	accel_z					= XPLMFindDataRef("sim/flightmodel/position/local_az");

	roll_rate				= XPLMFindDataRef("sim/flightmodel/position/P");
	pitch_rate				= XPLMFindDataRef("sim/flightmodel/position/Q");
	yaw_rate				= XPLMFindDataRef("sim/flightmodel/position/R");

	accel_roll				= XPLMFindDataRef("sim/flightmodel/position/P_dot");
	accel_pitch				= XPLMFindDataRef("sim/flightmodel/position/Q_dot");
	accel_yaw				= XPLMFindDataRef("sim/flightmodel/position/R_dot");

	// inventory variable dataref
	weaponCount				= XPLMFindDataRef("sim/weapons/weapon_count");
	type					= XPLMFindDataRef("sim/weapons/type");
	actionMode				= XPLMFindDataRef("sim/weapons/action_mode");
	selecWeapon				= XPLMFindDataRef("sim/cockpit/weapons/wpn_sel_console");

	chaff_now				= XPLMFindDataRef("sim/cockpit/weapons/chaff_now");
	flare_now				= XPLMFindDataRef("sim/cockpit/weapons/flare_now");
	gun_now					= XPLMFindDataRef("sim/cockpit/weapons/guns_armed");

	// xplm command
	fireSelectedWeapon		= XPLMFindCommand("sim/weapons/fire_any_shell");


	// systemtime init
	ZeroMemory(&m_UTCTime, sizeof(SYSTEMTIME));

	// extern draw call back function register
	XPLMRegisterDrawCallback(WeaponMovingLine, xplm_Phase_Airplanes, false, nullptr);

	// variable initialization
	g_flightLocationXYZ.x = XPLMGetDatad(planeX);
	g_flightLocationXYZ.y = XPLMGetDatad(planeY);
	g_flightLocationXYZ.z = XPLMGetDatad(planeZ);

	g_flightLocation.lat = m_dbLatitude = XPLMGetDatad(planeLat);
	g_flightLocation.lon = m_dbLongitude = XPLMGetDatad(planeLon);
	g_flightLocation.alt = m_dbAltitude = XPLMGetDatad(planeAlt) * 3.28084; // * 3.28084  convert meter to ftmsl

	m_fVelocityXYZ.x = XPLMGetDataf(velocityX), m_fVelocityXYZ.y = XPLMGetDataf(velocityY), m_fVelocityXYZ.z = XPLMGetDataf(velocityZ);
	m_fAxisAngle.roll = XPLMGetDataf(roll_angle), m_fAxisAngle.pitch = XPLMGetDataf(pitch_angle), m_fAxisAngle.heading = XPLMGetDataf(heading_angle);
	m_fMagneticHeadingAngle = XPLMGetDataf(magnetic_heading_angle);

	m_fAccelXYZ.x = XPLMGetDataf(accel_x), m_fAccelXYZ.y = XPLMGetDataf(accel_y), m_fAccelXYZ.z = XPLMGetDataf(accel_z);
	m_fRate.roll = XPLMGetDataf(roll_rate), m_fRate.pitch = XPLMGetDataf(pitch_rate), m_fRate.yaw = XPLMGetDataf(yaw_rate);
	m_fAccelAxis.roll = XPLMGetDataf(accel_roll), m_fAccelAxis.pitch = XPLMGetDataf(accel_pitch), m_fAccelAxis.yaw = XPLMGetDataf(accel_yaw);

	//=================== inventory ===================
	_selecWeapon = XPLMGetDatai(selecWeapon);
	_weaponCount = XPLMGetDatai(weaponCount);
	XPLMGetDatavi(type, _type, 0, 14);
	XPLMGetDatavi(actionMode, _actionMode, 0, 14);

	_chaffCount = XPLMGetDatai(chaff_now);
	_flareCount = XPLMGetDatai(flare_now);
	_gunCount = XPLMGetDatai(gun_now);
}

void cPlugin::Release()
{
	XPLMUnregisterDrawCallback(WeaponMovingLine, xplm_Phase_Airplanes, false, nullptr);
}

void cPlugin::Update()
{
	Position();
	ArmCmd();
	UTCTime();
}

void cPlugin::Position()
{
	g_flightLocationXYZ.x = XPLMGetDatad(planeX);
	g_flightLocationXYZ.y = XPLMGetDatad(planeY);
	g_flightLocationXYZ.z = XPLMGetDatad(planeZ);

	g_flightLocation.lat = m_dbLatitude = XPLMGetDatad(planeLat);
	g_flightLocation.lon = m_dbLongitude = XPLMGetDatad(planeLon);
	g_flightLocation.alt = m_dbAltitude = XPLMGetDatad(planeAlt) * 3.28084; // * 3.28084 ftmsl

	m_fVelocityXYZ.x = XPLMGetDataf(velocityX), m_fVelocityXYZ.y = XPLMGetDataf(velocityY), m_fVelocityXYZ.z = XPLMGetDataf(velocityZ);
	m_fAxisAngle.roll = XPLMGetDataf(roll_angle), m_fAxisAngle.pitch = XPLMGetDataf(pitch_angle), m_fAxisAngle.heading = XPLMGetDataf(heading_angle);
	m_fMagneticHeadingAngle = XPLMGetDataf(magnetic_heading_angle);

	m_fAccelXYZ.x = XPLMGetDataf(accel_x), m_fAccelXYZ.y = XPLMGetDataf(accel_y), m_fAccelXYZ.z = XPLMGetDataf(accel_z);
	m_fRate.roll = XPLMGetDataf(roll_rate), m_fRate.pitch = XPLMGetDataf(pitch_rate), m_fRate.yaw = XPLMGetDataf(yaw_rate);
	m_fAccelAxis.roll = XPLMGetDataf(accel_roll), m_fAccelAxis.pitch = XPLMGetDataf(accel_pitch), m_fAccelAxis.yaw = XPLMGetDataf(accel_yaw);
}

void cPlugin::ArmCmd()
{
	_weaponCount = XPLMGetDatai(weaponCount);			//ÃÑ ¹«Àå °¡´É °¹¼ö
	XPLMGetDatavi(type, _type, 0, 14);
	XPLMGetDatavi(actionMode, _actionMode, 0, 14);
	_selecWeapon = XPLMGetDatai(selecWeapon);

	// chaff, flare Count Info
	_chaffCount = XPLMGetDatai(chaff_now);
	_flareCount = XPLMGetDatai(flare_now);
	_gunCount = XPLMGetDatai(gun_now);
}

void cPlugin::UTCTime()
{
	GetSystemTime(&m_UTCTime);
}

int cPlugin::WeaponMovingLine(XPLMDrawingPhase inPhase, int inIsBefore, void* inRefcon) 
{
	auto empty = g_map_num_vecReceiveWeaponPos.empty();
	if (empty) return 1;
	
	map_num_receiveWeaponPos::iterator mapIter = g_map_num_vecReceiveWeaponPos.begin();
	for (mapIter; mapIter != g_map_num_vecReceiveWeaponPos.end(); mapIter++) {
		vec_ReceiveWeaponPos::iterator iter = mapIter->second.begin();
		
		glColor4f(1, 0, 0, 1);
		glLineWidth(5.0f);
		glBegin(GL_LINE_STRIP);
		
		for (iter; iter != g_map_num_vecReceiveWeaponPos[mapIter->first].end(); iter++) {
			glVertex3d(iter->x, iter->y, iter->z);
		}

		glEnd();
	}
	

	return 1;
}

void cPlugin::ReceiveWeaponPos(const double lat, const double lon, const double alt, const int num /*= 0*/)
{
	Double3 coord;
	XPLMWorldToLocal(lat, lon, alt, &coord.x, &coord.y, &coord.z);

	AddWeaponPosXYZ(coord, num); // if you need Multi Draw line. input the number
}

void cPlugin::ReceiveWeaponPos(const Double3 pos, const int num /*= 0*/)
{
	Double3 coord;
	XPLMWorldToLocal(pos.lat, pos.lon, pos.alt, &coord.x, &coord.y, &coord.z);

	AddWeaponPosXYZ(coord, num); // if you need Multi Draw line. input the number
}

inline void cPlugin::AddWeaponPosXYZ(const Double3 location, const int num)
{
	static double alt = 0;
	static int Compare = 0;

	if (!g_map_num_vecReceiveWeaponPos.empty()) {
		Compare = (alt > location.z) ? alt - location.z : location.z - alt;
		if (100 < Compare) { return; }
		g_map_num_vecReceiveWeaponPos[num].push_back(location);
		alt = location.z;
	}
	else {
		g_map_num_vecReceiveWeaponPos[num].push_back(location);
		alt = location.z;
	}
}

packet_41 cPlugin::Get41Packet()
{
	packet_41 temp;
	temp.ID = 41;
	temp.time = m_UTCTime;
	temp.position.lat = m_dbLatitude;
	temp.position.lon = m_dbLongitude;
	temp.position.alt = m_dbAltitude;
	temp.velocity = m_fVelocityXYZ;
	temp.axis_angle = m_fAxisAngle;
	temp.magnetic_heading = m_fMagneticHeadingAngle;
	temp.acceleration = m_fAccelXYZ;
	temp.axis_rate = m_fRate;
	temp.angular_acceleration = m_fAccelAxis;
	return temp;
}
