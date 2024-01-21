using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HasTouchedButton();
    }

    //check if player touched exit text on touch screen
    private void HasTouchedButton()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 screenPosition = Input.mousePosition;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            print(worldPosition);
            if (GetComponent<Collider2D>().OverlapPoint(worldPosition))
            {
                SceneManager.LoadScene(0);
            }
        }
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 screenPosition = touch.position;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            print(worldPosition);
            if (GetComponent<Collider2D>().OverlapPoint(worldPosition))
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}
