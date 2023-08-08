// MavlinkForRelay.cpp : 이 파일에는 'main' 함수가 포함됩니다. 거기서 프로그램 실행이 시작되고 종료됩니다.
//

#include <iostream>
#include <common/mavlink.h>
#include <sys/types.h>
#include <sys/timeb.h>
#include <WS2tcpip.h>


#define BUFFER_LENGTH 2041
#define DELTA_EPOCH_IN_MICROSECS  11644473600000000Ui64


long deltatime = 0;
long oldtime = 0;

int main(int argc, char* argv[])
{
    //std::cout << "Hello World!\n";

	oldtime = clock();

	char target_ip[100];

	struct sockaddr_in gcAddr;
	struct sockaddr_in locAddr;
	uint8_t buf[BUFFER_LENGTH];
	int recsize;
	int fromlen = sizeof(gcAddr);
	long timer[255] = { 0 };

	uint8_t mavtype = 0;


	if(argc >= 2)
		strcpy_s(target_ip, argv[1]);
	else
		strcpy_s(target_ip, "127.0.0.1");

	WSADATA wsaData;
	if (0 != WSAStartup(MAKEWORD(2, 2), &wsaData))
	{
		perror("WSA Init failed");
	}

	int sock = socket(PF_INET, SOCK_DGRAM, IPPROTO_UDP);

	memset(&locAddr, 0, sizeof(locAddr));
	locAddr.sin_family = AF_INET;
	locAddr.sin_addr.s_addr = INADDR_ANY;
	locAddr.sin_port = htons(14551);

	if (-1 == bind(sock, (struct sockaddr*)&locAddr, sizeof(struct sockaddr)))
	{
		perror("error bind failed");
		closesocket(sock);
		exit(EXIT_FAILURE);
	}

	

	memset(&gcAddr, 0, sizeof(gcAddr));
	gcAddr.sin_family = AF_INET;
	inet_pton(AF_INET,target_ip, &(gcAddr.sin_addr.s_addr));
	gcAddr.sin_port = htons(14551);


	while(true)
	{
		deltatime = clock() - oldtime;
		oldtime = clock();

		memset(buf, 0, BUFFER_LENGTH);
		recsize = recvfrom(sock, (char*)buf, BUFFER_LENGTH, 0, (struct sockaddr*)&gcAddr, &fromlen);
		if (recsize > 0)
		{
			mavlink_message_t msg;
			mavlink_status_t status;

			//printf("Bytes Received: %d\n", (int)recsize);
			for (int i = 0; i < recsize; ++i)
			{
				if (mavlink_parse_char(MAVLINK_COMM_0, buf[i], &msg, &status))
				{
					//printf("Received packet: SYS: %d, COMP: %d, LEN: %d, MSG ID: %d\n", msg.sysid, msg.compid, msg.len, msg.msgid);

					if (timer[msg.msgid] > 3000)
					{
						timer[msg.msgid] = 0;
						switch (msg.msgid)
						{
						case MAVLINK_MSG_ID_GLOBAL_POSITION_INT:
							mavlink_position_target_global_int_t pos;
							mavlink_msg_position_target_global_int_decode(&msg, &pos);
							printf("Msg Type : Position\n");
							printf("X :  %d, Y : %d, Z : %f\n", pos.lat_int, pos.lon_int, pos.alt);
							break;
						case MAVLINK_MSG_ID_ATTITUDE_QUATERNION:
							mavlink_attitude_quaternion_t rot;
							mavlink_msg_attitude_quaternion_decode(&msg, &rot);
							printf("Msg Type : Rotation\n");
							printf("W :  %f, X : %f, Y : %f, Z : %f\n", rot.q1, rot.q2, rot.q3, rot.q4);
							break;
							/*case MAVLINK_MSG_ID_ATTITUDE_QUATERNION_COV:
								mavlink_attitude_quaternion_cov_t covt
								break;*/
						case MAVLINK_MSG_ID_POSITION_TARGET_LOCAL_NED:
							mavlink_position_target_local_ned_t local_ned;
							mavlink_msg_position_target_local_ned_decode(&msg, &local_ned);
							printf("Msg Type : Velocity\n");
							printf("X : %f, Y : %f, Z : %f\n", local_ned.afx, local_ned.afy, local_ned.afz);
							break;
						case MAVLINK_MSG_ID_POSITION_TARGET_GLOBAL_INT:
							mavlink_position_target_global_int_t target_global;
							mavlink_msg_position_target_global_int_decode(&msg, &target_global);
							printf("Msg Type : Position & Velocity\n");
							printf("X :  %d, Y : %d, Z : %f\n", target_global.lat_int, target_global.lon_int, target_global.alt);
							printf("X : %f, Y : %f, Z : %f\n", target_global.afx, target_global.afy, target_global.afz);
							break;
						case MAVLINK_MSG_ID_SERVO_OUTPUT_RAW:
							if (mavtype != 0)
							{
								mavlink_servo_output_raw_t output_raw;
								mavlink_msg_servo_output_raw_decode(&msg, &output_raw);
								printf("Msg Type : Rotor\n");
								if (mavtype == MAV_TYPE_QUADROTOR)
								{
									printf("1 :  %d, 2 : %d, 3 : %d, 4 : %d\n", output_raw.servo1_raw, output_raw.servo2_raw, output_raw.servo3_raw, output_raw.servo4_raw);
								}
								else if (mavtype == MAV_TYPE_HEXAROTOR)
								{
									printf("1 :  %d, 2 : %d, 3 : %d, 4 : %d, 5 : %d, 6 : %d\n", output_raw.servo1_raw, output_raw.servo2_raw, output_raw.servo3_raw, output_raw.servo4_raw, output_raw.servo5_raw, output_raw.servo6_raw);
								}
								else if (mavtype == MAV_TYPE_OCTOROTOR)
								{
									printf("1 :  %d, 2 : %d, 3 : %d, 4 : %d, 5 : %d, 6 : %d, 7 : %d, 8 : %d\n", output_raw.servo1_raw, output_raw.servo2_raw, output_raw.servo3_raw, output_raw.servo4_raw, output_raw.servo5_raw, output_raw.servo6_raw, output_raw.servo7_raw, output_raw.servo8_raw);
								}
							}
							break;
						case MAVLINK_MSG_ID_HEARTBEAT:
							mavlink_heartbeat_t heartbeat;
							mavlink_msg_heartbeat_decode(&msg, &heartbeat);
							mavtype = heartbeat.type;
							printf("Msg Type : Heartbeat\n");
							printf("Type :  %ld\n", heartbeat.type);
							break;
						}
						//printf("\n");
					}
					
				}
			}
		}
		memset(buf, 0, BUFFER_LENGTH);

		for (int j = 0; j < 255; ++j)
			timer[j] += deltatime;

		Sleep(1);
	}
    
}
