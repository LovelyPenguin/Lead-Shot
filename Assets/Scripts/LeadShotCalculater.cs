using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeadShotCalculater : MonoBehaviour
{
    [SerializeField] private Rigidbody targetRigidbody;
    [SerializeField] private Rigidbody _rigidbody;

    [Header("Target Debug")]
    [SerializeField] private float targetSpeed;
    [SerializeField] private Vector3 targetVelocity;
    [SerializeField] private float distance;

    [Header("Projectile Setting")]
    [SerializeField] private GameObject projectileObject;
    [SerializeField] private float projectileSpeed;

    [Header("UI Setting")]
    [SerializeField] private Transform crossHair;
    [SerializeField] private Text debugText;

    [Header("Debug")]
    public float InterceptTimeValue;
    public float determinentValue;

    private float maxInterceptTime = 0f;
    private float t;
    private float t1;
    private float t2;
    private float a;
    private float b;
    private float c;

    private void Awake()
    {
        targetRigidbody = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        distance = Vector3.Distance(transform.position, targetRigidbody.position);
        targetSpeed = targetRigidbody.velocity.magnitude;
        targetVelocity = targetRigidbody.velocity;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject bullet = Instantiate(projectileObject, transform.position, transform.rotation);
            bullet.GetComponent<Bullet>().Fire(projectileSpeed, InterceptTimeValue + 1f);
        }

        maxInterceptTime = InterceptTimeValue > maxInterceptTime ? InterceptTimeValue : maxInterceptTime;
        
        crossHair.position = Camera.main.WorldToScreenPoint(Intercept(transform.position, _rigidbody.velocity, projectileSpeed, targetRigidbody.position, targetRigidbody.velocity));
        //_rigidbody.MovePosition(transform.position + Vector3.forward * 1);
        debugText.text = 
            $"Intercept Time : {InterceptTimeValue}\n" +
            $"Determinent Value : {determinentValue}\n" +
            $"Determinent Sqrt Value : {Mathf.Sqrt(determinentValue)}\n" +
            $"t : {t}\n" +
            $"t1 : {t1}\n" +
            $"t2 : {t2}\n" +
            $"a : {a}\n" +
            $"b : {b}\n" +
            $"c : {c}\n";
    }

    private void FixedUpdate()
    {
        transform.LookAt(Intercept(transform.position, _rigidbody.velocity, projectileSpeed, targetRigidbody.position, targetRigidbody.velocity), transform.up);
    }

    public Vector3 Intercept(
        Vector3 shooterPosition,
        Vector3 shooterVelocity,
        float shotSpeed,
        Vector3 targetPosition,
        Vector3 targetVelocity)
    {
        Vector3 targetRelativePosition = targetPosition - shooterPosition;
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;

        t = InterceptTime(shotSpeed, targetRelativePosition, targetVelocity);
        InterceptTimeValue = t;
        return targetPosition + t * targetRelativeVelocity;
    }

    public float InterceptTime(
        float shotSpeed,
        Vector3 targetRelativePosition,
        Vector3 targetRelativeVelocity)
    {
        // Magnitude의 연산은 무겁기 때문에 그냥 sqrMagnitude로 처리한 것으로 보임
        float velocitySqrt = targetRelativeVelocity.sqrMagnitude;
        
        // Projectile의 속도가 0에 수렴하면 도착 시간은 0
        if (velocitySqrt < 0.001f)
        {
            return 0f;
        }

        a = velocitySqrt - shotSpeed * shotSpeed;

        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude / (2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition));
            return Mathf.Max(t, 0f);
        }

        b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        c = targetRelativePosition.sqrMagnitude;

        float determinent = b * b - 4f * a * c;

        determinentValue = determinent;

        if (determinent > 0f)
        {
            t1 = (-b + Mathf.Sqrt(determinent)) / (2f * a);
            t2 = (-b - Mathf.Sqrt(determinent)) / (2f * a);

            if (t1 > 0f)
            {
                if (t2 > 0f)
                {
                    return Mathf.Min(t1, t2);
                }
                else
                {
                    return t1;
                }
            }
            else
            {
                return Mathf.Max(t2, 0f);
            }
        }
        else if (determinent < 0f)
        {
            return 0f;
        }
        else
        {
            return Mathf.Max(-b / (2f * a), 0f);
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 intercept = Intercept(transform.position, _rigidbody.velocity, projectileSpeed, targetRigidbody.position, targetRigidbody.velocity);
        Gizmos.DrawSphere(intercept, 1.3f);
    }
}
