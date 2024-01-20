using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private float posX = -11f;
    private float posY = 2.2f;
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
