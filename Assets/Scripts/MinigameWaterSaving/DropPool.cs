using System;
using UnityEngine;
using UnityEngine.Pool;

namespace MinigameWaterSaving
{
    public class DropPool : MonoBehaviour
    {
        [SerializeField] private Drop _dropPrefab;
        private static ObjectPool<Drop> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<Drop>(
                () =>
                {
                    return Instantiate(_dropPrefab);
                },
                arg0 =>
                {
                    arg0.gameObject.SetActive(true);
                },
                arg0 => arg0.gameObject.SetActive(false)
                );

            _pool.Get();
        }

        public static void Get(Vector2 pos)
        {
            Drop d = _pool.Get();

            d.transform.position = pos;
        }

        public static void Release(Drop d)
        {
            _pool.Release(d);
        }
    }
}