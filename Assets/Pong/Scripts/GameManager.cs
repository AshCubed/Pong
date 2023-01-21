using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

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
        [SerializeField] private Color _colorPaddle1;
        [SerializeField] private Color _colorPaddle2;
        [Header("Ui")]
        [SerializeField] private TMP_Text _txtTime;
        [SerializeField] private TMP_Text _txtPlayer1Score;
        [SerializeField] private TMP_Text _txtPlayer2Score;
        [Header("On Triggers")]
        [SerializeField] private OnCollision _onCollisionMainWall;
        [SerializeField] private OnTrigger _onTriggerBackWall;
        [SerializeField] private OnCollision _onCollisionRightWall;
        [SerializeField] private OnCollision _onCollisionLeftWall;

        private Paddle _paddleP1;
        private Paddle _paddleP2;
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
            _ball.BallColor = _ballDefaultColor;

            _onCollisionMainWall.OnCollisionEvent.AddListener(BallHitMainWall);
            _onTriggerBackWall.OnTriggerEvent.AddListener(BallHitScoreTrigger);
            _onCollisionRightWall.OnCollisionEvent.AddListener(BallHitOtherWall);
            _onCollisionLeftWall.OnCollisionEvent.AddListener(BallHitOtherWall);
            
            ResetBall();
        }

        // Update is called once per frame
        private void Update()
        {
            if (_isGameRunning)
            {
                Timer();
            }
        }
        
        public void PlayerInputManagerOnonPlayerJoined(PlayerInput obj)
        {
            if (!_paddleP1)
            {
                _paddleP1 = obj.gameObject.GetComponent<Paddle>();
                _paddleP1.PaddleColor = _colorPaddle1;
            }
            else
            {
                _paddleP2 = obj.gameObject.GetComponent<Paddle>();
                _paddleP2.PaddleColor = _colorPaddle2;
            }

            if (_paddleP1 && _paddleP2)
            {
                _isGameRunning = true;
                _isTimerRunning = true;
                LaunchBall();
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
                    if (_currentScorer)
                        _currentScorer.HasCollision = true;
                    if (_ball.LastPlayerToHit)
                    {
                        _currentScorer = _ball.LastPlayerToHit;
                        var c = _currentScorer.PaddleColor;
                        c.a = 1;
                        _ball.BallColor = c;
                        _currentScorer.HasCollision = false;
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
                    _ball.ResetLastPlayer(_ballDefaultColor);
                    _paddleP1.HasCollision = true;
                    _paddleP2.HasCollision = true;
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
                    _paddleP1.HasCollision = true;
                    _paddleP2.HasCollision = true;
                    if (_currentScorer != null)
                    {
                        if (_currentScorer == _paddleP1)
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
                    LaunchBall();
                }
            }
        }

        private void LaunchBall()
        {
            LeanTween.delayedCall(2f, () =>
            {
                _ball.gameObject.transform.position = _ballRestartPoint.position;
                _ball.Launch();
            });
        }

        private void ResetBall()
        {
            _ball.Stop();
            _ball.ResetLastPlayer(_ballDefaultColor);
        }
    }
}
