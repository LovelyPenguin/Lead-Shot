using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Fire(float speed, float destroyTimer)
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        _rigidbody.AddForce(forward * speed, ForceMode.VelocityChange);
        StartCoroutine(TimerDestroyer(destroyTimer));
    }

    private void OnDestroy()
    {
        _rigidbody.velocity = Vector3.zero;
    }

    IEnumerator TimerDestroyer(float destroyTime)
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}
