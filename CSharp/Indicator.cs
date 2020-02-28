using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    private double _airSpd;
    private double _windSpd;
    private double _ADescentRate;
    private double _altitude;
    private double _windAzimuth;
    private double _selfAzimuth;

    private ImportDllData _dll;
    private Communicator _commu;

    private Quaternion _quat;

    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        _dll = this.GetComponent<ImportDllData>();
        //_commu = this.GetComponent<Communicator>();
        //_commu = FindObjectOfType<Communicator>();
        _commu = GameObject.FindGameObjectWithTag("Communicator").GetComponent<Communicator>();

        _windSpd = _commu.Get_WindPacket_spd();
        _windAzimuth = _commu.Get_WindPacket_degree();
    }

    // Update is called once per frame
    void Update()
    {
        //_airSpd = _dll.Get_high_p_result().adv_velocity;
        //_windSpd = _dll.Get_high_p_result().wind_out_speed;
        //_windAzimuth = _dll.Get_high_p_result().wind_out_direction;
        //_altitude = this.gameObject.transform.position.y * 3.28084;
        //_ADescentRate = (_dll.Get_high_p_result().drop_velocity * 196.85) / 1000;
        //_selfAzimuth = this.transform.eulerAngles.y;

        //Debug.Log("altitude : " + _altitude);
        //Debug.Log("airSpd : " + _airSpd);
        //Debug.Log("windSpd : " + _windSpd);
        //Debug.Log("windAzimuth : " + _windAzimuth);
        //Debug.Log("ADescentRate : " + _ADescentRate);
        //Debug.Log("_selfAzimuth : " + _selfAzimuth);
    }
}
