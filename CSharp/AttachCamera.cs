using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AttachCamera : MonoBehaviour
{

    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        this.transform.SetParent(GameObject.Find("ParaGlider2").transform.Find("CamPos"));
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.Euler(Vector3.zero);
        InputTracking.disablePositionalTracking = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //this.transform.SetParent(GameObject.Find("ParaGlider2").transform.Find("CamPos"));
        //this.transform.localPosition = Vector3.zero;
        //this.transform.localRotation = Quaternion.Euler(Vector3.zero);
        //InputTracking.disablePositionalTracking = true;
        //this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y - 1, this.transform.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
