using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPosChanger : MonoBehaviour
{
    public GameObject _camPos0;
    //public GameObject _camPos1;
    //public GameObject _camPos2;
    //public GameObject _camPos3;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            Camera.main.transform.SetParent(_camPos0.transform);
        }

        //if (Input.GetKey(KeyCode.Alpha2))
        //{
        //    Camera.main.transform.SetParent(_camPos1.transform);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    Camera.main.transform.SetParent(_camPos2.transform);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    Camera.main.transform.SetParent(_camPos3.transform);
        //}

    }
}