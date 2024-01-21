using System;
using UnityEngine;

namespace NonRecycleScripts {
    public class Movement : MonoBehaviour
    {
        private InputManager inputs;
        private Rigidbody2D rb;

        public float Speed;

        Vector2 moveDir;
        bool isMoving;


        void Awake()
        {
            inputs = GetComponent<InputManager>();
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            moveDir = inputs.MoveInput.normalized;

            isMoving = Convert.ToBoolean(moveDir.magnitude);

            //Rotate();
        }

        void FixedUpdate()
        {
            if (rb != null)
            {
                rb.MovePosition(rb.position + moveDir * Speed * Time.fixedDeltaTime);
            }
        }

        void Rotate()
        {
            if (isMoving)
            {
                float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}