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

//�÷����� ���� �Լ�
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


	/****************************************        �ݹ��Լ� ���             ******************************************/
	XPLMRegisterFlightLoopCallback(FlightLoop, 1, NULL);
	
	return 1;
}

// �÷����� �����, x-plane ���� �ÿ� �ҷ��ô� �Լ�
PLUGIN_API void	XPluginStop(void)
{
	XPLMUnregisterFlightLoopCallback(FlightLoop, NULL);

	SAFE_DELETE_RELEASE(g_commu);
	SAFE_DELETE_RELEASE(g_display);
	SAFE_DELETE_RELEASE(g_plugin);
}

PLUGIN_API void XPluginDisable(void) { }			// �÷����� ����� �ҷ����� ��.
PLUGIN_API int  XPluginEnable(void)  { return 1; }	// �÷����� Ȱ��ȭ ��
PLUGIN_API void XPluginReceiveMessage(XPLMPluginID inFrom, int inMsg, void * inParam) { }



float FlightLoop(float inElapsedSinceLastCall, float inElapsedTimeSinceLastFlightLoop, int inCounter, void * inRefcon)
{
	g_plugin->Update();
	g_display->Update();
	g_commu->PacketGen();

	ThreadStart();

	return -1.f;
}
