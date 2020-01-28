using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public struct struc_wind
{
    public double windAlt_out;
    public double rho;
    public double wind_azimuth_angle;
    public double wind_spd;
}

public struct ExternalOutputs_init_cond
{
  public double tt_0;                         /* '<Root>/tt_0' */
  public double x_cg0;                        /* '<Root>/x_cg0' */
  public double y_cg0;                        /* '<Root>/y_cg0' */
  public double z_cg0;                        /* '<Root>/z_cg0' */
  public double u_cg0;                        /* '<Root>/u_cg0' */
  public double v_cg0;                        /* '<Root>/v_cg0' */
  public double w_cg0;                        /* '<Root>/w_cg0' */
  public double phi_cg0;                      /* '<Root>/phi_cg0' */
  public double theta_cg0;                    /* '<Root>/theta_cg0' */
  public double psi_cg0;                      /* '<Root>/psi_cg0' */
  public double drop_velocity0;               /* '<Root>/drop_velocity0' */
  public double fleet_distance0;              /* '<Root>/fleet_distance0' */
}

public struct ExternalOutputs_wind_alt_0
{
  public double altitude_out;                 /* '<Root>/altitude_out' */
  public double rho;                          /* '<Root>/rho' */
  public double wind_out_Azimuth_angle;       /* '<Root>/wind_out_Azimuth_angle' */
  public double wind_out_speed;               /* '<Root>/wind_out_speed' */
}

public struct ExternalOutputs_high_a
{
    public double tt;                           /* '<Root>/tt' */
    public double x_cg;                         /* '<Root>/x_cg' */
    public double y_cg;                         /* '<Root>/y_cg' */
    public double z_cg;                         /* '<Root>/z_cg' */
    public double u_cg;                         /* '<Root>/u_cg' */
    public double v_cg;                         /* '<Root>/v_cg' */
    public double w_cg;                         /* '<Root>/w_cg' */
    public double phi_cg;                       /* '<Root>/phi_cg' */
    public double theta_cg;                     /* '<Root>/theta_cg' */
    public double psi_cg;                       /* '<Root>/psi_cg' */
    public double wind_out_speed;               /* '<Root>/wind_out_speed' */
    public double wind_out_direction;           /* '<Root>/wind_out_direction' */
    public double drop_velocity;                /* '<Root>/drop_velocity' */
    public double fleet_distance;               /* '<Root>/fleet_distance' */
    public double adv_velocity_h;               /* '<Root>/adv_velocity' */
    public double warning;                      /* '<Root>/warning' */
    public double Aux_Out1;                     /* '<Root>/Aux_Out1' */
    public double Aux_Out2;                     /* '<Root>/Aux_Out2' */
    public double Aux_Out3;                     /* '<Root>/Aux_Out3' */
}

public class ImportDllData : MonoBehaviour
{
    /* ===================== private Variable List ===================== */
    // unit
    private double _spd;
    private double _modelDirDeg;
    private double _ft;
    private double _windDir;
    private double _windSpd;

    public float _control_right;
    public float _control_left;

    private string _leftRightStr;
    private int _indexComma;

    // define struct
    private ExternalOutputs_init_cond init_result;
    private ExternalOutputs_wind_alt_0 wind_result;
    private ExternalOutputs_high_a high_a_result;

    // basic struct
    private Vector3 _modelPos;

    // GetComponent
    private Communicator _commu;
    private TerrainCollision _terrColl;
    private LeftRightProgressBar _leftRightProgressBar = null;
    private InputSignal _inputSignal = null;

    /* ===================== public Variable List ===================== */


    /* ===================== DLL Function Definition ===================== */
    [DllImport("init_cond_win32")]
    public static extern void init_cond_set(double knotSpd, double dirDeg, double ft);

    [DllImport("init_cond_win32")]
    public static extern ExternalOutputs_init_cond init_cond_get();

    [DllImport("wind_alt_0_wind32")]
    public static extern void wind_alt_0_initialize(bool firstTime, double altFt, double alt41Ft, double windDir41, double windKnot);

    [DllImport("wind_alt_0_wind32")]
    public static extern void wind_alt_0_step();

    [DllImport("wind_alt_0_wind32")]
    public static extern ExternalOutputs_wind_alt_0 wind_alt_get();

    [DllImport("high_a_win32")]
    public static extern void high_a_initialize_new(byte firstTime, double modelX, double modelY, double modelZ /*Ft*/, double spdX, double spdY, double spdZ, double rollDeg, double pitchDeg, double yawDeg);

