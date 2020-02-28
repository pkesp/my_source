using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPos_Look_Rot : MonoBehaviour
{
    private Transform _targetTransform;
    private RingCollisionManager _ringManager;

    private float rotAngle;
    private Vector3 pos;
    //private Vector3 dir;
    private float rotSpd;
    private float dist_forward;
    private float dist_up;

    // Start is called before the first frame update
    void Start()
    {
        rotSpd = 200f;
        dist_forward = 10f;
        dist_up = 2f;

        _ringManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<RingCollisionManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // ##### ui location set #####
        pos = Camera.main.transform.position + Camera.main.transform.forward * dist_forward + Camera.main.transform.up * dist_up;
        this.transform.position = pos;

        // #### ui direction set 2 ####
        this.transform.LookAt(_ringManager.GetCurrentTarget());

        // #### ui rotation set ####
        //rotAngle += rotSpd * Time.deltaTime;
        //this.transform.Rotate(Vector3.forward, rotAngle, Space.Self);
    }
}
