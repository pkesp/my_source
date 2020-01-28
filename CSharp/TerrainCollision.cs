using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCollision : MonoBehaviour
{
    /* Collision variable */
    private RaycastHit _hit;
    private Ray _ray;


    private bool _isStandup;
    private float _standingSpd;
    private float _maxDistance;

    private Vector3 _hitPos;
    private Vector3 _saveRot;


    public void Init()
    {
        _standingSpd = 0f;
        _hitPos = Vector3.zero;
        _isStandup = false;
        _hit = new RaycastHit();
        _ray = new Ray();
        _maxDistance = 10f;
    }

    public void TerrainDetection()
    {
        if (!_isStandup)
        {
            _ray.origin = this.transform.position;
            _ray.direction = -(Vector3.up);
            //Debug.Log("광선 쏜다");
            if (Physics.Raycast(_ray, out _hit, _maxDistance))
            {
                //if (_hit.transform.tag.Equals("Cube"))
                //{
                //    Debug.Log("Cube 찾았다");
                //}
                if (_hit.distance < 2f)
                {
                    Debug.Log("지면 찾았다");
                    _isStandup = true;
                    _hitPos = _hit.point;
                    _hitPos.y += 4;
                    _saveRot = this.transform.rotation.eulerAngles;
                }
                else
                {
                    _isStandup = false;
                }
            }
            else
            {
                return;
            }
        }
       

        if (_isStandup)
        {
            _standingSpd += Time.deltaTime * 0.5f;
            this.transform.position = Vector3.Lerp(this.transform.position, _hitPos, _standingSpd);
            if (_saveRot.x <= 90f && _saveRot.z <= 90f)
            {
                this.transform.rotation = Quaternion.Euler(Vector3.Slerp(_saveRot, new Vector3(0, _saveRot.y, 0), _standingSpd));
            }
            else if (_saveRot.x <= 90f && _saveRot.z > 90f)
            {
                this.transform.rotation = Quaternion.Euler(Vector3.Slerp(_saveRot, new Vector3(0, _saveRot.y, 360f), _standingSpd));
            }
            else if (_saveRot.x > 90f && _saveRot.z <= 90f)
            {
                this.transform.rotation = Quaternion.Euler(Vector3.Slerp(_saveRot, new Vector3(360f, _saveRot.y, 0f), _standingSpd));
            }
            else if (_saveRot.x > 90f && _saveRot.z > 90f)
            {
                this.transform.rotation = Quaternion.Euler(Vector3.Slerp(_saveRot, new Vector3(360f, _saveRot.y, 360f), _standingSpd));
            }
            //if (this.transform.position.y == _hitPos.y)
            //{
            //    //_isStandup = false;
            //}
        }
    }

    public bool GetIsStanding()
    {
        return _isStandup;
    }

    public void SetIsStanding(bool value)
    {
        _isStandup = value;
    }
}
