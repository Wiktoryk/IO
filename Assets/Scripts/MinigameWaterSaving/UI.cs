using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MinigameWaterSaving
{
    public class UI : MonoBehaviour
    {
        public static UI Singleton;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Timer _timer;
        [SerializeField] private float _duration = 20f;
        [SerializeField] private GameObject _endCanvas;
        [SerializeField] private TextMeshProUGUI _endScore;

        private void Awake()
        {
            _endCanvas.SetActive(false);
            Singleton = this;
            _timer.Interval = _duration;
            _scoreText.text = "0";
        }

        private void OnEnable()
        {
            _timer.OnTimeEndEvent.AddListener(Bucket.PlayerWon);
            _timer.OnTimeEndEvent.AddListener(ShowEndCanvas);
        }

        private void OnDisable()
        {
            _timer.OnTimeEndEvent.RemoveListener(ShowEndCanvas);
            _timer.OnTimeEndEvent.RemoveListener(Bucket.PlayerWon);
        }

        private void ShowEndCanvas()
        {
            _endCanvas.SetActive(true);
            _endScore.text += _scoreText.text;
        }

        public void UpdateScore(int score)
        {
            _scoreText.text = score.ToString();
        }

        public void ToLobby()
        {
            MiniGameStatus.Instance.SetStatus("Oszczedzanie wody", Bucket.GetResult().Item2, Bucket.GetResult().Item1);
            SceneManager.LoadScene(0);
            Time.timeScale = 1;
        }
    }
}