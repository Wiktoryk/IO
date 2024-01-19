using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
public float posX = 0.7f;

    public void UpdatePosition()
    {
        posX += 1.2f;
    }
    private float GetPosition()
    {
        return posX;
    }
}
