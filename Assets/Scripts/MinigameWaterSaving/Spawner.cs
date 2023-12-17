using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MinigameWaterSaving
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Vector2 _xRange;
        [SerializeField] private Vector2 _yRange;
        [SerializeField] private float _interval = 1f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;

            Vector2 leftUpC = new Vector2(_xRange.x, _yRange.y);
            Vector2 leftDownC = new Vector2(_xRange.x, _yRange.x);
            Vector2 rightUpC = new Vector2(_xRange.y, _yRange.y);
            Vector2 rightDownC = new Vector2(_xRange.y, _yRange.x);

            Gizmos.DrawLine(leftDownC, leftUpC);
            Gizmos.DrawLine(leftDownC, rightDownC);
            Gizmos.DrawLine(leftUpC, rightUpC);
            Gizmos.DrawLine(rightUpC, rightDownC);

        }

        private void Start()
        {
            StartCoroutine(Spawn());
        }

        public Vector2 GetFreePosition()
        {
            return new Vector2(Random.Range(_xRange.x, _xRange.y), Random.Range(_yRange.x, _yRange.y));
        }
        
        public IEnumerator Spawn()
        {
            DropPool.Get(GetFreePosition());
            yield return new WaitForSeconds(_interval);
            StartCoroutine(Spawn());
            yield return null;
        }
    }
}