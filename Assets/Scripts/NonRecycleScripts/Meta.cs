using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Meta : MonoBehaviour
{
    public Statistics statistics;
    public  GameObject destroyObject;
    public GameObject destroyObject2;
    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(destroyObject);
        Destroy(destroyObject2);
        //MiniGameStatus.Instance.SetStatus("Nie nadajace sie do recyklingu", statistics.getScore(), true);
        SceneManager.LoadScene("LobbyScene");
    }
}