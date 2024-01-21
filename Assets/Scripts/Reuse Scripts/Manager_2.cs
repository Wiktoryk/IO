using UnityEngine;

namespace Reuse_Scripts {
    public class Manager : MonoBehaviour
    {
        public float posX = 0.7f;

        public void UpdatePosition()
        {
            posX += 1.2f;
        }
        private float GetPosition()
        {
            return posX;
        }
    }
}
