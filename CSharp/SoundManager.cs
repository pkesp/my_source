using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //private AudioSource _audio;
    private Transform _wind;
    private Transform _modelTransform;

    private float _maxDistance;
    private float _minDistance;
    private Vector3 _windDirection;

    // Start is called before the first frame update
    void Start()
    {
        //_audio = this.gameObject.GetComponent<AudioSource>();
        _minDistance = 5f;
        _maxDistance = 20f;

        _wind = GameObject.FindGameObjectWithTag("WindZone").GetComponent<Transform>();
        _modelTransform = GameObject.FindGameObjectWithTag("Glider").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 windForward = _wind.transform.forward;
        Vector3 modelForward = _modelTransform.forward;

        //windForward.Normalize();
        //modelForward.Normalize();

        Vector3 sumDirection = modelForward + windForward;
        _windDirection = modelForward - sumDirection;
        _windDirection *= _minDistance;
        this.transform.position = _modelTransform.position + _windDirection;
    }
}
