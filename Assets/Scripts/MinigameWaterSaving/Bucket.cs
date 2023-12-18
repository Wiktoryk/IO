using System;
using UnityEngine;

namespace MinigameWaterSaving
{
    public class Bucket : MonoBehaviour
    {
        private static Statistics _statistics = new Statistics();
        private Camera _camera;
        private static bool _canDrag;

        private void Start()
        {
            _camera = Camera.main;
            _statistics.Init();
            _canDrag = true;
        }

        private void OnMouseDrag()
        {
            if(!_canDrag) return;
            
            Vector2 v = Input.mousePosition;
            v = _camera.ScreenToWorldPoint(v);

            transform.position = v;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                DropPool.Release(other.GetComponent<Drop>());
                _statistics.GainPoint(1);
            }
        }

        public static void PlayerWon()
        {
            _statistics.SetPlayerWon(true);
            _canDrag = false;
            Time.timeScale = 0;
        }

        public static (bool, int) GetResult()
        {
            return _statistics.GetResult();
        }
    }
}