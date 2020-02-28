using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingCollisionManager : MonoBehaviour
{
    CollisionSphere[] _arrSphere = null;
    int _arraySize;
    int _colliCount;
    float _distance;
    int _index;

    Transform _currentTrans;

    // Start is called before the first frame update
    void Start()
    {
        _arrSphere = GameObject.Find("Gate").GetComponentsInChildren<CollisionSphere>();
        _arraySize = _arrSphere.Length;
        _colliCount = 0;
        _distance = Mathf.Infinity;
        _index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // 거리 계산하면서 계속 가까운 녀석을 타겟으로
        // 0번 부터 순서대로 높은 곳에서 낮은 곳으로 간다고 생각함.
        for (int i = _index; i < _arraySize; i++)
        {
            if (_arrSphere[i].GetCheck()) continue;
            if (this.gameObject.transform.position.y < (_arrSphere[i].GetTransform().position.y - 5f)) continue;

            float dist = Vector3.SqrMagnitude(_arrSphere[i].GetTransform().position - this.transform.position);
            if (_distance > dist)
            {
                _index = i;
                _distance = dist;
            }
        }

        _currentTrans = _arrSphere[_index].GetTransform();
        _distance = Mathf.Infinity;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 여기 들어오면 타겟을 바꾸고
        if (other.tag == "Gate")
        {
            other.transform.GetComponent<CollisionSphere>().SetCheck(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 충돌 벗어나면 통과 했다고 카운트 해주고
        if (other.tag == "Gate")
        {
            _colliCount++;
        }
    }

    public int GetColliCount()
    {
        return _colliCount;
    }

    public int GetRingNumber()
    {
        return _arraySize;
    }

    public Transform GetCurrentTarget()
    {
        return _currentTrans;
    }

    public int GetMaxRingCount()
    {
        return _arraySize;
    }
}
