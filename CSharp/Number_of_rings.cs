using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Number_of_rings : MonoBehaviour
{
    public Text _numberOfRings;
    private RingCollisionManager _ringManager;

    // Start is called before the first frame update
    void Start()
    {
        _ringManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<RingCollisionManager>();
    }

    // Update is called once per frame
    void Update()
    {
        _numberOfRings.text = "통과한 링의 개수" + _ringManager.GetColliCount().ToString();
    }
}
