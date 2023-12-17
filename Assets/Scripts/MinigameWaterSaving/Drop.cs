using System;
using UnityEngine;

namespace MinigameWaterSaving
{
    public class Drop : MonoBehaviour
    {
        [SerializeField] private float _fallingSpeed = 5;

        private void Start()
        {
            
        }

        private void Update()
        {
            Vector2 v = transform.position;

            v += Vector2.down * (_fallingSpeed * Time.deltaTime);
            
            Move(v);
        }

        private void Move(Vector2 nextPos)
        {
            transform.position = nextPos;
        }
    }
}