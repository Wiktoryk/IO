using UnityEngine;

namespace Reuse_Scripts {
    public class Touch : MonoBehaviour
    {
        [SerializeField]
        // private Transform obiekt;
        private float deltaX, deltaY;
        public static bool locked;
        public bool isOrginal = true;
        // Update is called once per frame
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
                            if (transform.position.y <= -6)
                            {
                                GameObject kopia = GameObject.Find(gameObject.name);
                                GameObject kopiaObject = Instantiate(kopia, gameObject.transform.position, Quaternion.identity);
                                kopiaObject.transform.position = new Vector2(touchPos.x - deltaX, -4f);
                                kopiaObject.transform.localScale = new Vector3(3.5f, 3.5f, 1f);
                            }
                            else
                            {
                                deltaX = touchPos.x - transform.position.x;
                                deltaY = touchPos.y - transform.position.y;
                            }
                        }

                        break;
                    case TouchPhase.Moved:
                        if(GetComponent<Collider2D>() == Physics2D.OverlapPoint(touchPos))
                        {
                            if (transform.position.y <= -6)
                            {

                            }
                            else
                            {
                                transform.position = new Vector2(touchPos.x - deltaX, touchPos.y - deltaY);
                            }
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
