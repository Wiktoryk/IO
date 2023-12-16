using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioGameDisplay : MonoBehaviour
{
    GameObject currentProductDisplay;
    GameObject scoreDisplay;
    int previousScore;

    // Start is called before the first frame update
    void Start()
    {
        currentProductDisplay = GameObject.Find("CurrentProductDisplay");
        if(currentProductDisplay == null)
        {
            Debug.LogWarning("Unsuccessful assignment of CurrentProductDisplay");
        }
        scoreDisplay = GameObject.Find("ScoreDisplay");
        previousScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateDisplay(Product currentProduct, int score)
    {
        if (currentProductDisplay.GetComponent<UnityEngine.UI.Image>().sprite == null || currentProduct.GetImage() != currentProductDisplay.GetComponent<UnityEngine.UI.Image>().sprite) 
        {
            UnityEngine.UI.Image img = currentProductDisplay.GetComponent<UnityEngine.UI.Image>();
            img.sprite = currentProduct.GetImage();
        }

        if (score != previousScore)
        {
            UnityEngine.UI.Text textField = scoreDisplay.GetComponent<UnityEngine.UI.Text>();
            textField.text = "Score: " + score.ToString();
            previousScore = score;
        }
    }
}
