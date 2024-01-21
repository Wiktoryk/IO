using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollider : MonoBehaviour
{
    //WorkSpace work = new WorkSpace();
    public WorkSpace work;
    public Statistics statistics;
    public Manager manager;
    float posX = 0.6f;
    private void OnTriggerEnter2D(Collider2D other)

    {
        if (name != tag)
        {
            if (other.name == "Bin")
            {
                Destroy(gameObject);
            }
            else if (work.checkRecipe(other.tag, tag) && other.transform.position.y != -6 && transform.position.y != -6)
            {
                    GameObject latarka = GameObject.Find(work.getNewObject(other.tag, tag));
                    posX = manager.posX;
                    manager.UpdatePosition();
                    Vector3 newPosition = new Vector3(posX, -6f,-1f);
                    latarka.transform.position = newPosition;
                    GameObject latarkaObject = Instantiate(latarka, other.transform.position, Quaternion.identity);
                    latarkaObject.transform.position = other.transform.position;
                    latarkaObject.transform.localScale = new Vector3(3.5f, 3.5f, 1f);
                if (!work.RecipeId())
                {
                    statistics.UpdateScore();
                }
                    Destroy(gameObject);
                    Destroy(other.gameObject);
           
            }
        }
    }

}
