using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private float _speed = 3f;
    private Rigidbody2D _righbody;
    public Batteries bat;
    private void Awake()
    {
        _righbody = GetComponent<Rigidbody2D>();

    }
    private void OnMove(InputValue value)
    {
        if (_righbody != null)
        {
            _righbody.velocity = value.Get<Vector2>() * _speed;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_righbody != null)
        {
            _righbody.freezeRotation = true;
        }
        if (other.gameObject.CompareTag("Battery"))
        {
            Destroy(other.gameObject);
            bat.batteryCount++;
        }
    }
}
