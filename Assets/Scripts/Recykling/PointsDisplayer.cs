using TMPro;
using UnityEngine;

namespace Recykling {
    public class PointsDisplayer : MonoBehaviour
    {
        public RecyklingGameScript rgs;
        public TMP_Text pointsText;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            UpdatePoints();
        }

        public void UpdatePoints()
        {
            pointsText.text = "Points: " + rgs.pointsCounter.GetPoints();
        }
    }
}
