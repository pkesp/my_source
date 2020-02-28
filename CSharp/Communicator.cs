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
    private InputSignal _inputSignal = null;

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
    private bool _bSceneStart;

    private int _tempNum;

    // ====================== bool variable ================= 
    private bool _bThread;
    private bool _bWind;
    public bool _bPause { get; set; }
    private bool _bTutorialCoroutine;
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
            return;
        }
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
        _bTutorialCoroutine = false;

        _windZone = GameObject.FindGameObjectWithTag("WindZone").GetComponent<WindZone>();

        // Test
        _tempNum = 0;
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

        if (SceneManager.GetActiveScene().name == "Tutorial_Scene_00")
        {
            _inputSignal = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InputSignal>();
        }
    }

    void Update()
    {
        if (_bWind)
        {
            _windZone.transform.Rotate(Vector3.up, _windInfo.windDegree - _windZone.transform.rotation.eulerAngles.y, Space.World);
            _windZone.windMain = _windInfo.windForce;
        }

        if (_bPause)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        if (_bTutorialCoroutine)
        {
            if (SceneManager.GetActiveScene().name == "Tutorial_Scene_00")
            {
                if (!_bPause) _inputSignal.StartTutorialCoroutine();
                else if (_bPause) _inputSignal.StopTutorialCoroutine();
            }
            _bTutorialCoroutine = false;
        }

        if (_bApplicationQuit)
        {
            Application.Quit();
            _bApplicationQuit = false;
        }

        if (_bSceneChange)
        {
            StartCoroutine(MyLoadScene(_sceneNum, 0f));

            _bSceneChange = false;
        }
    }

    void OnDisable()
    {
    }

    public IEnumerator MyLoadScene(int loadSceneNum, float time)
    {
        yield return new WaitForSecondsRealtime(time);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadSceneNum, LoadSceneMode.Single);
        Utils.stPacket_Button packet;
        packet.id = (Utils.PacketID)loadSceneNum;
        packet.value = 0;
        SendPacket(packet);
        _bSceneStart = false;

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

        if (SceneManager.GetActiveScene().name == "Tutorial_Scene_00")
        {
            _inputSignal = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InputSignal>();
        }
    }

    void Send_Ui(byte[] data)
    {
        if (_client_send != null)
        {
            _client_send.Send(data, data.Length);
        }
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
                            _bPause = false;
                            _bTutorialCoroutine = false;

                            Utils.stPacket_Button packet;
                            packet.id = Utils.PacketID.SIMULATOR_PAUSE_SEND;
                            packet.value = _bPause ? 1 : 0;
                            SendPacket(packet);
                        }
                        break;

                    case Utils.PacketID.SIMULATOR_COMPETITION_SCENE_CHANGE: // 경기 신호
                        {
                            _sceneNum = 1;
                            _bSceneChange = true;
                            _bSceneStart = false;
                            _bPause = false;
                            _bTutorialCoroutine = false;

                            Utils.stPacket_Button packet;
                            packet.id = Utils.PacketID.SIMULATOR_PAUSE_SEND;
                            packet.value = _bPause ? 1 : 0;
                            SendPacket(packet);
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

                    case Utils.PacketID.SIMULATOR_PAUSE_SEND:          // 일시 정지 신호
                        {
                            _bPause = !_bPause;
                            _bTutorialCoroutine = !_bTutorialCoroutine;
                            Utils.stPacket_Button packet;
                            packet.id = Utils.PacketID.SIMULATOR_PAUSE_SEND;
                            packet.value = _bPause ? 1 : 0;
                            packet.Convert_byte(data, data.Length);
                            Send_Ui(data);
                        }
                        break;

                    case Utils.PacketID.SIMULATOR_STOP_SEND:           // 유니티 종료 신호
                        _bThread = false;           // threaad 종료.
                        _bApplicationQuit = true;   // game 종료
                        break;

                    case Utils.PacketID.SIMULATOR_WIND_SEND:           // 바람 정보
                        {
                            Utils.stPacket_Wind _winPacket = new Utils.stPacket_Wind();
                            _winPacket.Convert_ByteToStructure(data);
                            _windInfo.windDegree = _winPacket.windDegree;
                            _windInfo.windForce = _winPacket.windForce;

                            ImportDllData._windDir = _winPacket.windDegree;
                            ImportDllData._windSpd = _winPacket.windForce;

                            if (ImportDllData._windSpd == 0) ImportDllData._windSpd = 2.8;
                            else if (ImportDllData._windSpd == 1) ImportDllData._windSpd = 5.5;
                            else if (ImportDllData._windSpd == 2) ImportDllData._windSpd = 7;

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

    public void SendPacket<T>(T packet) where T : Utils.Convert
    {
        int size = Marshal.SizeOf(packet);
        byte[] data = new byte[size];

        packet.Convert_byte(data, size);

        Send_Ui(data);
    }

    public int GetTempNum()
    {
        return (int)_enumPacket;
    }

    public void SetTempNum(int num)
    {
        _enumPacket = (Utils.PacketID)num;
    }
    
}
