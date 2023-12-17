using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MinigameWaterSaving
{
    public class Timer : MonoBehaviour
    {
        public UnityEvent OnTimeEndEvent;
        private bool _invokeEventOnce = true;
        private float _interval;
        private float _currentTime;
        private Image _image;

        private void Start()
        {
            _image = GetComponent<Image>();
            _currentTime = _interval;
        }

        private void Update()
        {
            if (_currentTime > 0)
            {
                _currentTime -= Time.deltaTime;
                _image.fillAmount = 1 - (_interval - _currentTime) / _interval;
            }
            else if (_invokeEventOnce)
            {
                OnTimeEndEvent.Invoke();
                _invokeEventOnce = false;
            }
        }

        public float Interval
        {
            set => _interval = value;
        }
    }
}