using UnityEngine;

namespace Recykling {
    public class DragController : MonoBehaviour
    {

        public Draggable LastDragged => lastDragged;
        private bool isDragActive = false;
        private Vector2 screenPosition;
        private Vector3 worldPosition;

        private Draggable lastDragged;
        private void Awake()
        {
            DragController[] dragControllers = FindObjectsOfType<DragController>();
            if (dragControllers.Length > 1)
            {
                Destroy(gameObject);
            }
        }
        void Update()
        {
            if(isDragActive && (Input.GetMouseButtonUp(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)))
            {
                Drop();
                return;
            }
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePosition = Input.mousePosition;
                screenPosition = new Vector2(mousePosition.x, mousePosition.y);
            }
            else if (Input.touchCount > 0)
            {
                screenPosition = Input.GetTouch(0).position;
            }
            else return;

            worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            if (isDragActive)
            {
                Drag();
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
                if (hit.collider != null)
                {
                    Draggable draggable = hit.collider.GetComponent<Draggable>();
                    if (draggable != null)
                    {
                        lastDragged = draggable;
                        InitDrag();
                    }
                }
            }
        }

        void InitDrag()
        {
            lastDragged.lastPosition = lastDragged.transform.position;
            UpdateDragStatus(true);
        }

        void Drag()
        {
            lastDragged.transform.position = new Vector2(worldPosition.x, worldPosition.y);
        }

        void Drop()
        {
            UpdateDragStatus(false);
        }

        void UpdateDragStatus(bool isDragging)
        {
            isDragActive = lastDragged.isDragging = isDragging;
            if (isDragging)
            {
                lastDragged.gameObject.layer = 7;
            }
            else
            {
                lastDragged.gameObject.layer = 0;
            }
        }
    }
}
