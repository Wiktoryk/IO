using System;
using UnityEngine;

namespace MinigameWaterSaving
{
    public class Releaser : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                DropPool.Release(other.GetComponent<Drop>());
            }
        }
    }
}