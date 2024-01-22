using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Reuse_Scripts {
    public class Timer : MonoBehaviour
    {
        [SerializeField]
        public Text text;
        private float sekunda = 0.0f;
        public float iloscsekund = 20.0f;
        public Statistics statistics;
        public bool timerOn = true;
        private void Awake()
        {
            text = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            if (timerOn)
            {

                sekunda += Time.deltaTime;
                if (sekunda >= 1)
                {
                    iloscsekund -= 1;
                    sekunda = 0;
                }
                text.text = iloscsekund.ToString();
                if (iloscsekund <= 0)
                {
                    Debug.Log(statistics.getScore());

                    Screen.orientation = ScreenOrientation.Portrait;
                    MiniGameStatus.Instance.SetStatus("Ponowne wykorzystanie", statistics.getScore(), true);
                    SceneManager.LoadScene("LobbyScene");
                }
            }
        }
    }
}
