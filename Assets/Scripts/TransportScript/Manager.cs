using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private float posX = -9f;
    private float posY = -5f;
    public void add()
    {
        posX += 1.5f;
    }
    public void removed()
    {
        posX -= 1.5f;
    }
    public float getx()
    {
        return posX;
    }
    public float gety()
    {
        return posY;
    }

}
