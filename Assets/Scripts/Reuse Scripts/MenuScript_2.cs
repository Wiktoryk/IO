using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reuse_Scripts {
    public class MenuScript : MonoBehaviour
    {
        public GameObject menu;
        public GameObject menuButton;
        public Timer timer;
        public Statistics statistics;
        // Start is called before the first frame update
        void Start()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            menu.SetActive(false);
            menuButton.SetActive(true);
        }

        public void Resume()
        {
            menu.SetActive(false);
            menuButton.SetActive(true);
            timer.timerOn = true;
        }
        public void MenuOn()
        {
            menu.SetActive(true);
            menuButton.SetActive(false);
            timer.timerOn = false;
        }

        public void Repeat() {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }

        public void End() {
            MiniGameStatus.Instance.SetStatus("Ponowne wykorzystanie", 0, false);
            Screen.orientation = ScreenOrientation.Portrait;
            SceneManager.LoadScene("LobbyScene");

        }


    }
}
