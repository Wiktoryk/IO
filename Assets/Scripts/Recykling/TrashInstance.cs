using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TrashType
{
    Paper,
    Plastic,
    Metal,
    Glass,
    Bio,
    Other
}

public class TrashInstance : MonoBehaviour
{
    [Header("TrashType")]
    public TrashType trashType;

    [Header("Sprites")]
    public SpriteRenderer spriteRenderer;
    public Sprite paperSprite;
    public Sprite plasticSprite;
    public Sprite metalSprite;
    public Sprite glassSprite;
    public Sprite bioSprite;
    public Sprite otherSprite;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        switch (trashType)
        {
            case TrashType.Paper:
                spriteRenderer.sprite = paperSprite;
                break;
            case TrashType.Plastic:
                spriteRenderer.sprite = plasticSprite;
                break;
            case TrashType.Metal:
                spriteRenderer.sprite = metalSprite;
                break;
            case TrashType.Glass:
                spriteRenderer.sprite = glassSprite;
                break;
            case TrashType.Bio:
                spriteRenderer.sprite = bioSprite;
                break;
            case TrashType.Other:
                spriteRenderer.sprite = otherSprite;
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
