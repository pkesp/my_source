#pragma once

#include "Utill.h"
#include "cPlugin.h"
#include "cDisplay.h"
#include "cCommunicator.h"

cPlugin*		g_plugin	= new cPlugin;
cDisplay*		g_display	= new cDisplay;
cCommunicator*	g_commu		= new cCommunicator;


/***********************************************************************
* Function
***********************************************************************/

static float FlightLoop(float inElapsedSinceLastCall, float inElapsedTimeSinceLastFlightLoop, int inCounter, void * inRefcon);

void CommuStart() {
	//g_commu->PacketGen();
	g_commu->MCComm();
}

void ThreadStart() {
	std::thread t(CommuStart);
	t.detach();
}

//플러그인 시작 함수
PLUGIN_API int XPluginStart(
						char *		outName,
						char *		outSig,
						char *		outDesc)
{
	strcpy(outName, "Flight Sim S/W");
	strcpy(outSig, "xpsdk.examples");
	strcpy(outDesc, "Flight Sim S/W plugin");

	srand(time(NULL));

	g_plugin->Init();
	g_display->Init();
	g_commu->Init();


	/****************************************        콜백함수 등록             ******************************************/
	XPLMRegisterFlightLoopCallback(FlightLoop, 1, NULL);
	
	return 1;
}

// 플러그인 종료시, x-plane 종료 시에 불려시는 함수
PLUGIN_API void	XPluginStop(void)
{
	XPLMUnregisterFlightLoopCallback(FlightLoop, NULL);

	SAFE_DELETE_RELEASE(g_commu);
	SAFE_DELETE_RELEASE(g_display);
	SAFE_DELETE_RELEASE(g_plugin);
}

PLUGIN_API void XPluginDisable(void) { }			// 플러그인 종료시 불려지게 됨.
PLUGIN_API int  XPluginEnable(void)  { return 1; }	// 플러그인 활성화 시
PLUGIN_API void XPluginReceiveMessage(XPLMPluginID inFrom, int inMsg, void * inParam) { }



float FlightLoop(float inElapsedSinceLastCall, float inElapsedTimeSinceLastFlightLoop, int inCounter, void * inRefcon)
{
	g_plugin->Update();
	g_display->Update();
	g_commu->PacketGen();

	ThreadStart();

	return -1.f;
}
