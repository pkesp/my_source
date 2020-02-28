using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingWind : MonoBehaviour
{
    bool _isRising;
    float _deltaTime;
    int _rndNum;
    int _additionNum;

    // Start is called before the first frame update
    void Start()
    {
        _isRising = false;
        _deltaTime = 0f;
        _rndNum = 0;
        _additionNum = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void TwoSecond()
    {
        _isRising = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RisingWindZone")
        {
            _isRising = true;
        }
    }

    //private void OnTriggerStay(Collider other)
    //{        
    //    if (other.tag == "RisingWindZone")
    //    {
    //        //_deltaTime += Time.deltaTime;

    //        //if ((_rndNum - _additionNum) < 10 && !_isRising)
    //        //{
    //        //    _deltaTime = 0f;
    //        //    _isRising = true;
    //        //    _additionNum = 0;
    //        //    Invoke("TwoSecond", 2f);
    //        //}

    //        //if (_deltaTime > 0.5f )
    //        //{
    //        //    _rndNum = Random.Range(0, 100);
    //        //    _deltaTime = 0f;
    //        //    _additionNum += 1;
    //        //}

    //        //_isRising = true;
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "RisingWindZone")
        {
            _isRising = false;
        }
    }

    public bool GetIsRising()
    {
        return _isRising;
    }

    public void SetIsRising(bool b)
    {
        _isRising = b;
    }
}
