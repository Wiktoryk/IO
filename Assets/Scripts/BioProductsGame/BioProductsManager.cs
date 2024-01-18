using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class BioProductsManager : MonoBehaviour
{
    private Product currentProduct;
    private int score;
    [SerializeField]
    private float timeLeft;

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;
    // Start is called before the first frame update
    void Start()
    {
        score = 100;
        timeLeft = 30;
        GenerateProduct();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
        }

        GetComponent<BioGameDisplay>().UpdateDisplay(currentProduct, score, timeLeft);


        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            startTouchPos = Input.GetTouch(0).position;
        }

        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            endTouchPos = Input.GetTouch(0).position;

            if(endTouchPos.x < startTouchPos.x)
            {
                Choose(false);
            }

            if(endTouchPos.x > startTouchPos.x)
            {
                Choose(true);
            }
        }
    }

    void GenerateProduct()
    {
        int numberOfProducts = 0;
        var products = Resources.LoadAll("Products/");
        numberOfProducts = products.Length;

        int index = Random.Range(1, numberOfProducts + 1);
        while (Resources.Load<Product>("Products/product" + index.ToString()) == currentProduct)
        {
            index = Random.Range(1, numberOfProducts + 1);
        }

        currentProduct = Resources.Load<Product>("Products/product" + index.ToString());
        //currentProduct = Resources.Load<Product>("Products/product1");
    }

    void Choose(bool ecological)
    {
        if (currentProduct.GetEcological() == ecological)
        {
            score += 10;
        }
        else
        {
            score -= 10;
        }
        GenerateProduct();
    }

    public int GetScore() { return score; }
    public Product GetProduct() { return currentProduct; }
    //asasa
}
