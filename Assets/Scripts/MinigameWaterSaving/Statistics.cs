namespace MinigameWaterSaving
{
    public class Statistics
    {
        private int _score;
        private bool _playerWon;

        public void Init()
        {
            _score = 0;
            _playerWon = false;
        }

        public void GainPoint(int points)
        {
            _score += points;
            UI.Singleton.UpdateScore(_score);
        }

        public void SetPlayerWon(bool val)
        {
            _playerWon = val;
        }

        public (bool, int) GetResult()
        {
            return (_playerWon, _score);
        }
    }
}