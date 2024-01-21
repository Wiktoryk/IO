using UnityEngine;
using UnityEngine.UI;

namespace TransportScript {
    public class Wynik : MonoBehaviour
    {
        [SerializeField]
        public Text text;
        // Start is called before the first frame update
        void Start()
        {
            int wynikGry = PlayerPrefs.GetInt("WynikGry", 0);
            text.text = "Twój wynik to: "+ wynikGry.ToString();
        }

    }
}
