using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    private int stationID;
    [SerializeField]
    private GameObject employee;
    private bool powerOn = true;

    private void Update()
    {
        if(!powerOn && employee.GetComponent<Worker>().GetIsWorking())
        {
            powerOn = true;
            GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("EnergySaving/deskOn");
        }
    }

    public bool IsEnergyWasted()
    {
        if (!employee.GetComponent<Worker>().GetIsWorking() && powerOn)
        {
            return true;
        }
        
        return false;
    }

    public void TurnOffStation()
    {
        if (IsEnergyWasted())
        {
            powerOn = false;
            GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("EnergySaving/deskOff");
        }
    }

    public void StartShift()
    {
        employee.GetComponent<Worker>().GenerateShift();
    }

    public GameObject GetEmployee()
    {
        return employee;
    }
}
