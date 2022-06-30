#pragma once

#include "Utill.h"

#include <WinSock2.h>
#pragma comment(lib,"ws2_32.lib")

#include "cDisplay.h"
#include "cPlugin.h"

extern cPlugin*		g_plugin;
extern cDisplay*	g_display;

// Add Variable
//cTimer *		_pTimer50;

class cCommunicator
{
private:
	UINT			m_nPacketReceiveCnt;
	UINT			m_nPacketSendCnt;
	char			m_chPacket[BUFSIZE];
	SOCKET			m_socMC;
	SOCKADDR_IN		m_serverAddr;
	int				m_retval;

	bool			m_winSockSuc;
	bool			m_udpSockCreateSuc;
	bool			m_connectSuc;

	SOCKET			m_socInter;
	SOCKADDR_IN		m_InterAddr;


public:
	cCommunicator();
	~cCommunicator();

	void MCComm();					// MC comm
	void Parser(const char* buf);	// packet seperator
	void PacketGen();				// send packet

	//Add Function
	void Init();					// 초기화
	void Release();					// 해제

	// Out of Sight
	void CreateSimDataFile();
};

