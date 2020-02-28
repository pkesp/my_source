using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class ControllerInput : MonoBehaviour
{
    private RectTransform Stick;

    private Vector2 stickFirstPos;
    private Vector3 joyVec;
    private float Radius;
    private float maxAngle;

    float pitchAxis_origin;
    float rollAxis_origin;
    float yawAxis_origin;

    float rollAxis;
    float pitchAxis;

    float rollAxisDelta;
    float pitchAxisDelta;
    float yawAxisDelta;

    private TerrainCollision _terrainColli;

    //
    // James-20.0114 added to use Zyrosensor value.
    //
    private bool m_bUseZyroSensor = true;  // true(zyro sensor 사용), false(sensor 외 사용: 예: 조이스틱)
    private Communicator clsCommu;
    private bool m_bFirstRead = true;

    private float pitchAxisDelta_First; // 변화량 계산을 위함
    private float rollAxisDelta_First;

    private float m_fDeltaPitch;
    private float m_fDeltaRoll;


    void Awake()
    {
        // 연결된 조이스틱 이름 출력
        string[] temp = Input.GetJoystickNames();
        if (temp.Length > 0)
        {
            for (int i = 0; i < temp.Length; ++i)
            {
                if (!string.IsNullOrEmpty(temp[i]))
                {
                    Debug.Log("Controller " + (i + 1) + " is connected using" + temp[i]);
                }
                else
                {
                    Debug.Log("controller " + (i + 1) + " is disconnected.");
                }
            }
        }

        maxAngle = 50f;

        // James-20.0114 added. -> JMH-20.0203 added.
        clsCommu = GameObject.FindGameObjectWithTag("Communicator").GetComponent<Communicator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //if (SceneManager.GetActiveScene().buildIndex == 1)
        //{
        //    Radius = GameObject.Find("Canvas (1)").transform.Find("background").GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        //    Stick = GameObject.Find("Canvas (1)").transform.Find("Stick").GetComponent<RectTransform>();
        //    stickFirstPos = Stick.anchoredPosition;
        //}
        _terrainColli = GameObject.FindGameObjectWithTag("Glider").GetComponent<TerrainCollision>();
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    void FixedUpdate()
    {
        if (m_bUseZyroSensor)
        {            
            //////////////// tested value //////////////////////////
            //
            // 처음 값을 기준으로 변화량에 따른 동작을 수행한다.
            //
            if (m_bFirstRead)
            {
                pitchAxisDelta_First = clsCommu.m_stZyroInfo.pitch;
                rollAxisDelta_First = clsCommu.m_stZyroInfo.roll;

                if (pitchAxisDelta_First != 0.0f && rollAxisDelta_First != 0.0f)
                {
                    m_bFirstRead = false;   // 더 이상 초기 값 설정하지 않기 위한 변수 변경
                }

                // 처음에 받은 값으로 셋팅.
                pitchAxisDelta = 0; //  clsCommu.m_stZyroInfo.pitch;
                rollAxisDelta = 0; // clsCommu.m_stZyroInfo.roll;
                yawAxisDelta = this.transform.eulerAngles.y; // rollAxisDelta;
            }

            m_fDeltaPitch = clsCommu.m_stZyroInfo.pitch - pitchAxisDelta_First;
            m_fDeltaRoll = clsCommu.m_stZyroInfo.roll - rollAxisDelta_First;

            if (Mathf.Abs(m_fDeltaPitch) > 0.0f || Mathf.Abs(m_fDeltaRoll) > 0.0f)
            {
                pitchAxisDelta = (m_fDeltaPitch );
                rollAxisDelta = (m_fDeltaRoll );
                yawAxisDelta += (rollAxisDelta * Time.deltaTime);
            }

            //this.transform.eulerAngles = new Vector3(pitchAxisDelta, -yawAxisDelta, rollAxisDelta);
            this.transform.rotation = Quaternion.Euler(new Vector3(pitchAxisDelta, -yawAxisDelta, rollAxisDelta));

        }
        else // if (m_bUseZyroSensor == false)
        {
            #region 0 Joystic 사용시. original source 
            if (!_terrainColli.GetIsStanding())
            {
                if (Mathf.Abs(Input.GetAxis("Pitch")) > 0.01f ||
                Mathf.Abs(Input.GetAxis("Roll")) > 0.01f)
                {
                    pitchAxis_origin = Input.GetAxis("Pitch");
                    rollAxis_origin = Input.GetAxis("Roll");

                    //Debug.Log("RollAxis : " + rollAxis_origin);

                    rollAxis = rollAxis_origin * Radius;    // James-20.0114 added comment: Radious는 항상 '0' 임.. ?? 
                    pitchAxis = pitchAxis_origin * Radius;

                    //System.String strMsg = "Roll(org: " + rollAxis_origin + ", axis:" + rollAxis + ")" + "Pitch(org: " + pitchAxis_origin + ", axis:" + pitchAxis + ")";
                    //Debug.Log(strMsg);

                    if (SceneManager.GetActiveScene().name == "Competition_Scene_00")
                    {
                        Stick.anchoredPosition = new Vector2(rollAxis, -pitchAxis) + stickFirstPos; 
                    }

                    rollAxisDelta += rollAxis_origin * maxAngle * Time.deltaTime;
                    pitchAxisDelta += pitchAxis_origin * maxAngle * Time.deltaTime;

                    yawAxisDelta += rollAxisDelta * Time.deltaTime;

                    //plane.transform.eulerAngles = new Vector3(-pitchAxisDelta, 0, -rollAxisDelta);
                }

                //if (Mathf.Abs(Input.GetAxis("Yaw")) > 0.05f)
                //{
                //    yawAxis_origin = Input.GetAxis("Yaw");
                //    yawAxisDelta += yawAxis_origin * 15 * Time.deltaTime;

                //    //plane.transform.eulerAngles = new Vector3(0, yawAxisDelta, 0);
                //}

                this.transform.eulerAngles = new Vector3(-pitchAxisDelta, yawAxisDelta, -rollAxisDelta);

                System.String strMsg = "eulerAngles(" + this.transform.eulerAngles.x + "," + this.transform.eulerAngles.y + "," + this.transform.eulerAngles.z + ")";
                Debug.Log(strMsg + "\n");
            }
            #endregion
        }
    }
}
