using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Utils : MonoBehaviour
{
    public enum PacketID
    {
        SIMULATOR_TUTORIAL_SCENE_CHANGE = 0,   //  0 // value 값으로 설정입력
        SIMULATOR_COMPETITION_SCENE_CHANGE,    //  1 // value 값으로 설정입력
        SIMULATOR_TUTORIAL_SCENE_START,        //  2 // 
        SIMULATOR_COMPETITION_SCENE_START,     //  3 // 
        SIMULATOR_STOP_SEND,                   //  4 // 시뮬레이터 종료
        SIMULATOR_PAUSE_SEND,                  //  5 // 시뮬레이터 일시 중지(재입력시 resume)
        SIMULATOR_RESULT_SHOW_SEND,            //  6 // 결과창을 띄워라
        SIMULATOR_SCORE_SEND,                  //  7 // 운용자 프로그램과 시뮬레이터2에게 점수 공유
        SIMULATOR_MULTI_POSITION_RECV,         //  8 // 시뮬레이터2 위치 공유
        SIMULATOR_SOUND_SEND,                  //  9 // 사운드 0 ~ 100 값
        SIMULATOR_WIND_SEND,                   // 10 // 바람 방향과 세기 전송
        SIMULATOR_WIND_RECV,                   // 11 // 현재 설정되어 있는 바람 방향과 세기 요청 신호
        SIMULATOR_CONNECTED,                   // 12 // Sim1 연결 돼있는지 보내고 받고
        SIMULATOR_TIME,                        // 13 // 
        SIMULATOR_HARDWARE,                    // 14 
        SIMULATOR_PAN,                         // 15 
        SIMULATOR_MOTOR,                       // 16 //
        SIMULATOR_RISING_WIND                  // 17
    }

    public interface Convert
    {
        void Convert_byte(byte[] data, int size);   // data 변수에 구조체 값을 넣어줌
        void Convert_ByteToStructure(byte[] data);  // data 변수에 있는 값을 구조체에 넣어줌
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct stPacket_rising : Convert
    {
        public PacketID id;
        public bool wind;

        public void Convert_byte(byte[] data, int size)
        {
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, ptr, false);
            Marshal.Copy(ptr, data, 0, size);
            Marshal.FreeHGlobal(ptr);
        }

        public void Convert_ByteToStructure(byte[] data)
        {
            int size = Marshal.SizeOf(this);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, ptr, size);
            this = (stPacket_rising)Marshal.PtrToStructure(ptr, typeof(stPacket_rising));
            Marshal.FreeHGlobal(ptr);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct stPacket_HW : Convert
    {
        public PacketID id;
        public int left;
        public int right;

        public void Convert_byte(byte[] data, int size)
        {
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, ptr, false);
            Marshal.Copy(ptr, data, 0, size);
            Marshal.FreeHGlobal(ptr);
        }

        public void Convert_ByteToStructure(byte[] data)
        {
            int size = Marshal.SizeOf(this);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, ptr, size);
            this = (stPacket_HW)Marshal.PtrToStructure(ptr, typeof(stPacket_HW));
            Marshal.FreeHGlobal(ptr);
        }
    }

    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct stPacket_Setting : Convert
    {
        public PacketID id;
        public int windSpd;         // 풍속
        public int windDeg;         // 풍향

        // 매개변수 data 에 구조체 값을 넘겨줌
        public void Convert_byte(byte[] data, int size)
        {
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, ptr, false);
            Marshal.Copy(ptr, data, 0, size);
            Marshal.FreeHGlobal(ptr);
        }

        // 매개변수로 받은 data를 구조체에 넣어줌
        public void Convert_ByteToStructure(byte[] data)
        {
            int size = Marshal.SizeOf(this);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, ptr, size);
            this = (stPacket_Setting)Marshal.PtrToStructure(ptr, typeof(stPacket_Setting));
            Marshal.FreeHGlobal(ptr);
        }
    }

    // 간단한 버튼 신호는 ID 값만으로 처리 value 값은 사운드신호까지 쓰기 위함.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct stPacket_Button : Convert
    {
        public PacketID id;
        public int value;

        public void Convert_byte(byte[] data, int size)
        {
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, ptr, false);
            Marshal.Copy(ptr, data, 0, size);
            Marshal.FreeHGlobal(ptr);
        }

        public void Convert_ByteToStructure(byte[] data)
        {
            int size = Marshal.SizeOf(this);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, ptr, size);
            this = (stPacket_Button)Marshal.PtrToStructure(ptr, typeof(stPacket_Button));
            Marshal.FreeHGlobal(ptr);
        }
    }

    // 멀티 연결 시 상대방의 위치 정보를 받기 위한 구조체. 아직 완성되지 않음. 에니메이션 정보를 모름.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct stPacket_Position
    {
        public PacketID id;
        public double x;
        public double y;
        public double z;
        
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct stPacket_Wind : Convert
    {
        public PacketID id;
        public float windForce;
        public float windDegree;

        public void Convert_byte(byte[] data, int size)
        {
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, ptr, false);
            Marshal.Copy(ptr, data, 0, size);
            Marshal.FreeHGlobal(ptr);
        }

        public void Convert_ByteToStructure(byte[] data)
        {
            int size = Marshal.SizeOf(this);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, ptr, size);
            this = (stPacket_Wind)Marshal.PtrToStructure(ptr, typeof(stPacket_Wind));
            Marshal.FreeHGlobal(ptr);
        }
    }
}
