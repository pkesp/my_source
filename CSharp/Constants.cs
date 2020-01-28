using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants 
{
    public struct Mode
    {
        public const int PRACTICE = 0x100000; // 연습모드
        public const int COMPETITION = 0x200000; // 경기장소
        public const int COMPETITION2 = 0x400000; // 경기장소2
    }

    public struct Weather
    {
        public const int CLEAN = 0x1000; // 눈
        public const int CLOUD = 0x2000; // 비
        public const int RAIN = 0x4000; // 구름
        public const int SNOW = 0x8000; // 맑음
    }

    public struct Wind
    {
        public const int EMPTY = 0x1000; // 무풍
        public const int MILD = 0x2000; // 약풍
        public const int MID = 0x4000; // 중풍
        public const int STRONG = 0x8000; // 강풍
    }

    public struct Level
    {
        public const int EASY = 0x100; // 쉬움
        public const int NORMAL = 0x200; // 보통
        public const int HARD = 0x400; // 어려움
        public const int INSANE = 0x800; // 매우 어려움
    }

    public struct Port
    {
        public const int ui_recv = 19401;
        public const int ui_send = 19402;

        public const int sim1_recv = 19501;
        public const int sim1_send = 19502;
    }

    public struct Address
    {
        public const string localAddress = "127.0.0.1";
    }
}
