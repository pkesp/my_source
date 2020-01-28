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
    private Vector3 _pos;

    private ImportDllData _dllInstance;
    private TerrainCollision _terrainCollision;
    private Communicator _commu;
    
    private int _leftRight;
    private int _maxIdx;
    private float _totalStoryTime;
    
    private bool _bInput;
    private bool _bTutorialComplete;
    private bool _bTutorialStart;

    private string[] _story;

    private IEnumerator _coroutine;
    

    // Start is called before the first frame update
    void Start()
    {
        _bInput = false;
        _totalStoryTime = 0f;
        _myColor = _text.color;
        _leftRight = 0;
        _bTutorialComplete = false;
        _bTutorialStart = false;

        _story = new string[24] { 
            "패러글라이더 튜토리얼을 시작하겠습니다.",                                                 // 0
            "앞서 설명 들었겠지만 패러글라이더는 조종줄을 잡아 당기고 놓아주고 하면서 조종하게 됩니다.",  // 1
            "기본 원리는 잡아당긴 방향의 공기 저항을 많이 받게 하면서 속도가 줄어들고 높아지게 됩니다.",  // 2
            "본격적으로 튜토리얼에 들어가기에 앞서",                                                   // 3
            "조종줄을 너무 세게 잡아당기지 말아주시기 바랍니다.",                                       // 4
            "왼쪽 조종줄을 잡아당겨 보십시오.",                                                        // 5(왼쪽으로 이동하는 모션이 취해지고 난 후)
            "잘 하셨습니다.",                                                                         // 6
            "이번에는 오른쪽 조종줄을 잡아당겨 보십시오.",                                              // 7(오른쪽으로 이동하는 모션이 취해지고 난 후)
            "잘 하셨습니다.",                                                                         // 8
            "이번에는 양 손을 전부 힘을 빼고 놓아 보십시오.",                                           // 9 (양손의 힘을 풀고 전부 놓아주십시오)
            "잘 하셨습니다.",                                                                         // 10
            "이번에는 양 손의 조종줄을 전부 아래로 잡아당겨 보십시오",                                  // 11 (양손의 힘을 전부 아래로)
            "사실 착륙을 할 때가 아니면 조종줄을 전부 잡아 당겨서 끝까지 내리는 것은"                    // 12
            , "매우 위험한 행동입니다."                                                               // 13
            , "왜냐하면 많이 잡아당길 수록 잡아당긴 방향의 공기 저항은 높아지게 됩니다.",                // 14
            "그러니 속도는 더 느려지고 속도가 0이 되어버리면"                                           // 15
            , "제어력을 상실한 패러글라이더는 힘 없이 떨어지게 됩니다."                                 // 16
            , "이것은 마치 종이 비행기를 힘차게 날리는 것과 비슷합니다."                                // 17
            , "종이 비행기를 날리면 힘차게 날아가는 모습을 볼 수 있지만"                                // 18
            , "종이 비행기를 손에서 잡은 상태로 그냥 놓아버렸다고 생각해 보십시오"                       // 19
            , "종이 배행기는 자세를 잡지 못하고 아무렇게나 지면으로 떨어질 것입니다."                    // 20
            , "그러니 실전에서도 착륙할 때가 아니면 절대로 조종줄을 100% 잡아당기는 일은 하지 않습니다."  // 21
            , "그리고 이 시뮬레이터에서는 100% 줄을 잡아당겼다고 해도 추락하지는 않습니다."              // 22
            , "이것으로 튜토리얼을 마치겠습니다."                                                      // 23
        };

        _dllInstance = _paraObj.GetComponent<ImportDllData>();
        _terrainCollision = _paraObj.GetComponent<TerrainCollision>();
        _commu = GameObject.FindGameObjectWithTag("Communicator").GetComponent<Communicator>();

        _maxIdx = _story.Length - 1;
        _pos = _paraObj.transform.position;

        _coroutine = TutorialCoroutine();
        StartCoroutine(_coroutine);
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
                case 0:         // left input 70. 20
                    {
                        if (70f <= _dllInstance.GetLeftLinePercentage() && _dllInstance.GetRightLinePercentage() <= 50f)
                        {
                            _totalStoryTime += Time.deltaTime;
                        }
                    }
                    break;

                case 1:         // right input 70, 20
                    {
                        if (70f <= _dllInstance.GetRightLinePercentage() && _dllInstance.GetLeftLinePercentage() <= 50f)
                        {
                            _totalStoryTime += Time.deltaTime;
                        }
                    }
                    break;

                case 2:         // none left right 10, 10
                    {
                        if (10f >= _dllInstance.GetRightLinePercentage() && _dllInstance.GetLeftLinePercentage() <= 10f)
                        {
                            _totalStoryTime += Time.deltaTime;
                        }
                    }
                    break;

                case 3:         // full left right 90, 90
                    {
                        if (89f <= _dllInstance.GetRightLinePercentage() && _dllInstance.GetLeftLinePercentage() >= 89f)
                        {
                            _totalStoryTime += Time.deltaTime;
                        }
                    }
                    break;
                default:
                    _totalStoryTime += Time.deltaTime;
                    break;
            }
        }

        if (_terrainCollision.GetIsStanding() && !_bTutorialComplete)
        {
            StopAllCoroutines();
            _text.text = "튜토리얼을 완료하지 못했습니다. 다시 하고 싶으시면 양손을 내리십시오.";
            _bInput = true;

            if (89f <= _dllInstance.GetRightLinePercentage() && _dllInstance.GetLeftLinePercentage() >= 89f)
            {
                SceneManager.LoadScene("Tutorial_Scene_00", LoadSceneMode.Single);
            }
        }

    }

    private void StoryTimeControlFunction(float delayTime, int IdxNum)
    {
        _text.text = _story[IdxNum];

        _totalStoryTime += Time.deltaTime;
    }

    public IEnumerator TutorialCoroutine()
    {
        yield return new WaitUntil(() => _bTutorialStart == true);
        yield return StartCoroutine(WaitPrint(3f, 0));
        yield return StartCoroutine(WaitPrint(3f, 1));
        yield return StartCoroutine(WaitPrint(3f, 2));
        yield return StartCoroutine(WaitPrint(3f, 3));
        yield return StartCoroutine(WaitPrint(3f, 4));
        yield return StartCoroutine(WaitHardwareInputSignal(1f, 0, 5));
        yield return StartCoroutine(WaitPrint(1f, 6));
        yield return StartCoroutine(WaitHardwareInputSignal(1f, 1, 7));
        yield return StartCoroutine(WaitPrint(1f, 8));
        yield return StartCoroutine(WaitHardwareInputSignal(1f, 2, 9));
        yield return StartCoroutine(WaitPrint(1f, 10));
        yield return StartCoroutine(WaitHardwareInputSignal(1f, 3, 11));
        yield return StartCoroutine(WaitPrint(3f, 12));
        yield return StartCoroutine(WaitPrint(3f, 13));
        yield return StartCoroutine(WaitPrint(3f, 14));
        yield return StartCoroutine(WaitPrint(3f, 15));
        yield return StartCoroutine(WaitPrint(3f, 16));
        yield return StartCoroutine(WaitPrint(3f, 17));
        yield return StartCoroutine(WaitPrint(3f, 18));
        yield return StartCoroutine(WaitPrint(3f, 19));
        yield return StartCoroutine(WaitPrint(3f, 20));
        yield return StartCoroutine(WaitPrint(3f, 21));
        yield return StartCoroutine(WaitPrint(3f, 22));
        yield return StartCoroutine(WaitPrint(3f, 23));

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
        
        _bInput = true;
        yield return new WaitUntil(() => _totalStoryTime >= timeTemp);
        _bInput = false;
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
        StopCoroutine(_coroutine);
    }

    public void StartTutorialCoroutine()
    {
        StartCoroutine(_coroutine);
    }
}
