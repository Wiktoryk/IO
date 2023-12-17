using UnityEngine;
using UnityEngine.SceneManagement;

namespace MinigameWaterSaving
{
    public class MinigameWaterSaving : MonoBehaviour, IMinigame
    {
        public void PlayMinigame()
        {
            SceneManager.LoadScene("MinigameWaterSaving");
        }

        public (bool, int) GetResult()
        {
            return Bucket.GetResult();
        }
    }
}