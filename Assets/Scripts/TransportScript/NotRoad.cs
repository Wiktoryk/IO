using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotRoad : MonoBehaviour
{
    private Rigidbody2D _righbody;
    private void Awake()
    {
        _righbody = GetComponent<Rigidbody2D>();

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_righbody != null)
        {
            _righbody.freezeRotation = true;
        }
    }
}
