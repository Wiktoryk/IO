using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Office : MonoBehaviour
{
    float energyWasted;
    Station[] stations;
    float timeLeft;

    void CalculateWaste()
    {
        for(int i = 0; i < stations.Length; i++)
        {
            if (stations[i].IsEnergyWasted())
            {
                energyWasted += 0.01f;
            }
        }
    }

    private void Start()
    {
        stations = FindObjectsOfType<Station>();
        energyWasted = 0;
        timeLeft = 60;
        for (int i = 0;i < stations.Length;i++)
        {
            stations[i].StartShift();
        }
    }

    void Update()
    {
        CalculateWaste();

        if (timeLeft <= 0.0f)
        {
            EndGame();
        }

        timeLeft -= Time.deltaTime;
        GetComponent<EnergySavingDisplay>().UpdateDisplay((int)(1000.0f - energyWasted), timeLeft);
    }

    public void EndGame()
    {
        for (int i = 0; i < stations.Length; i++)
        {
            stations[i].GetEmployee().GetComponent<Worker>().SetIsRunning(false);
        }
    }
}
