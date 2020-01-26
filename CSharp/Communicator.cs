using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;


public class Communicator : MonoBehaviour
{
    public static Communicator instance = null;

    private Utils.PacketID _enumPacket;
    private UdpClient _client_recv;
    private UdpClient _client_send;
    
    private IPEndPoint _ipep_sim1_recv_server;
    private IPEndPoint _ipep_ui_send;

    private Thread _thread;

    private WindZone _windZone;

    private Utils.stPacket_Wind _windInfo;
    private Utils.stPacket_HW _hwInfo;

    private int _sceneNum;
    private bool _bSceneStart  { get; set; }

    // ====================== bool variable ================= 
    private bool _bThread;
    private bool _bWind;
    private bool _bPause;
    private bool _bApplicationQuit;

    private bool _bRunningThread;

    private bool _bSceneChange;

    // ======================  =================

    // ====================== 20191120 Add Moon Principal Engineer variable ================= 
    private int _nExceotCnt;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            Debug.Log("Destroy gameObject");
            return;
        }

        Debug.Log("Destroy gameObject after");

        DontDestroyOnLoad(gameObject);
        gameObject.tag = "Communicator";

        _bRunningThread = true;

        _nExceotCnt = 0;    // 191120 added.

        // ipEndPoint
        _ipep_sim1_recv_server = new IPEndPoint(IPAddress.Any, Constants.Port.sim1_recv);
        _ipep_ui_send = new IPEndPoint(IPAddress.Parse(Constants.Address.localAddress), Constants.Port.ui_recv);


        // 통신
        _client_recv = new UdpClient();
        _client_recv.Client.ReceiveTimeout = 3000;
        _client_recv.Client.Bind((EndPoint)_ipep_sim1_recv_server);

        _client_send = new UdpClient();
        _client_send.Connect(_ipep_ui_send);


        // 쓰레드 생성
        _thread = new Thread(ReceiveData);
        _thread.Start();

        _bWind = false;
        _bPause = false;
        _bThread = true;
        _bSceneChange = false;
        _sceneNum = -1;
        _bSceneStart = false;

        _windZone = GameObject.FindGameObjectWithTag("WindZone").GetComponent<WindZone>();
    }

    //. Start is called before the first frame update
    void Start()
    {
        // wind info request
        _windInfo.id = Utils.PacketID.SIMULATOR_WIND_RECV;
        _windInfo.windDegree = 0;
        _windInfo.windForce = 0;

        int size = Marshal.SizeOf(_windInfo);
        byte[] data = new byte[size];
        _windInfo.Convert_byte(data, size);

        Send_Ui(data);

        Debug.Log("SceneLoadComplete()");

        SceneManager.sceneLoaded += SceneLoadComplete;
    }

    void Update()
    {
        if (_bWind)
        {
            _windZone.transform.Rotate(Vector3.up, _windInfo.windDegree - _windZone.transform.rotation.eulerAngles.y, Space.World);
            _windZone.windMain = _windInfo.windForce;

            _bWind = false;
        }

        if (_bPause)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        if (_bApplicationQuit)
        {
            Application.Quit();
            _bApplicationQuit = false;
        }

        if (_bSceneChange)
        {
            StartCoroutine(MyLoadScene(_sceneNum));

            _bSceneChange = false;
        }
    }

    void OnDisable()
    {
    }

    public IEnumerator MyLoadScene(int loadSceneNum)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadSceneNum, LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    void SceneLoadComplete(Scene arg0, LoadSceneMode arg1)
    {
        _windInfo.id = Utils.PacketID.SIMULATOR_WIND_RECV;
        _windInfo.windDegree = 0;
        _windInfo.windForce = 0;

        int size = Marshal.SizeOf(_windInfo);
        byte[] data = new byte[size];
        _windInfo.Convert_byte(data, size);

        Send_Ui(data);
    }

    void Send_Ui(byte[] data)
    {
        _client_send.Send(data, data.Length);
    }

    void ReceiveData()
    {
        _bRunningThread = true;

        while (_bThread)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);                
                byte[] data = _client_recv.Receive(ref anyIP);
                _enumPacket = (Utils.PacketID)BitConverter.ToInt32(data, 0);
                
                // 각 신호에 대한 처리
                switch (_enumPacket)
                {
                    case Utils.PacketID.SIMULATOR_TUTORIAL_SCENE_CHANGE: // 기본 훈련 신호
                        {
                            _sceneNum = 0;
                            _bSceneChange = true;
                            _bSceneStart = false;
                        }
                        break;

                    case Utils.PacketID.SIMULATOR_COMPETITION_SCENE_CHANGE: // 경기 신호
                        {
                            _sceneNum = 1;
                            _bSceneChange = true;
                            _bSceneStart = false;
                        }
                        break;

                    case Utils.PacketID.SIMULATOR_TUTORIAL_SCENE_START:
                        {
                            _bSceneStart = true;
                        }
                        break;

                    case Utils.PacketID.SIMULATOR_COMPETITION_SCENE_START:
                        {
                            _bSceneStart = true;
                        }
                        break;

                    case Utils.PacketID.SIMULATOR_MULTI_POSITION_RECV: // 멀티 연결시 상대방의 위치 정보 받기
                        Debug.Log("POSITION_RECV");
                        break;

                    case Utils.PacketID.SIMULATOR_PAUSE_SEND:          // 일시 정지 신호
                        {
                            _bPause = !_bPause;
                        }
                        break;

                    case Utils.PacketID.SIMULATOR_RESULT_SHOW_SEND:    // 결과창 보여줘라 신호
                        Debug.Log("RESULT_SHOW");
                        break;

                    case Utils.PacketID.SIMULATOR_SCORE_SEND:          // 연결 되어 있다면 점수 보내줘라
                        Debug.Log("SCORE");
                        break;

                    case Utils.PacketID.SIMULATOR_SOUND_SEND:          // 받은 사운드 적용 시키는 곳
                        Debug.Log("SOUND");
                        break;

                    case Utils.PacketID.SIMULATOR_STOP_SEND:           // 유니티 종료 신호
                        Debug.Log("STOP");
                        _bThread = false;           // threaad 종료.
                        _bApplicationQuit = true;   // game 종료
                        break;

                    case Utils.PacketID.SIMULATOR_WIND_SEND:           // 바람 정보
                        {
                            Utils.stPacket_Wind _winPacket = new Utils.stPacket_Wind();
                            _winPacket.Convert_ByteToStructure(data);
                            _windInfo.windDegree = _winPacket.windDegree;
                            _windInfo.windForce = _winPacket.windForce;
                            _bWind = true;
                        }
                        break;

                    case Utils.PacketID.SIMULATOR_CONNECTED:
                        {
                            Utils.stPacket_Button packet = new Utils.stPacket_Button();
                            packet.id = Utils.PacketID.SIMULATOR_CONNECTED;
                            packet.value = 0;
                            byte[] data2 = new byte[Marshal.SizeOf(packet)];
                            packet.Convert_byte(data2, data2.Length);

                            Send_Ui(data2);
                        }
                        break;

                    case Utils.PacketID.SIMULATOR_HARDWARE:
                        {
                            Utils.stPacket_HW packet = new Utils.stPacket_HW();
                            packet.Convert_ByteToStructure(data);
                            Debug.Log("############# SIMULATOR1_HARDWARE #############");
                            Debug.Log(packet.left + ", " + packet.right);
                            SetHWSignal(packet);
                        }
                        break;

                    default:
                        // ignored
                        break;
                }

                _nExceotCnt = 0;
            }
            catch (SocketException err)
            {
                Utils.stPacket_Button packet;
                packet.id = Utils.PacketID.SIMULATOR_CONNECTED;
                packet.value = 0;

                byte[] data2 = new byte[Marshal.SizeOf(packet)];
                packet.Convert_byte(data2, data2.Length);
                Send_Ui(data2);
            }
            catch (Exception err)
            {
                Debug.Log(err.ToString());
                _nExceotCnt++;
            }
        }

        _bRunningThread = false;
    }

    public double Get_WindPacket_degree()
    {
        return (double)_windInfo.windDegree;
    }

    public double Get_WindPacket_spd()
    {
        return (double)_windInfo.windForce;
    }

    public bool Get_bWind()
    {
        return _bWind;
    }

    public void Set_bWind(bool bWind)
    {
        _bWind = bWind;
    }

    public Utils.stPacket_HW GetHWsignal()
    {
        return _hwInfo;
    }

    public void SetHWSignal(Utils.stPacket_HW hw)
    {
        _hwInfo = hw;
    }

    public bool GetSceneStart()
    {
        return _bSceneStart;
    }

    public void SetSceneStart(bool value)
    {
        _bSceneStart = value;
    }
}
