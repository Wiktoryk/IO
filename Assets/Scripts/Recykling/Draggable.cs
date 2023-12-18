using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    public bool isDragging;
    public Vector3 lastPosition;

    private Collider2D collider;
    private DragController dragController;
    private float movementTime = 15f;
    private System.Nullable<Vector3> movementDestination;
    private TrashInstance trashInstance;

    private void Start()
    {
        trashInstance = GetComponent<TrashInstance>();
        collider = GetComponent<Collider2D>();
        dragController = FindObjectOfType<DragController>();
    }

    private void FixedUpdate()
    {
        if(movementDestination.HasValue)
        {
            if (isDragging)
            {
                movementDestination = null;
                return;
            }
            if(transform.position == movementDestination)
            {
                gameObject.layer = 0;
                movementDestination = null;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, movementDestination.Value, Time.fixedDeltaTime * movementTime);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Draggable collidedDraggable = collision.GetComponent<Draggable>();
        if(collidedDraggable != null && dragController.LastDragged.gameObject == gameObject)
        {
            ColliderDistance2D colliderDistance2D = collision.Distance(collider);
            Vector3 diff = new Vector3(colliderDistance2D.normal.x, colliderDistance2D.normal.y) * colliderDistance2D.distance;
            transform.position -= diff;
        }

        ContainerInstance containerInstance = collision.GetComponent<ContainerInstance>();
        TrashType trashType = trashInstance.trashType;
        if(containerInstance != null )
        {
               if(containerInstance.containerType == TrashToContainerType(trashType))
            {
                movementDestination = collision.transform.position;
                containerInstance.rgs.pointsCounter.AddPoints(1);
                containerInstance.rgs.trashCollected++;
                Destroy(gameObject);
            }
            else if (containerInstance.containerType != TrashToContainerType(trashType))
            {
                containerInstance.rgs.gameTimer.timeLeft -= 5f;
                movementDestination = lastPosition;
                
            }
        }

    }

    private ContainerType TrashToContainerType(TrashType trashType)
    {
        switch (trashType)
        {
            case TrashType.Paper:
                return ContainerType.Paper;
            case TrashType.Plastic:
                return ContainerType.PlasticAndMetal;
            case TrashType.Metal:
                return ContainerType.PlasticAndMetal;
            case TrashType.Glass:
                return ContainerType.Glass;
            case TrashType.Bio:
                return ContainerType.Bio;
            case TrashType.Other:
                return ContainerType.Other;
            default:
                return ContainerType.Other;
        }
    }



}
