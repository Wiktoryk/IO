using UnityEngine;

namespace TransportScript {
    public class Touch : MonoBehaviour
    {
        [SerializeField]
        // private Transform obiekt;
        private float deltaX, deltaY;
        public static bool locked;
        public bool isOrginal = true;
        void Update()
        {
            if(Input.touchCount > 0 && !locked)
            {
                UnityEngine.Touch touch = Input.GetTouch(0);
                Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);


                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos))
                        {

                            deltaX = touchPos.x - transform.position.x;
                            deltaY = touchPos.y - transform.position.y;

                        }

                        break;
                    case TouchPhase.Moved:
                        if(GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos))
                        {
                            transform.position = new Vector2(touchPos.x - deltaX, touchPos.y - deltaY);
                        }
                        break;
                    case TouchPhase.Ended:
                        //transform.position = new Vector2(initialPosition.x, initialPosition.y);
                        break;
                }

            }
        }
    }
}
