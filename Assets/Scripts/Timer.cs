using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField]
    public Text text;
    private float sekunda = 0.0f;
    public float iloscsekund = 8.0f;
    public Statistics statistics;
    public bool timerOn = true;
    private void Awake()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerOn)
        {

            sekunda += Time.deltaTime;
            if (sekunda >= 1)
            {
                iloscsekund -= 1;
                sekunda = 0;
            }
            text.text = iloscsekund.ToString();
            if (iloscsekund <= 0)
            {
                Debug.Log(statistics.getScore());

                //MiniGameStatus.Instance.SetStatus("Transport", statistics.getScore(), true);
                SceneManager.LoadScene("LobbyScene");
            }
        }
    }
}
