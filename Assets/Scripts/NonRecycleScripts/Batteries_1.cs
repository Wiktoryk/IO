using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Batteries : MonoBehaviour
{
    public int batteryCount = 0;
    public Text text;
    public Statistics statistics;
    public int valueBattery = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = batteryCount.ToString();
        statistics.UpdateScore(batteryCount * valueBattery);
    }
}
