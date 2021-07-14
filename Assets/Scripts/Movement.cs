using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField] private float speed = 1.5f;
    public Vector3 GetRigidBodyVelocity
    {
        get
        {
            return _rigidbody.velocity;
        }
    }
    private Vector3 previousPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        previousPosition = transform.position;
        //StartCoroutine(Turn90Degree());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 forward = transform.forward;
        forward.y = 0f;
        _rigidbody.MovePosition(transform.position + (forward * (speed / 50)));
    }

    private IEnumerator Turn90Degree()
    {
        yield return new WaitForSeconds(5f);
        transform.Rotate(transform.up, 90);
        StartCoroutine(Turn90Degree());
    }
}
