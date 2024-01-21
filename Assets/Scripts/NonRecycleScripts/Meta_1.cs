using UnityEngine;
using UnityEngine.SceneManagement;

namespace NonRecycleScripts {
    public class Meta : MonoBehaviour
    {
        public Statistics statistics;
        public  GameObject destroyObject;
        public GameObject destroyObject2;
        // Update is called once per frame
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Destroy(destroyObject);
            Destroy(destroyObject2);
            MiniGameStatus.Instance.SetStatus("non-recycle", statistics.getScore(), true);
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
