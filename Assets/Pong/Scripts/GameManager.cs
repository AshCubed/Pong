using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Pong
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")] 
        [SerializeField] private float _gameTime;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private CameraShake _cameraShake;
        [SerializeField] private float _hitAmplitudeGainWall, _hitFrequencyGainWall, _shakeTimeWall;
        [SerializeField] private float _hitAmplitudeGainScore, _hitFrequencyGainScore, _shakeTimeScore;
        [Header("Ball")]
        [SerializeField] private Ball _ball;
        [SerializeField] private float _ballLaunchDelay;
        [SerializeField] private Color _ballDefaultColor;
        [SerializeField] private Transform _ballRestartPoint;
        [Header("Players")] 
        [SerializeField] private Color _colorPaddle1;
        [SerializeField] private Color _colorPaddle2;
        [Header("Ui - Main")]
        [SerializeField] private TMP_Text _txtTime;
        [SerializeField] private TMP_Text _txtPlayer1Score;
        [SerializeField] private TMP_Text _txtPlayer2Score;
        [Header("Ui - Player Join")]
        [SerializeField] private GameObject _joinPlayer1;
        [SerializeField] private GameObject _joinPlayer2;
        [Header("Ui - Score Animation")]
        [SerializeField] private AnimationCurve _animationCurveScored;
        [SerializeField] private float _inTime;
        [SerializeField] private float _outTime;
        [SerializeField] private CanvasGroup _groupPlayer1Scored;
        [SerializeField] private CanvasGroup _groupPlayer2Scored;
        [SerializeField] private Image _imagePlayer1Scored;
        [SerializeField] private Image _imagePlayer2Scored;
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
            InitGame();
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

        private void InitGame()
        {
            _currentGameTime = _gameTime;
            _scorePlayer1 = 0;
            _scorePlayer2 = 0;
            _ball.BallColor = _ballDefaultColor;
            _groupPlayer1Scored.alpha = 0;
            _groupPlayer2Scored.alpha = 0;
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
                _isGameRunning = false;
                _txtTime.text = "00:00";
                ResetBall();
                _gameOverScreen.PlayAnimation(_paddleP1.PaddleColor, _paddleP2.PaddleColor, 
                    _scorePlayer1, _scorePlayer2);
                PlayerMovement(false);
            }

            string DisplayTime(float timeToDisplay)
            {
                float min = Mathf.FloorToInt(timeToDisplay / 60);
                float sec = Mathf.FloorToInt(timeToDisplay % 60);
                return $"{min:00}:{sec:00}";
            }
        }
        
        private void AnimationScore(CanvasGroup canvasGroup)
        {
            var groupGameObject = canvasGroup.gameObject;
            groupGameObject.transform.localScale = new Vector3(.1f, .1f, .1f);
            LeanTween.alphaCanvas(canvasGroup, 1f, _inTime);
            var lean = LeanTween.scale(groupGameObject, new Vector3(0.9f, 0.9f, 0.9f), _inTime).setOnComplete(() =>
            {
                LeanTween.delayedCall(0.5f, () =>
                {
                    LeanTween.alphaCanvas(canvasGroup, 0f, _outTime);
                    LeanTween.scale(groupGameObject, new Vector3(0.1f, 0.1f, 0.1f), _outTime);
                });
            });
            lean.setEase(_animationCurveScored);
        }

        public void Retry()
        {
            var waitTime = _gameOverScreen.InitScreen();
            LeanTween.delayedCall(waitTime, () =>
            {
                InitGame();
                _isGameRunning = true;
                _isTimerRunning = true;
                PlayerMovement(true);
                LaunchBall();
            });
        }

        private void PlayerMovement(bool canMove)
        {
            _paddleP1.CanMove = canMove;
            _paddleP2.CanMove = canMove;
            _paddleP1.HasCollision = canMove;
            _paddleP2.HasCollision = canMove;
        }

        #region Player Input Joined Events Calls
        public void PlayerInputManagerOnonPlayerJoined(PlayerInput obj)
        {
            if (!_paddleP1)
            {
                _paddleP1 = obj.gameObject.GetComponent<Paddle>();
                _paddleP1.PaddleColor = _colorPaddle1;
                _imagePlayer1Scored.color = _colorPaddle1;
                _paddleP1.CanMove = true;
                _joinPlayer1.SetActive(false);
            }
            else
            {
                _paddleP2 = obj.gameObject.GetComponent<Paddle>();
                _paddleP2.PaddleColor = _colorPaddle2;
                _imagePlayer2Scored.color = _colorPaddle2;
                _paddleP2.CanMove = true;
                _joinPlayer2.SetActive(false);
            }

            if (_paddleP1 && _paddleP2)
            {
                PlayerMovement(true);
                _isGameRunning = true;
                _isTimerRunning = true;
                LaunchBall();
            }
        }
        
        public void PlayerInputManagerOnonPlayerLeft(PlayerInput obj)
        {
            _isGameRunning = false;
            _isTimerRunning = false;
            ResetBall();
            var paddle = obj.gameObject.GetComponent<Paddle>();
            if (paddle == _paddleP1)
            {
                _joinPlayer1.SetActive(true);
            }
            else
            {
                _joinPlayer2.SetActive(true);
            }
        }
        #endregion

        #region Ball Code
        private void BallHitMainWall(Collision2D x, OnContactType y)
        {
            if (y == OnContactType.ENTER)
            {
                if (x.collider.CompareTag(Constants.Tags.BALL))
                {
                    _cameraShake.Shake(_hitAmplitudeGainWall, _hitFrequencyGainWall, _shakeTimeWall);
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
                if (x.collider.CompareTag(Constants.Tags.BALL))
                {
                    _cameraShake.Shake(_hitAmplitudeGainWall, _hitFrequencyGainWall, _shakeTimeWall);
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
                if (x.CompareTag(Constants.Tags.BALL))
                {
                    ResetBall();
                    _paddleP1.HasCollision = true;
                    _paddleP2.HasCollision = true;
                    if (_currentScorer != null)
                    {
                        _cameraShake.Shake(_hitAmplitudeGainScore, _hitFrequencyGainScore, _shakeTimeScore);
                        if (_currentScorer == _paddleP1)
                        {
                            _scorePlayer1 += 1;
                            _txtPlayer1Score.text = _scorePlayer1.ToString();
                            AnimationScore(_groupPlayer1Scored);
                        }
                        else
                        {
                            _scorePlayer2 += 1;
                            _txtPlayer2Score.text = _scorePlayer2.ToString();
                            AnimationScore(_groupPlayer2Scored);
                        }
                    }
                    LaunchBall();
                }
            }
        }

        private void LaunchBall()
        {
            LeanTween.delayedCall(_ballLaunchDelay, () =>
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
        #endregion
    }
}
