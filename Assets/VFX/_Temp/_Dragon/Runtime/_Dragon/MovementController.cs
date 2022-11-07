using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] Animator _animator;

    [SerializeField] Transform _target;

    [SerializeField] float _maxForce = .1f;

    [SerializeField] float _maxSpeed = 4f;

    [Header("Distance Control")]
    [SerializeField] float _avoidDistance = 2f;
    [SerializeField] float _attractDistance = 5f;

    Vector3 _velocity = new();
    float _angluarVelocity = new(); // only in y axis cause it's a flying vreature
    Vector3 _steer = new();
    Vector3 _acceleration = new();
    Vector3 nextAngle = new();

    bool _isFollow = false;

    // debug
    Vector3 realDesired;

    void Start()
    {
    }

    private void FixedUpdate()
    {
        Arrive(_target.position);
        VelocityUpdate();
        RotationUpdate();
        AnimationUpdate();
    }

    void VelocityUpdate()
    {
        _velocity += _acceleration;

        _velocity = Limit(_velocity, _maxSpeed);

        //Debug.DrawRay(transform.position, _velocity / Time.deltaTime * 10f, Color.green);

        transform.position += _velocity * Time.deltaTime;

        _acceleration = Vector3.zero;
    }

    void applyForce(Vector3 force)
    {
        // We could add mass here if we want A = F / M
        _acceleration += force;

        //Debug.DrawRay(transform.position, _acceleration / Time.deltaTime * 10f, Color.blue);

    }

    void Arrive(Vector3 targetPos)
    {
        realDesired = new Vector3(targetPos.x, 0, targetPos.z) - new Vector3(transform.position.x, 0, transform.position.z);

        float realDist = realDesired.magnitude;

        if (realDist < _avoidDistance + 1f && realDist > _avoidDistance - 1f)
        {
            realDesired = Vector3.zero;
        }
        if (realDist < _avoidDistance)
        {
            float m = (1f - (realDist / _avoidDistance)) * _maxSpeed;

            //Debug.Log("avoid");
            //        Debug.Log($"avoid with a d: {realDist}; " +
            //$"with a m: {1 - (realDist / _avoidDistance)}; " +
            //$"with a force: {realDesired}");

            realDesired = SetMag(-realDesired, m);
        }
        else if (realDist < _attractDistance)
        {
            //Debug.Log("attract");

            float m = ((realDist - _avoidDistance) / (_attractDistance - _avoidDistance)) * _maxSpeed;

            realDesired = SetMag(realDesired, m);
        }
        else
        {
            realDesired = SetMag(realDesired, _maxSpeed);
        }

        var dH = transform.position.y - (targetPos.y + 0.5f);

        var heightDesired = dH > 0 ? -1f : 1f;

        float mHeight = Mathf.Clamp01(Mathf.Abs(dH));

        if (_isFollow)
        {
            realDesired += Vector3.up * heightDesired * mHeight;
        }
        else
        {
            realDesired = Vector3.up * heightDesired * mHeight; ;
        }
        

        _steer = realDesired - _velocity;

        _steer = Limit(_steer, _maxForce);

        applyForce(_steer);
    }

    void RotationUpdate()
    {
        var realDirection = (new Vector3(_target.position.x, 0, _target.position.z)
            - new Vector3(transform.position.x, 0, transform.position.z)).normalized;

        var angle = Vector3.SignedAngle(realDirection, transform.forward, Vector3.up);

        var absAngle = Mathf.Abs(angle);
        var m = absAngle / 180;
        m = Mathf.Clamp01(m * 8);
        var steer = m * 10f;

        if (absAngle < 1f)
        {
            // avoid shaking
            if(_angluarVelocity > 0)
            {
                _angluarVelocity -= 10f;
                if (_angluarVelocity < 0) _angluarVelocity = 0;
            }
            else
            {
                _angluarVelocity += 10f;
                if (_angluarVelocity > 0) _angluarVelocity = 0;
            }
        }
        else if (angle > 0)
        {
            //Debug.Log($">0 with: {angle}");

            _angluarVelocity += steer;
            if (_angluarVelocity > 90) _angluarVelocity = 90;
            nextAngle -= Vector3.up * _angluarVelocity * Time.deltaTime;
        }
        else
        {
            //Debug.Log($"<0 with: {angle}");

            _angluarVelocity -= steer;
            if (_angluarVelocity < -90) _angluarVelocity = -90;
            nextAngle -= Vector3.up * _angluarVelocity * Time.deltaTime;
        }

        transform.rotation = Quaternion.Euler(nextAngle + new Vector3(0,180,0));
    }

    void AnimationUpdate()
    {
        var velocity = transform.InverseTransformVector(_velocity);

        //Debug.DrawRay(transform.position, velocity.normalized + velocity / Time.deltaTime, Color.blue);

        _animator.SetFloat("Velocity X", _angluarVelocity / 90);

        _animator.SetFloat("Velocity Z", velocity.z / _maxSpeed);
    }

    Vector3 Limit(Vector3 velocity, float length)
    {
        if (velocity.magnitude > length)
        {
            //Debug.Log($"reach max with a value: {velocity.magnitude} higher than: {length}");
            return velocity.normalized * length;
        }
        else
        {
            return velocity;
        }
    }

    Vector3 SetMag(Vector3 velocity, float length)
    {
        return velocity.normalized * length;
    }

    public void OnBeginFollow()
    {
        _isFollow = true;
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(fixedTargetPosition, 2);
    //}
}
