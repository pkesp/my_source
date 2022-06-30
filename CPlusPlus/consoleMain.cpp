#define _CRT_SECURE_NO_WARNINGS
#define WIN32_LEAN_AND_MEAN

#include <Windows.h>
#include <WinSock2.h>
#pragma comment(lib,"ws2_32.lib")

#include <iostream>
#include <fstream>
#include <string>
#include <time.h>

using namespace std;

enum enum_Packet
{
	// ������� -> ETU 41 ~ 49 flight
	UTC_TIME_41 = 41,
	POSITION,
	VELOCITY,
	MOTION,

	// ETU -> ������� 51 ~ 59 weaponInfo
	WAEPON_POS_51 = 51,
	WEAPON_RELOAD_52,


	// ������� -> MC 61 ~ 69 
	TOTAL_CNT_61 = 61, //total packet count
	WEAPON_INFO_62,	//name, remain
	INVENTORY_READY_63,


	// MC -> ������� 71 ~ 79 fire
	WEAPON_FIRE_71 = 71	//fireCmd
};

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
}Float3, *pFloat3;

typedef struct st_Packet41 {
	int ID;
	SYSTEMTIME time;
	Double3 position;
	Float3	velocity;
	Float3	axis_angle;
	float	magnetic_heading;
	Float3	acceleration;
	Float3	axis_rate;
	Float3	angular_acceleration;
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

#pragma pack(pop)

int main() 
{
	srand(time(NULL));

	//���� �ʱ�ȭ
	WSADATA    wsa;
	if (WSAStartup(MAKEWORD(2, 2), &wsa) != 0)
		return -1;

	//socket(UDP ���� ����)
	SOCKET socMC = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if (socMC == INVALID_SOCKET) cout << "socket creation fail" << endl;

	SOCKADDR_IN serverAddr;
	SOCKADDR_IN clientAddr;
	int retval;

	// address.txt START
	ifstream infile("address.txt");
	string str;

	char addrBuf[32];
	char portBuf[8];

	if ( infile.is_open() )	cout << "file open success" << endl;
	else					cout << "file open failure" << endl;

	for ( int i = 0; !infile.eof(); i++ ) {
		if ( i == 0 )		infile.getline(addrBuf, 32);
		else if ( i == 1 )	infile.getline(portBuf, 8);
	}
	unsigned short portNum = (unsigned short)atoi(portBuf);
	cout << addrBuf << endl;
	cout << portNum << endl;


	//���� �ּ� ����ü �ʱ�ȭ
	ZeroMemory(&serverAddr, sizeof(serverAddr));
	serverAddr.sin_family = AF_INET;
	serverAddr.sin_port = htons(portNum);
	serverAddr.sin_addr.s_addr = htonl(INADDR_ANY);

	// connect()
	retval = bind(socMC, (SOCKADDR *)&serverAddr, sizeof(serverAddr));
	if (retval == SOCKET_ERROR) {
		cout << "bind errorrrrrrr" << endl;
		int num = WSAGetLastError();
		cout << num << endl;
		return -1;
	}
	else {
		cout << "bind success" << endl;
	}
	
	infile.close();
	// address.txt END

	// position.txt START
	infile.open("position.txt", ios::in);

	if ( infile.is_open() )	cout << "position open seccess" << endl;
	else					cout << "position open failure" << endl;

	static int num = 0;

	cout.precision(15);

	while (true) {
		int addrlen = sizeof(clientAddr);
		char chPacket[512];
		ZeroMemory(chPacket, 512);

		retval = recvfrom(socMC, chPacket, 512, 0, (SOCKADDR *)&clientAddr, &addrlen);

		if (retval == SOCKET_ERROR){
			cout << "recv error" << endl;
			continue;
		}

		int id;
		memcpy(&id, chPacket, sizeof(id));

		switch (id)
		{
		case UTC_TIME_41 :
		case POSITION :
		case VELOCITY :	
		case MOTION :
		{
			packet_41 recv41;
			memcpy(&recv41, (packet_41*)chPacket, sizeof(recv41));
			cout << num << "�� ° " << "ID : " << recv41.ID << " �� ��Ŷ ����" << endl;
			/*cout << "acceler x : " << recv41.acceleration.x << endl;
			cout << "acceler y : " << recv41.acceleration.y << endl;
			cout << "acceler z : " << recv41.acceleration.z << endl;
			cout << "angular_acceleration x : " << recv41.angular_acceleration.x << endl;
			cout << "angular_acceleration y : " << recv41.angular_acceleration.y << endl;
			cout << "angular_acceleration z : " << recv41.angular_acceleration.z << endl;
			cout << "axis_angle x : " << recv41.axis_angle.x << endl;
			cout << "axis_angle y : " << recv41.axis_angle.y << endl;
			cout << "axis_angle z : " << recv41.axis_angle.z << endl;
			cout << "axis_rate x : " << recv41.axis_rate.x << endl;
			cout << "axis_rate y : " << recv41.axis_rate.y << endl;
			cout << "axis_rate z : " << recv41.axis_rate.z << endl;*/
			cout << "magnetic_heading : " << recv41.magnetic_heading << endl;
			cout << "position.lat : " << recv41.position.x << endl;
			cout << "position.lon : " << recv41.position.y << endl;
			cout << "position.alt : " << recv41.position.z << " ftmsl" << endl;
			/*cout << "time.wHour : " << recv41.time.wHour << endl;
			cout << "velocity.x : " << recv41.velocity.x << endl;
			cout << "velocity.y : " << recv41.velocity.y << endl;
			cout << "velocity.z : " << recv41.velocity.z << endl << endl;*/

			if (recv41.ID == 41) {
				 Double3 position;
				 int pos;
				 string _str;
				 getline(infile, str);
				 /*if (infile.eof()) {
				 infile.seekg(0, ios::beg);
				 num++;
				 }*/

				 pos = str.find(',', 0);
				 _str = str.substr(0, pos);
				 position.x = atof(_str.c_str());
				 str.erase(0, pos + 1);

				 pos = str.find(',', 0);
				 _str = str.substr(0, pos);
				 position.y = atof(_str.c_str());
				 str.erase(0, pos + 1);

				 position.z = atof(str.c_str());

				 /*for (int j = 0; j < 3; j++) {
					 cout << position.coordi[j] << ", ";
				 }
				 cout << endl;*/

				 packet_51 temp;
				 temp.ID = 51;
				 temp.position = position;

				 char buf[512];
				 memcpy(buf, &temp, sizeof(temp));
				
				 if (!infile.eof()) {
					 retval = sendto(socMC, (char*)&temp, sizeof(temp), 0, (SOCKADDR *)&clientAddr, sizeof(clientAddr));
					 //cout << "retval : " << retval << endl;
				 }
				 system("cls");

			}
		}
			break;

		case INVENTORY_READY_63: // 63 recv -> 52 send
		{
			packet_52 send52;
			send52.ID = 52;
			for (int i = 0; i < 12; i++) {
				send52.weapon[i] = rand() % 19;
			}
			sendto(socMC, (char*)&send52, sizeof(send52), 0, (SOCKADDR *)&clientAddr, sizeof(clientAddr));
		}
			break;
		default:
			cout << "undefine packet recv" << endl;
			//return -1;
			break;
		}

		printf("[UDP\%s:%d] \n", inet_ntoa(clientAddr.sin_addr), ntohs(clientAddr.sin_port));

		

		//infile.seekg(0, ios::beg);

	}

	infile.close();
	// position.txt END

	// closesock()
	closesocket(socMC);

	//��������
	WSACleanup();

	return 0;
}
