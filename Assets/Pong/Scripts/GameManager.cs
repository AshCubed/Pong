using UnityEngine;
using TMPro;

namespace Pong
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")] 
        [SerializeField] private float _gameTime;
        [Header("Ball")]
        [SerializeField] private Ball _ball;
        [SerializeField] private Color _ballDefaultColor;
        [SerializeField] private Transform _ballRestartPoint;
        [Header("Players")] 
        [SerializeField] private Paddle _paddleP1;
        [SerializeField] private Paddle _paddleP2;
        [Header("Ui")]
        [SerializeField] private TMP_Text _txtTime;
        [SerializeField] private TMP_Text _txtPlayer1Score;
        [SerializeField] private TMP_Text _txtPlayer2Score;
        [Header("On Triggers")]
        [SerializeField] private OnCollision _onCollisionMainWall;
        [SerializeField] private OnTrigger _onTriggerBackWall;
        [SerializeField] private OnCollision _onCollisionRightWall;
        [SerializeField] private OnCollision _onCollisionLeftWall;

        private float _currentGameTime;
        private int _scorePlayer1;
        private int _scorePlayer2;
        private Paddle _currentScorer;
        private bool _isGameRunning;
        private bool _isTimerRunning;
        
        
        // Start is called before the first frame update
        private void Start()
        {
            _currentGameTime = _gameTime;
            _scorePlayer1 = 0;
            _scorePlayer2 = 0;
            _ball.BallColor = Color.white;

            _onCollisionMainWall.OnCollisionEvent.AddListener(BallHitMainWall);
            _onTriggerBackWall.OnTriggerEvent.AddListener(BallHitScoreTrigger);
            _onCollisionRightWall.OnCollisionEvent.AddListener(BallHitOtherWall);
            _onCollisionLeftWall.OnCollisionEvent.AddListener(BallHitOtherWall);

            _isGameRunning = true;
            _isTimerRunning = true;
        }

        // Update is called once per frame
        private void Update()
        {
            if (_isGameRunning)
            {
                Timer();
            }
        }

        private void Timer()
        {
            if (_currentGameTime > 0 && _isTimerRunning)
            {
                _currentGameTime -= Time.deltaTime;
                _txtTime.text = DisplayTime(_currentGameTime);
            }
            else
            {
                _isTimerRunning = false;
                _txtTime.text = "00:00";
                ResetBall();
            }

            string DisplayTime(float timeToDisplay)
            {
                float min = Mathf.FloorToInt(timeToDisplay / 60);
                float sec = Mathf.FloorToInt(timeToDisplay % 60);
                return $"{min:00}:{sec:00}";
            }
        }

        private void BallHitMainWall(Collision2D x, OnContactType y)
        {
            if (y == OnContactType.ENTER)
            {
                if (x.collider.CompareTag("Ball"))
                {
                    if (_currentScorer != _ball.LastPlayerToHit)
                    {
                        _currentScorer = _ball.LastPlayerToHit;
                        _ball.BallColor = _currentScorer.PaddleColor;
                    }
                }
            }
            
        }

        private void BallHitOtherWall(Collision2D x, OnContactType y)
        {
            if (y == OnContactType.ENTER)
            {
                if (x.collider.CompareTag("Ball"))
                {
                    _currentScorer = null; 
                    _ball.BallColor = Color.white;
                }
            }
        }

        private void BallHitScoreTrigger(Collider2D x, OnContactType y)
        {
            if (y == OnContactType.ENTER)
            {
                if (x.CompareTag("Ball"))
                {
                    ResetBall();
                    if (_currentScorer != null)
                    {
                        if (_currentScorer.IsPlayer1)
                        {
                            _scorePlayer1 += 1;
                            _txtPlayer1Score.text = _scorePlayer1.ToString();
                        }
                        else
                        {
                            _scorePlayer2 += 1;
                            _txtPlayer2Score.text = _scorePlayer2.ToString();
                        }
                    }
                    LeanTween.delayedCall(2f, () =>
                    {
                        _ball.gameObject.transform.position = _ballRestartPoint.position;
                        _ball.Launch();
                    });
                }
            }
        }

        private void ResetBall()
        {
            _ball.Stop();
            _ball.ResetLastPlayer(_ballDefaultColor);
        }
    }
}
