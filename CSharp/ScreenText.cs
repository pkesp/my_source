using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenText : MonoBehaviour
{
    public Text _txt;
    private float _sec;
    private int _min;

    private Communicator _commu;

    // Start is called before the first frame update
    void Start()
    {
        _min = 3;
        _sec = 0f;

        _commu = GameObject.FindGameObjectWithTag("Communicator").GetComponent<Communicator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_commu.GetSceneStart())
        {
            // play time count
            if (_min > -1)
            {
                if (_sec < 0)
                {
                    _min -= 1;
                    _sec = 60f;
                }

                _sec -= Time.deltaTime;

                if (_min > 0)
                {
                    _txt.text = _min + " 분" + (int)_sec + " 초";
                }
                else if (_min > -1)
                {
                    _txt.text = "    " + (int)_sec + " 초";
                }
            }
            else // destroy code
            {
                _txt.text = "체험을 종료합니다. ";
                StartCoroutine(_commu.MyLoadScene(0, 5f));
            }
        }
    }
}
