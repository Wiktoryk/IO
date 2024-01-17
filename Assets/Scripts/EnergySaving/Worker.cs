using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class Worker : MonoBehaviour
{
    private float workTime;
    private float breakTime;
    private bool isWorking;
    private bool isRunning = false;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isWorking && isRunning)
        {
            workTime -= Time.deltaTime;
        }
        else
        {
            breakTime -= Time.deltaTime;
        }
        
        if (isWorking && workTime < 0)
        {
            isWorking = false;
            GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 0);
        }
        else if (!isWorking && breakTime <= 0 && isRunning)
        {
            GenerateShift();
        }
    }

    public void GenerateShift()
    {
        isRunning = true;
        isWorking = true;
        GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 1);
        workTime = Random.Range(5.0f, 40.0f);
        breakTime = Random.Range(5.0f, 35.0f);
    }

    public bool GetIsWorking()
    {
        return isWorking;
    }

    public void SetIsRunning(bool value)
    {
        isRunning=value;
    }
}
