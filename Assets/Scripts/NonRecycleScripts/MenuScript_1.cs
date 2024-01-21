using UnityEngine;
using UnityEngine.SceneManagement;

namespace NonRecycleScripts {
    public class MenuScript : MonoBehaviour
    {
        public GameObject menu;
        public GameObject menuButton;
        public Statistics statistics;
        public GameObject destroyObject;
        public GameObject destroyObject2;
        // Start is called before the first frame update
        void Start()
        {
            menu.SetActive(false);
            menuButton.SetActive(true);
        }

        public void Resume()
        {
            menu.SetActive(false);
            menuButton.SetActive(true);
        }
        public void MenuOn()
        {
            menu.SetActive(true);
            menuButton.SetActive(false);
        }

        public void Repeat()
        {
            Destroy(destroyObject);
            Destroy(destroyObject2);
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }

        public void End()
        {
            //MiniGameStatus.Instance.SetStatus("Ponowne wykorzystanie", 0, false);
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