    [DllImport("high_a_win32")]
    public static extern void high_a_step_new(int tid, double planeSpd_knot, double alt, double oldAlt, double density, double windSpd_knot, double windDir_deg, double windSpd_knot_old, double wind_Dir_deg_old, double maxLineControlMM_right /*= 800*/, double maxLineControlMM_left /*= 800*/, double control_percentage_right, double control_percentage_left, int drop_cr /*= 1*/, int malfunction /*= 1*/, int aux1 /*= 0*/);

    [DllImport("high_a_win32")]
    public static extern ExternalOutputs_high_a get_high_a();



    void Awake()
    {
        _spd = 35;
        _modelDirDeg = (double)this.transform.eulerAngles.y;
        _ft = (double)this.transform.position.y;
        _windDir = 0;
        _windSpd = 5;
        _indexComma = -1;

        _control_left = 0f;
        _control_right = 0f;

        _modelPos = this.gameObject.transform.position;
        //_commu = this.gameObject.GetComponent<Communicator>();
        if (SceneManager.GetActiveScene().name == "Tutorial_Scene_00")
        {
            _inputSignal = GameObject.Find("Main Camera").GetComponent<InputSignal>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("ImportDllData.cs - Start() - Start");
        //_windDir = this.gameObject.GetComponent<Communicator>().GetwindPacket_degree();

        // ####################### GetComponent ####################### Start

        //_commu = this.GetComponent<Communicator>();
        //_commu = FindObjectOfType<Communicator>();
        _commu = GameObject.FindGameObjectWithTag("Communicator").GetComponent<Communicator>();
        //Debug.Log("################### ImportDllData.GetComponent<Communicator>() ");
        _terrColl = gameObject.GetComponent<TerrainCollision>();
        _terrColl.Init();

        // ####################### GetComponent ####################### End

        _windDir = _commu.Get_WindPacket_degree();  // 받아오는 값 : 도
        _windSpd = _commu.Get_WindPacket_spd();     // 받아오는 값 : m/s

        //Debug.Log("Wind Spd : " + _windSpd);
        //Debug.Log("Wind Dir : " + _windDir);

        // 바람 0 ~ 2 수치를 몇 m/s 로 변환할 것인지 결정!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // 패러 글라이더 이착륙시 가장 좋은 바람의 속도는 4~6 m/s 라고 함.
        // 2 (중풍)을 4~6 m/s 로 생각하고 있음.
        // 1 순풍 2.8, 2 중풍 : 5.5, 3 강풍 : 7
        if (_windSpd == 0) _windSpd = 2.8;
        else if (_windSpd == 1) _windSpd = 5.5;
        else if (_windSpd == 2) _windSpd = 7;


        /* init dll */

        if (_modelDirDeg >= 180f)
        {
            _modelDirDeg -= 180f;
        }
        else
        {
            _modelDirDeg += 180f;
        }
                                                // 0을 넣으면 180도가 나오고 있음.
        init_cond_set(_spd, _modelDirDeg, _ft); // <- 현재 이 함수에 방위 값 넣어주면 180도 회전된 값이 나오고 있음. 180도 회전해서 넣어주면 원래 몸 방향으로 감
        init_result = init_cond_get();

        //Debug.Log("_modelDirDeg : " + _modelDirDeg);
        //Debug.Log("phi_cg0 : " + init_result.phi_cg0);
        //Debug.Log("theta_cg0 : " + init_result.theta_cg0);
        //Debug.Log("psi_cg0 : " + init_result.psi_cg0);


        /* wind dll */
        wind_alt_0_initialize(true, 100, 10000, _windDir, _windSpd * 1.94384); // m/s -> knot 으로 변환
        wind_alt_0_step();
        wind_result = wind_alt_get();

        //Debug.Log("altitude_out : " + wind_result.altitude_out);
        //Debug.Log("rho : " + wind_result.rho);
        //Debug.Log("wind_out_Azimuth_angle : " + wind_result.wind_out_Azimuth_angle);
        //Debug.Log("wind_out_speed : " + wind_result.wind_out_speed);

        high_a_initialize_new(1, _modelPos.x, _modelPos.y, _modelPos.z, init_result.u_cg0, init_result.v_cg0, init_result.w_cg0,
            init_result.phi_cg0, init_result.theta_cg0, init_result.psi_cg0);
        
        //Debug.Log("ImportDllData.cs - Start() - End");

        if (SceneManager.GetActiveScene().name == "Tutorial_Scene_00")
        {
            _leftRightProgressBar = GameObject.Find("Canvas").GetComponent<LeftRightProgressBar>();
        }
    }

    void Update()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _terrColl.TerrainDetection();
        if (_terrColl.GetIsStanding())
        {

            return;
        }
        else
        {
            #region percentage control line code 1 -- joystick ver
            //if ((_control_right = Input.GetAxis("Roll")) > 0) {
            //    _control_right *= 100;
            //    _control_left = 0;
            //}
            //else {
            //    _control_left   = _control_right;
            //    _control_left *= 100;
            //    _control_right = 0;
            //}
            #endregion

            #region percentage control line code 2 -- joystick ver1
            //if (Mathf.Abs(Input.GetAxis("Roll")) > 0.00f ||
            //    Mathf.Abs(Input.GetAxis("Pitch")) > 0.00f)
            //{
            //    float roll = Input.GetAxis("Roll");
            //    float pitch = Input.GetAxis("Pitch");

            //    //Debug.Log("roll : " + roll);
            //    //Debug.Log("pitch : " + pitch); 

            //    if (Mathf.Abs(roll) > Mathf.Abs(pitch))
            //    {
            //        if (roll > 0.00f)
            //        {
            //            _control_right = roll * 100f;
            //            _control_left = 0f;
            //        }
            //        else
            //        {
            //            _control_left = Mathf.Abs(roll) * 100f;
            //            _control_right = 0f;
            //        }
            //    }
            //    else
            //    {
            //        _control_left = (50f * pitch) + 50;
            //        _control_right = (50f * pitch) + 50;
            //    }
            //}
            #endregion

            #region Control Line HardWare Ver
            Utils.stPacket_HW hw = _commu.GetHWsignal();

            if (hw.id == Utils.PacketID.SIMULATOR_HARDWARE)
            {
                float left = (float)hw.left / 20000f;
                float right = (float)hw.right / 20000f;

                if (SceneManager.GetActiveScene().name == "Tutorial_Scene_00")
                {
                    _leftRightProgressBar.LeftRightBar_Value(left, right);  // 당긴 줄만큼 튜토리얼에서 표시 되게
                }

                // 끝까지 당겨도 50%만 사용하는 것으로 하기 위함. 지금 모델은 군용이라 한쪽으로 당겼을 때 굉장히 빠름.
                // 김박사님께서는 이 모델은 군용이니 그냥 이런 모델이라고 쓰라고 하셨지만 너무 빠른 것 같아 내 본의로 30%만 사용하는 것으로 함. 
                _control_left = left * 30f;
                _control_right = right * 30f;
            }
            #endregion
            if (_commu.GetSceneStart())
            {
                if (SceneManager.GetActiveScene().name == "Tutorial_Scene_00")
                {
                    if (_inputSignal.GetInputSignal())
                    {
                        high_a_step_new(1, _spd * 1.94384, this.transform.position.y, _modelPos.y, wind_result.rho, wind_result.wind_out_speed, wind_result.wind_out_Azimuth_angle, wind_result.wind_out_speed, wind_result.wind_out_Azimuth_angle, 800, 800, _control_right, _control_left, 1, 0, 0);

                        high_a_result = get_high_a();
                        _spd = high_a_result.adv_velocity_h;

                        this.transform.position = new Vector3((float)(high_a_result.x_cg), (float)(high_a_result.z_cg * 0.3048), (float)(high_a_result.y_cg));
                        this.transform.eulerAngles = new Vector3((float)high_a_result.phi_cg, (float)high_a_result.psi_cg, (float)high_a_result.theta_cg);
                    }
                }
                else
                {
                    high_a_step_new(1, _spd * 1.94384, this.transform.position.y, _modelPos.y, wind_result.rho, wind_result.wind_out_speed, wind_result.wind_out_Azimuth_angle, wind_result.wind_out_speed, wind_result.wind_out_Azimuth_angle, 800, 800, _control_right, _control_left, 1, 0, 0);

                    high_a_result = get_high_a();
                    _spd = high_a_result.adv_velocity_h;

                    this.transform.position = new Vector3((float)(high_a_result.x_cg), (float)(high_a_result.z_cg * 0.3048), (float)(high_a_result.y_cg));
                    this.transform.eulerAngles = new Vector3((float)high_a_result.phi_cg, (float)high_a_result.psi_cg, (float)high_a_result.theta_cg);
                }
                _modelPos = this.transform.position;
            }
            
        }
    }

    public ExternalOutputs_high_a Get_high_a_result()
    {
        return high_a_result;
    }

    public float GetLeftLinePercentage()
    {
        return _control_left;
    }

    public float GetRightLinePercentage()
    {
        return _control_right;
    }
}
