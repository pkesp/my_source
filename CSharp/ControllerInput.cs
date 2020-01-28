using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerInput : MonoBehaviour
{
    public RectTransform Stick;
    public Transform plane;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        Radius = GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        stickFirstPos = Stick.anchoredPosition;
        _terrainColli = GameObject.Find("ParaGlider2").GetComponent<TerrainCollision>();
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        if (!_terrainColli.GetIsStanding())
        {
            if (Mathf.Abs(Input.GetAxis("Pitch")) > 0.01f ||
                Mathf.Abs(Input.GetAxis("Roll")) > 0.01f)
            {
                pitchAxis_origin = Input.GetAxis("Pitch");
                rollAxis_origin = Input.GetAxis("Roll");

                //Debug.Log("RollAxis : " + rollAxis_origin);

                rollAxis = rollAxis_origin * Radius;
                pitchAxis = pitchAxis_origin * Radius;
                Stick.anchoredPosition = new Vector2(rollAxis, -pitchAxis) + stickFirstPos;

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

            // DLL 사용으로 잠시 묶어둠.
            //plane.eulerAngles = new Vector3(-pitchAxisDelta, yawAxisDelta, -rollAxisDelta);
        }
    }
}
