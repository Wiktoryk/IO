using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    [SerializeField]
    private int id = 0;
    [SerializeField]
    private string color = "black";
    private bool wait = true;
    public Passenger()
    {
    }
    public string getColor()
    {
        return color; ;
    }
    public int getId()
    {
        return id;
    }
    public void changeWait()
    {
        wait = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

    }
}
