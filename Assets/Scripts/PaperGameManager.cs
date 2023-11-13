using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class PaperGameManager : MonoBehaviour
{
    public List<Sprite> sprites;
    public Object spritePrefab;
    public List<AnimalStampComponent> stamps;
    // Start is called before the first frame update
    public RectTransform  prefabZone;
    public RectTransform  paperZone;
    public TextMeshProUGUI  scoreText;

    public BoxCollider2D animalZoneCollider;
    public BoxCollider2D paperCollider;

    public Vector3[] animalZoneCorners = new Vector3[4];
    public Vector3[] paperCorners = new Vector3[4];
    
    private float  score;
    private bool thereIsAnError;

    void Start()
    {

        prefabZone.GetWorldCorners(animalZoneCorners);
        paperZone.GetComponent<RectTransform>().GetWorldCorners(paperCorners);

        for (int i = 0; i < 50; i++)
        {
            SpawnInsideTheZone();
        }
    }

    // Update is called once per frame
    void Update()
    {
        score= 0;
        thereIsAnError = false;
        int inAnimalZone = 0;
        foreach (var stamp in stamps)
        {
            if (stamp.inPaper && !stamp.collides)
            {
                score += (50 * stamp.scale) ;
            }
            else if(stamp.inAnimalZone)
            {
                inAnimalZone++;
            }
            else if(!stamp.inAnimalZone || stamp.collides)
            {
                thereIsAnError = true;
            }
        }

        if (inAnimalZone < 50)
        {
            SpawnInsideTheZone();
        }
        
        scoreText.text = "Score: " + score.ToString();
    }

    public void SpawnInsideTheZone()
    {
        List<Vector2> physicsShape = new List<Vector2>();
        float scale = Random.Range(0.5f, 2.5f);
        float rotation = Random.Range(0f, 360f);
        Vector3 spawnPosition = new Vector3(Random.Range(animalZoneCorners[0].x + 50,animalZoneCorners[2].x - 50)   , Random.Range(animalZoneCorners[0].y + 50,animalZoneCorners[2].y - 50) , 0);
        Object spriteObject = Instantiate(spritePrefab, Vector3.zero, Quaternion.identity,this.transform);
        AnimalStampComponent currentStamp = spriteObject.GetComponent<AnimalStampComponent>();
        currentStamp.paperGameManager = this;
        RectTransform rt = spriteObject.GetComponent<RectTransform>();
        rt.position = spawnPosition;
        rt.localScale *= scale;
        rt.Rotate(Vector3.forward,rotation);
        spriteObject.GetComponent<Image>().sprite = sprites[Random.Range(0,sprites.Count)];
        spriteObject.GetComponent<Image>().sprite.GetPhysicsShape(0,physicsShape);
        for (int i = 0; i < physicsShape.Count; i++)
        {
            physicsShape[i] *= 100;
        }
        spriteObject.GetComponent<PolygonCollider2D>().SetPath(0,physicsShape);
        currentStamp.scale = scale;
        stamps.Add(currentStamp);
    }
}
