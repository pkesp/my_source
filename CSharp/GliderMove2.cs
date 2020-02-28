using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GliderMove2 : MonoBehaviour
{
    private WindZone _wind;

    private float _spd;             // 행글라이더 평균 속도가 35 ~ 60km 라고 함.
    private float _accel;           // 가속도는 -35 ~ 60 사이
    private float _maxSpd;          // 0 ~ 120 사이
    private float _maxAccel;        // 행글라이더 최고 속도가 급강하 할때 120km 까지 나온다고 함.
    private Rigidbody _rigid;

    Vector3 windVec;
    Vector3 spdVec;
    Vector3 rigidVec;

    TerrainCollision _terrColl;
    RisingWind _rising;
    InputSignal _inputSignal;

    //
    // James-20.0114 added to use Zyrosensor value.
    //
    private bool m_bUseZyroSensor = true;  // true(zyro sensor 사용), false(sensor 외 사용: 예: 조이스틱)
    private Communicator clsCommu;

    bool _isStanding;

    void Awake()
    {
        _isStanding = false;
        _spd = 35f;    
        _accel = 0f;
        _maxAccel = 60f;
        _maxSpd = 120f;
        
        // James-20.0114 added.
        clsCommu = GameObject.FindGameObjectWithTag("Communicator").GetComponent<Communicator>();

        _wind = GameObject.FindGameObjectWithTag("WindZone").GetComponent<WindZone>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _terrColl = this.GetComponent<TerrainCollision>();
        _terrColl.Init();
        _rigid = this.GetComponent<Rigidbody>();
        _rising = this.GetComponent<RisingWind>();
        _inputSignal = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InputSignal>();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            _rigid.useGravity = true;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            _rigid.useGravity = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (!_isStanding)
        {
            if (m_bUseZyroSensor == true)
            {
                windVec = _wind.transform.forward * (_wind.windMain * 0.5f);

                if (this.transform.localEulerAngles.x < 90f && this.transform.localEulerAngles.x >= 0f)
                {
                    _spd = 35f + (this.transform.localEulerAngles.x * 1.2f);
                }
                else if (this.transform.localEulerAngles.x < 360f && this.transform.localEulerAngles.x > 270f)
                {
                    _spd = 35f + ((this.transform.localEulerAngles.x - 360f) * 0.7f);
                    _spd = Mathf.Clamp(_spd, 0, 35f);
                }

                spdVec = this.transform.forward * _spd;
                rigidVec = (spdVec + windVec) * Time.deltaTime;
            }
            else // ########## Use the Joystick ##########
            {
                windVec = _wind.transform.forward * (_wind.windMain * 0.5f);

                if (this.transform.localEulerAngles.x < 90f && this.transform.localEulerAngles.x >= 0f)
                {
                    //Debug.Log("this.transform.localEulerAngles.x : " + this.transform.localEulerAngles.x);
                    _spd = 35f + (this.transform.localEulerAngles.x * 1.2f);
                    //Debug.Log("_spd : " + _spd);
                }
                else if (this.transform.localEulerAngles.x < 360f && this.transform.localEulerAngles.x > 270f)
                {
                    _spd = 35f + ((this.transform.localEulerAngles.x - 360f) * 0.7f);
                    _spd = Mathf.Clamp(_spd, 0, 35f);
                }

                spdVec = this.transform.forward * _spd;
                rigidVec = (spdVec + windVec) * Time.deltaTime;


                // James-20.0114 added.
                //System.String strMsg = "Roll2: " + this.transform.localEulerAngles.z + "Pitch2: " + this.transform.localEulerAngles.x + "Yaw2: " + this.transform.localEulerAngles.y + "\n";
                //Debug.Log(strMsg);
            }


        }

        _terrColl.TerrainDetection();

        if (_terrColl.GetIsStanding())
        {
            _isStanding = true;
            _rigid.useGravity = false;
        }

        if (!_isStanding)
        {            
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                if (_inputSignal.GetIsMoveStart())
                {
                    _rigid.MovePosition(this.transform.position + rigidVec);
                }
            }
            else
            {
                _rigid.MovePosition(this.transform.position + rigidVec);
            }
        }

        if (_rising.GetIsRising())
        {
            _rigid.AddForce(Vector3.up * 2, ForceMode.Impulse);
            _rising.SetIsRising(false);
        }
    }
}
