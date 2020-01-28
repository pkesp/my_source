using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GliderMove2 : MonoBehaviour
{
    public WindZone _wind;

    private float _spd;             // 행글라이더 평균 속도가 35 ~ 60km 라고 함.
    private float _accel;           // 가속도는 -35 ~ 60 사이
    private float _maxSpd;          // 0 ~ 120 사이
    private float _maxAccel;        // 행글라이더 최고 속도가 급강하 할때 120km 까지 나온다고 함.
    private Rigidbody _rigid;

    Vector3 _pos;

    Vector3 windVec;
    Vector3 spdVec;
    Vector3 rigidVec;

    TerrainCollision _terrColl;

    bool _isStanding;

    void Awake()
    {
        _isStanding = false;
        _spd = 35f;         
        _accel = 0f;
        _maxAccel = 60f;    
        _maxSpd = 120f;     

        _pos = this.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        _terrColl = this.GetComponent<TerrainCollision>();
        _terrColl.Init();
        _rigid = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isStanding)
        {
            windVec = _wind.transform.forward * (_wind.windMain / 3f);

            if (this.transform.localEulerAngles.x > 42f) _spd = 90f * 0.277778f;
            else if (this.transform.localEulerAngles.x > 38f) _spd = 85f * 0.277778f;
            else if (this.transform.localEulerAngles.x > 34f) _spd = 80f * 0.277778f;
            else if (this.transform.localEulerAngles.x > 30f) _spd = 75f * 0.277778f;
            else if (this.transform.localEulerAngles.x > 26f) _spd = 70f * 0.277778f;
            else if (this.transform.localEulerAngles.x > 22f) _spd = 65f * 0.277778f;
            else if (this.transform.localEulerAngles.x > 18f) _spd = 60f * 0.277778f;
            else if (this.transform.localEulerAngles.x > 14f) _spd = 55f * 0.277778f;
            else if (this.transform.localEulerAngles.x > 10f) _spd = 50f * 0.277778f;
            else if (this.transform.localEulerAngles.x > 6f) _spd = 45f * 0.277778f;
            else if (this.transform.localEulerAngles.x > 2f) _spd = 40f * 0.277778f;
            else if (this.transform.localEulerAngles.x > -2f) _spd = 35f * 0.277778f;
            else if (this.transform.localEulerAngles.x > -6f) _spd = 30f * 0.277778f;
            else if (this.transform.localEulerAngles.x > -10f) _spd = 20f * 0.277778f;
            else if (this.transform.localEulerAngles.x > -14f) _spd = 10f * 0.277778f;
            else _spd = 5f * 0.277778f;

            spdVec = this.transform.forward * _spd;
            //this.transform.position = this.transform.position + spdVec;
            rigidVec = (spdVec + windVec); //* Time.deltaTime;
            //Debug.Log(rigidVec);
        }
        
        _terrColl.TerrainDetection();

        if (_terrColl.GetIsStanding())
        {
            _isStanding = true;
            _rigid.useGravity = false;
        }

        if (!_isStanding)
        {
            _rigid.AddForce(rigidVec, ForceMode.Acceleration);
            //_rigid.velocity = rigidVec;
        }
    }

    void FixedUpdate()
    {
        
    }
}
