using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSphere : MonoBehaviour
{
    private static int s_createNum = 0;
    private bool _check;
    private int _createNum;

    public CollisionSphere()
    {
        _check = false;
        _createNum = s_createNum++;
    }

    public bool GetCheck()
    {
        return _check;
    }

    public void SetCheck(bool bol)
    {
        _check = bol;
    }

    public Transform GetTransform()
    {
        return this.gameObject.transform;
    }
}
