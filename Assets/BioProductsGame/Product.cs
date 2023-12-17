using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Product : ScriptableObject
{
    [SerializeField]
    private bool ecological;

    [SerializeField]
    private Sprite image;

    public Product(bool ecological, Sprite image)
    {
        this.ecological = ecological;
        this.image = image;
    }
    public Product() { }

    public bool GetEcological()
    { 
        return ecological; 
    }

    public Sprite GetImage()
    {
        return image;
    }
}
