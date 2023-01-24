using System;
using System.Linq;
using Pong.Audio;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace Pong
{
    public class GameManager : MonoBehaviour, IPause
    {
        [Header("Game Settings")] 
        [SerializeField] private float _gameTime;
        [SerializeField] private float _waitTimeAfterRetryPress;
        [Header("Ball")]
        [SerializeField] private Ball _ball;
        [SerializeField] private float _ballLaunchDelay;
        [SerializeField] private Color _ballDefaultColor;
        [SerializeField] private Transform _ballRestartPoint;
        [Header("Players")] 
        [SerializeField] private Transform _player1StartPos;
        [SerializeField] private Transform _player2StartPos;
        [SerializeField] private Color _colorPaddle1;
        [SerializeField] private Color _colorPaddle2;
        [Header("Ui - Main")]
        [SerializeField] private TMP_Text _txtTime;
        [SerializeField] private TMP_Text _txtBallLaunchCountdown;
        [Header("On Triggers")]
        [SerializeField] private OnCollision _onCollisionMainWall;
        [SerializeField] private OnTrigger _onTriggerBackWall;
        [SerializeField] private OnCollision _onCollisionRightWall;
        [SerializeField] private OnCollision _onCollisionLeftWall;
        
        private Action _onGameInit;
        private Action<Paddle, int, int> _onPlayerScoreChange;
        private Action<BallHit> _onBallHit;
        private Action<float> _onGameRetry;
        private Action<Color, Color, int, int> _onGameEnd;
        private Action<Paddle, Color> _onPlayerJoined;
        private Action<Paddle> _onPlayerLeft;

        private Paddle _paddleP1;
        private Paddle _paddleP2;
        private float _currentGameTime;
        private int _scorePlayer1;
        private int _scorePlayer2;
        private Paddle _currentScorer;
        private bool _isGameRunning;
        private bool _isTimerRunning;
        private LTDescr _leanBallReset;
        
        // Start is called before the first frame update
        private void Start()
        {
            //Find all MonoBehaviours which use the IGameManager interface and add them to the relevant actions.
            foreach (var item in FindObjectsOfType<MonoBehaviour>(true).OfType<IGameManager>())
            {
                _onGameInit += item.OnInit;
                _onPlayerScoreChange += item.OnPlayerScoreChange;
                _onBallHit += item.OnBallHit;
                _onGameRetry += item.OnGameRetry;
                _onGameEnd += item.OnGameEnd;
                _onPlayerJoined += item.OnPlayerJoined;
                _onPlayerLeft += item.OnPlayerLeft;
            }
            
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
            _txtBallLaunchCountdown.gameObject.SetActive(false);
            _onPlayerScoreChange?.Invoke(null, _scorePlayer1, _scorePlayer2);
            _onGameInit?.Invoke();
        }

        private void StartGame()
        {
            if (_paddleP1 && _paddleP2)
            {
                LaunchBall(() =>
                {
                    PlayerMovement(true);
                    _isGameRunning = true;
                    _isTimerRunning = true;
                });
            }
        }
        
        private void Timer()
        {
            if (_isTimerRunning)
            {
                if (_currentGameTime > 0)
                {
                    _currentGameTime -= Time.deltaTime;
                    _txtTime.text = DisplayTime(_currentGameTime);
                }
                else
                {
                    EndGame();
                }
            }
            
            string DisplayTime(float timeToDisplay)
            {
                float min = Mathf.FloorToInt(timeToDisplay / 60);
                float sec = Mathf.FloorToInt(timeToDisplay % 60);
                return $"{min:00}:{sec:00}";
            }
        }

        private void EndGame()
        {
            _isTimerRunning = false;
            _isGameRunning = false;
            _txtTime.text = "00:00";
            ResetBall();
            PlayerMovement(false);
            AudioManager.Instance.PlaySounds("GameOver");
            AudioManager.Instance.FadeOutAudio("BackgroundMusic", null);
            _onGameEnd?.Invoke(_paddleP1.PaddleColor, _paddleP2.PaddleColor, 
                _scorePlayer1, _scorePlayer2);
        }

        public void Retry()
        {
            AudioManager.Instance.PlaySounds("BackgroundMusic");
            AudioManager.Instance.StopSounds("GameOver");
            InitGame();
            _onGameRetry?.Invoke(_waitTimeAfterRetryPress);
            LeanTween.delayedCall(_waitTimeAfterRetryPress, () =>
            {
                LaunchBall(() =>
                {
                    _isGameRunning = true;
                    _isTimerRunning = true;
                    PlayerMovement(true);
                });
            });
        }

        private void PlayerMovement(bool canMove)
        {
            if (_paddleP1 && _paddleP2)
            {
                _paddleP1.CanMove = canMove;
                _paddleP1.HasCollision = canMove;
                _paddleP2.CanMove = canMove;
                _paddleP2.HasCollision = canMove;
            }
            else
            {
                Debug.Log($"{GetType().Name}::MISSING PADDLE 1 or PADDLE 2::");
            }
        }

        #region Player Input Joined Events Calls
        public void PlayerInputManagerOnonPlayerJoined(PlayerInput obj)
        {
            if (!_paddleP1)
            {
                AudioManager.Instance.PlayOneShot("PlayerJoined");
                _paddleP1 = obj.gameObject.GetComponent<Paddle>();
                _paddleP1.gameObject.transform.position = _player1StartPos.position;
                _paddleP1.CanMove = true;
                _onPlayerJoined?.Invoke(_paddleP1, _colorPaddle1);
            }
            else
            {
                AudioManager.Instance.PlayOneShot("PlayerJoined");
                _paddleP2 = obj.gameObject.GetComponent<Paddle>();
                _paddleP2.gameObject.transform.position = _player2StartPos.position;
                _paddleP2.CanMove = true;
                _onPlayerJoined?.Invoke(_paddleP2, _colorPaddle2);
            }

            StartGame();
        }
        
        public void PlayerInputManagerOnonPlayerLeft(PlayerInput obj)
        {
            _isGameRunning = false;
            _isTimerRunning = false;
            ResetBall();
            _onPlayerLeft?.Invoke(obj.gameObject.GetComponent<Paddle>() == _paddleP1 ? _paddleP1 : _paddleP2);
        }
        #endregion

        #region Ball Code
        private void BallHitMainWall(Collision2D x, OnContactType y)
        {
            if (y == OnContactType.ENTER)
            {
                if (x.collider.CompareTag(Constants.Tags.BALL))
                {
                    AudioManager.Instance.PlayOneShot("BallHit");
                    _onBallHit?.Invoke(BallHit.WALL);
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
                    AudioManager.Instance.PlayOneShot("BallHit");
                    _onBallHit?.Invoke(BallHit.WALL);
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
                        AudioManager.Instance.PlaySounds("ScorePoint");
                        _onBallHit?.Invoke(BallHit.SCORE_TRIGGER);
                        if (_currentScorer == _paddleP1)
                        {
                            _scorePlayer1 += 1;
                        }
                        else
                        {
                            _scorePlayer2 += 1;
                        }
                        _onPlayerScoreChange?.Invoke(_currentScorer, _scorePlayer1, _scorePlayer2);
                    }
                    LaunchBall(null, true);
                }
            }
        }

        private void LaunchBall(Action afterBallLaunch = null, bool foregoCountdown = false)
        {
            if (foregoCountdown)
            {
                _leanBallReset = LeanTween.delayedCall(_ballLaunchDelay, () =>
                {
                    _ball.gameObject.transform.position = _ballRestartPoint.position;
                    afterBallLaunch?.Invoke();
                    _ball.Launch();
                });
            }
            else
            {
                _txtBallLaunchCountdown.gameObject.SetActive(true);
                _leanBallReset = LeanTween.value(_ballLaunchDelay+1, 0, 3f).setOnComplete(() =>
                {
                    _ball.gameObject.transform.position = _ballRestartPoint.position;
                    _txtBallLaunchCountdown.gameObject.SetActive(false);
                    afterBallLaunch?.Invoke();
                    _ball.Launch();
                }).setOnUpdate(x =>
                {
                    _txtBallLaunchCountdown.text = Mathf.FloorToInt(x).ToString();
                });
            }
        }

        private void ResetBall()
        {
            if (_leanBallReset != null)
                LeanTween.cancel(_leanBallReset.uniqueId);
            _ball.Stop();
            _ball.ResetLastPlayer(_ballDefaultColor);
        }
        #endregion

        #region IPause
        public void Pause()
        {
            PlayerMovement(false);
            if (_leanBallReset != null)
            {
                LeanTween.cancel(_leanBallReset.uniqueId);
                _txtBallLaunchCountdown.gameObject.SetActive(false);
            }
            
            if (_isGameRunning)
            {
                if (_isTimerRunning)
                {
                    _isTimerRunning = false;
                    ResetBall();
                }
            }
        }

        public void Resume()
        {
            if (_isGameRunning)
            {
                if (!_isTimerRunning)
                {
                    LaunchBall(() =>
                    {
                        _isTimerRunning = true;
                        PlayerMovement(true);
                    });
                }
            }
            else
            {
                StartGame();
            }
        }
        #endregion
    }
}
