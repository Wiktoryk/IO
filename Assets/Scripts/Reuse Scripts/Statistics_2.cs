using UnityEngine;

namespace Reuse_Scripts {
    public class Statistics : MonoBehaviour
    {
        int mnoznik = 10;
        int score = 0;
        // Start is called before the first frame update
        public void UpdateScore()
        {
            score += 1;
        }

        // Update is called once per frame
        public int getScore()
        {
            return mnoznik*score;
        }
    }
}
