using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wynik : MonoBehaviour
{
    [SerializeField]
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        int wynikGry = PlayerPrefs.GetInt("WynikGry", 0);
        text.text = "Tw�j wynik to: "+ wynikGry.ToString();
    }

}
