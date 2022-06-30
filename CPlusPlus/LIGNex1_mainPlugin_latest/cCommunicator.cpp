#include "cCommunicator.h"


cCommunicator::cCommunicator() : m_nPacketReceiveCnt(0), m_nPacketSendCnt(0), m_winSockSuc(true), m_udpSockCreateSuc(true), m_connectSuc(true)
{
}


cCommunicator::~cCommunicator()
{
}

void cCommunicator::Init()
{
	//윈속 초기화
	WSADATA    wsa;
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
	{
		m_winSockSuc = false;
	}

	//socket(UDP 소켓 생성)
	m_socMC = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if (m_socMC == INVALID_SOCKET) {
		m_udpSockCreateSuc = false;
	}

	char buf[32];
	DWORD bufsize = 32;
	GetUserName(buf, &bufsize);
	std::string path = "C:\\Users\\";
	std::string path2 = "\\Desktop\\X-Plane 11\\Resources\\plugins\\FlightSim_SW\\64\\address.txt";
	path.append(buf);
	path.append(path2);
	std::ifstream infile(path.c_str());

	char addrBuf[32];
	char portBuf[8];

	for (int i = 0; !infile.eof(); i++) {
		if (i == 0)			infile.getline(addrBuf, 32);
		else if (i == 1)	infile.getline(portBuf, 8);
	}

	infile.close();

	unsigned short portNum = (unsigned short)atoi(portBuf);

	//소켓 주소 구조체 초기화
	ZeroMemory(&m_serverAddr, sizeof(m_serverAddr));
	m_serverAddr.sin_family = AF_INET;
	m_serverAddr.sin_port = htons(portNum);
	m_serverAddr.sin_addr.s_addr = inet_addr(addrBuf);

	// ====================================================================================== internal communication part
	m_socInter = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if (m_socInter == INVALID_SOCKET) {
		// 내부
	}

	//내부통신 소켓 구조체 초기화
	ZeroMemory(&m_InterAddr, sizeof(m_InterAddr));
	m_InterAddr.sin_family = AF_INET;
	m_InterAddr.sin_port = htons(9502);
	m_InterAddr.sin_addr.s_addr = inet_addr("127.0.0.1");

	
	// ========================================================================================= connect()
	if (m_winSockSuc && m_udpSockCreateSuc) {
		m_retval = connect(m_socMC, (SOCKADDR *)&m_serverAddr, sizeof(m_serverAddr));
		if (m_retval == SOCKET_ERROR) {
			m_connectSuc = false;
		}

		connect(m_socInter, (SOCKADDR *)&m_InterAddr, sizeof(m_InterAddr));
	}
}

void cCommunicator::Release()
{
	// closesock()
	closesocket(m_socInter);

	closesocket(m_socMC);

	//윈속종료
	WSACleanup();
}

void cCommunicator::MCComm()
{
	// 윈속 초기화 && 소켓 생성 다 잘 됐으면...
	if (m_winSockSuc && m_udpSockCreateSuc && m_connectSuc) {
		SOCKADDR_IN serverAddr;
		int addrlen = sizeof(serverAddr);
		ZeroMemory(m_chPacket, BUFSIZE);

		m_retval = recvfrom(m_socMC, m_chPacket, BUFSIZE, 0, (SOCKADDR *)&serverAddr, &addrlen);

		if (m_retval == SOCKET_ERROR) {
			
		}

		m_nPacketReceiveCnt++;

		Parser(m_chPacket);
	}
}

void cCommunicator::Parser(const char* buf)
{
	static int wpnFireNum = 0;
	int id; 
	memcpy(&id, buf, sizeof(id));

	switch (id)
	{
	case WAEPON_POS_51: 
	{
		packet_51 temp;
		ZeroMemory(&temp, sizeof(temp));
		memcpy(&temp, (packet_51*)buf, sizeof(temp));
		g_plugin->ReceiveWeaponPos(temp.position, wpnFireNum);

		packet_51 temp2;
		ZeroMemory(&temp2, sizeof(temp2));
		temp2.position = temp.position;
		temp2.ID = wpnFireNum; // ID를 발사 신호 갯수로 사용하겠다
		sendto(m_socInter, (char*)&temp2, sizeof(temp2), 0, (SOCKADDR *)&m_InterAddr, sizeof(m_InterAddr));
	}
		break;

	case WEAPON_FIRE_71:
	{
		// 일단 fire 신호 들어오지 않는 것으로 가정
		// 여기서부터는 신호 들어오는 것으로 가정하고 코딩
		wpnFireNum++;

		/*packet_71 temp;
		ZeroMemory(&temp, sizeof(temp));
		memcpy(&temp, (packet_71*)buf, sizeof(temp));

		g_plugin->SetSelectWeapon(temp.selectNum);

		XPLMCommandOnce(g_plugin->GetFireSelectedWeapon());*/
	}
		break;
	}
}

void cCommunicator::PacketGen()
{
	if (m_winSockSuc && m_udpSockCreateSuc && m_connectSuc) {
		packet_41 temp;
		ZeroMemory(&temp, sizeof(temp));
		temp = g_plugin->Get41Packet();

		m_retval = sendto(m_socMC, (char*)&temp, sizeof(temp), 0, (SOCKADDR *)&m_serverAddr, sizeof(m_serverAddr));

		m_nPacketSendCnt++;
	}
	else {
		/*char buffer[512];
		float col_white[] = { 1.0f, 0.0f, 0.0f };

		sprintf(buffer, "Win socket - %d, UDP socket - %d, connect - %d", m_winSockSuc, m_udpSockCreateSuc, m_connectSuc);
		XPLMDrawString(col_white, 500, 540, buffer, NULL, xplmFont_Proportional);*/
	}
}

// Out of Sight
void cCommunicator::CreateSimDataFile()
{

}