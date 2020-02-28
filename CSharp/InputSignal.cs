using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InputSignal : MonoBehaviour
{
    public Text _text;
    public GameObject _paraObj;
    private Color _myColor;
    private Vector3 _initPos;
    private Vector3 _initAngle;

    private ImportDllData _dllInstance;
    private TerrainCollision _terrainCollision;
    private Communicator _commu;
    private RingCollisionManager _ringCollision;
    
    private int _leftRight;
    private int _maxIdx;
    private float _totalStoryTime;
    
    private bool _bInput;
    private bool _bTutorialComplete;
    private bool _bTutorialStart;
    private bool _bSceneMove;

    private string[] _story;
    

    // Start is called before the first frame update
    void Start()
    {
        _bInput = false;
        _totalStoryTime = 0f;
        _myColor = _text.color;
        _leftRight = 0;
        _bTutorialComplete = false;
        _bTutorialStart = false;
        _bSceneMove = false;

        _story = new string[12] { 
            "패러글라이더 튜토리얼을 시작하겠습니다.",                                                 // 0
            "앞에 보이는 좌우 세로 막대는",                                                           // 1
            "조종줄을 잡아당긴 만큼 표시해줍니다.",                                                   // 2
            "본격적으로 튜토리얼에 들어가기에 앞서",                                                   // 3
            "조종줄을 너무 세게 잡아당기지 말아주시기 바랍니다.",                                       // 4
            "왼쪽 조종줄을 60% 이상 잡아당겨 보십시오.",                                               // 5(왼쪽으로 이동하는 모션이 취해지고 난 후)
            "잘 하셨습니다.",                                                                         // 6
            "오른쪽 조종줄을 60% 이상 잡아당겨 보십시오.",                                              // 7(오른쪽으로 이동하는 모션이 취해지고 난 후)
            "잘 하셨습니다.",                                                                         // 8           
            "이번에는 양 손의 조종줄을 전부 아래로 잡아당겨 보십시오",                                  // 9 (양손의 힘을 전부 아래로)
            "잘 하셨습니다.",                                                                         // 10                        
            "화살표가 가리키는 링을 통과해 보십시오."                                                // 11 (링 통과해라~)
        };

        _dllInstance = _paraObj.GetComponent<ImportDllData>();
        _terrainCollision = _paraObj.GetComponent<TerrainCollision>();
        _commu = GameObject.FindGameObjectWithTag("Communicator").GetComponent<Communicator>();
        _ringCollision = Camera.main.GetComponent<RingCollisionManager>();

        _maxIdx = _story.Length - 1;
        _initPos = _paraObj.transform.position;
        _initAngle = _paraObj.transform.eulerAngles;


        StartCoroutine(TutorialCoroutine());

        Utils.stPacket_Button packet;
        packet.id = 0;
        packet.value = 0;
        _commu.SendPacket(packet);
    }

    // Update is called once per frame
    void Update()
    {
        if (_commu.GetSceneStart())
        {
            _bTutorialStart = true;
        }
        
        #region PreVer.1
        //if (!_bInput) _time += Time.deltaTime;


        //if (_time < 3f)
        //{
        //    _text.text = "튜토리얼을 시작하겠습니다.";
        //}
        //else if (_time < 6f)
        //{
        //    _text.text = "왼쪽 조종줄을 당기면 왼쪽으로 회전할 수 있습니다.";
        //}
        //else if (_time < 9f)
        //{
        //    _text.color = new Color(1f, 0f, 1f);
        //    _text.text = "왼쪽 조종줄을 당겨 왼쪽으로 회전해 보십시오.";
        //    _bInput = true;
        //}
        //else if (_time < 12f)
        //{
        //    _text.color = _myColor;
        //    _text.text = "오른쪽 조종줄을 당기면 오른쪽으로 회전 할 수 있습니다.";
        //}
        //else if (_time < 14f)
        //{
        //    _text.color = new Color(1f, 0f, 1f);
        //    _text.text = "오른쪽 조종줄을 당겨 오른쪽으로 회전해 보십시오.";
        //    _bInput = true;
        //}
        //else if (_time < 17f)
        //{
        //    _text.color = _myColor;
        //    _text.text = "튜토리얼이 완료 되었습니다. 플레이 화면으로 넘어갑니다.";
        //    if (!_onece) {
        //        SceneManager.LoadScene(1);
        //        _onece = !_onece;
        //    }
        //}

        //if (_bInput)
        //{
        //    if (80f < _dllInstance.GetLeftLinePercentage() && _dllInstance.GetRightLinePercentage() < 20f && _time < 9f)
        //    {
        //        _time += Time.deltaTime;
        //        if (_time > 9f) _bInput = false;
        //    }

        //    if (80f < _dllInstance.GetRightLinePercentage() && _dllInstance.GetLeftLinePercentage() < 20f && (12f < _time) && (_time < 14f))
        //    {
        //        _time += Time.deltaTime;
        //        if (_time > 14f) _bInput = false;
        //    }
        //}
        #endregion

        if (_bInput)
        {
            switch (_leftRight)
            {
                case 0:         // left input 60. 20
                    {
                        if (60f <= _dllInstance.GetLeftLinePercentage() && _dllInstance.GetRightLinePercentage() <= 30f)
                        {
                            _totalStoryTime += Time.deltaTime;
                        }
                    }
                    break;

                case 1:         // right input 60, 20
                    {
                        if (60f <= _dllInstance.GetRightLinePercentage() && _dllInstance.GetLeftLinePercentage() <= 30f)
                        {
                            _totalStoryTime += Time.deltaTime;
                        }
                    }
                    break;

                case 2:         // full left right 90, 90
                    {
                        if (89f <= _dllInstance.GetRightLinePercentage() && _dllInstance.GetLeftLinePercentage() >= 89f)
                        {
                            _totalStoryTime += Time.deltaTime;
                        }
                    }
                    break;

                case 3:         // pass the ring
                    {
                        _totalStoryTime += Time.deltaTime;
                    }
                    break;
                default:
                    break;
            }
        }

        
        if (_bTutorialComplete && _terrainCollision.GetIsStanding() && !_bSceneMove)
        {
            _totalStoryTime++;
            _text.text = "전체 "+ _ringCollision.GetMaxRingCount() + "개 중에서 " + _ringCollision.GetColliCount() + "개 통과하셨습니다. 튜토리얼을 마칩니다.";
            StartCoroutine(_commu.MyLoadScene(1, 4f));
            _bSceneMove = true;
        }
        else if (_terrainCollision.GetIsStanding() && !_bTutorialComplete && !_bSceneMove)
        {
            StopAllCoroutines();
            _text.text = "다시 하고 싶으시면 양손을 내리십시오.";
            _bInput = true;

            if (89f <= _dllInstance.GetRightLinePercentage() && _dllInstance.GetLeftLinePercentage() >= 89f)
            {
                StartCoroutine(_commu.MyLoadScene(0, 4f));
            }
            _bSceneMove = true;
        }

    }

    //public IEnumerator MyLoadScene(int loadSceneNum)
    //{
    //    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(loadSceneNum, LoadSceneMode.Single);
    //    Utils.stPacket_Button packet;
    //    packet.id = (Utils.PacketID)loadSceneNum;
    //    packet.value = 0;
    //    _commu.SendPacket(packet);

    //    while (!asyncLoad.isDone)
    //    {
    //        yield return null;
    //    }
    //}

    private void StoryTimeControlFunction(float delayTime, int IdxNum)
    {
        _text.text = _story[IdxNum];

        _totalStoryTime += Time.deltaTime;
    }

    public IEnumerator TutorialCoroutine()
    {
        yield return new WaitUntil(() => _bTutorialStart == true);
        //yield return StartCoroutine(WaitPrint(3f, 0));
        //yield return StartCoroutine(WaitPrint(3f, 1));
        //yield return StartCoroutine(WaitPrint(3f, 2));
        //yield return StartCoroutine(WaitPrint(3f, 3));
        //yield return StartCoroutine(WaitPrint(3f, 4));
        //yield return StartCoroutine(WaitHardwareInputSignal(1f, 0, 5));
        //yield return StartCoroutine(WaitPrint(1f, 6));
        //yield return StartCoroutine(WaitHardwareInputSignal(1f, 1, 7));
        //yield return StartCoroutine(WaitPrint(1f, 8));
        //yield return StartCoroutine(WaitHardwareInputSignal(1f, 2, 9));
        //yield return StartCoroutine(WaitPrint(1f, 10));
        yield return StartCoroutine(WaitHardwareInputSignal(5f, 3, 11));

        yield return null;
    }

    

    public IEnumerator WaitPrint(float waitSeconds, int idx)
    {
        _totalStoryTime += waitSeconds;
        _text.text = _story[idx];
        yield return new WaitForSecondsRealtime(waitSeconds);

        if (idx == _maxIdx)
        {
            _bInput = true;
            _bTutorialComplete = true;
        }

        yield return new WaitWhile(() => _commu._bPause == true);
    }

    public IEnumerator WaitHardwareInputSignal(float waitSeconds, int leftRight, int idx)
    {
        float timeTemp = waitSeconds + _totalStoryTime;
        _text.text = _story[idx];
        _leftRight = leftRight;

        if (idx == _maxIdx)
        {
            _bInput = true;
            _bTutorialComplete = true;

            _paraObj.transform.position = _initPos;
            _paraObj.transform.eulerAngles = _initAngle;
        }

        _bInput = true;
        yield return new WaitUntil(() => _totalStoryTime >= timeTemp);

        if (idx != _maxIdx) _bInput = false;
    }

    public bool GetInputSignal()
    {
        return _bInput;
    }

    public bool GetTotorialComplete()
    {
        return _bTutorialComplete;
    }

    public void StopTutorialCoroutine()
    {
        StopCoroutine("TutorialCoroutine");
    }

    public void StartTutorialCoroutine()
    {
        StartCoroutine("TutorialCoroutine");
    }
}
