using TMPro;
using UnityEngine;

namespace Pong.Managers
{
    public class GameOverScreen : MonoBehaviour, IGameManager
    {
        [SerializeField] private AnimationCurve _animationCurveScored;
        [SerializeField] private float _openAnimTime;
        [SerializeField] private Animator _animator;
        [SerializeField] private CanvasGroup _groupGameOver;
        [SerializeField] private TMP_Text _txtPlayerXWins;
        [SerializeField] private TMP_Text _txtPlayer1;
        [SerializeField] private TMP_Text _txtPlayer2;
        [SerializeField] private TMP_Text _txtPlayer1Score;
        [SerializeField] private TMP_Text _txtPlayer2Score;

        private readonly Vector3 _startingScale = new(.01f, .01f, .01f);
        private static readonly int _playAnim = Animator.StringToHash("playAnim");

        // Start is called before the first frame update
        private void Start()
        {
            InitScreen(0f);
        }
        
        /// <summary>
        /// Initializes Game over screen with default values, also closes the screen.
        /// </summary>
        /// <param name="animTime"></param>
        private void InitScreen(float animTime)
        {
            var groupGameObject = _groupGameOver.gameObject;
            LeanTween.alphaCanvas(_groupGameOver, -1f, animTime);
            LeanTween.scale(groupGameObject, _startingScale, animTime)
                .setEase(_animationCurveScored).setOnComplete(() =>
                {
                    _animator.SetBool(_playAnim, true);
                    SetText(Color.white, Color.white, 0, 0);
                    _animator.SetBool(_playAnim, false);
                    _groupGameOver.alpha = 0f;
                    _groupGameOver.gameObject.transform.localScale = _startingScale;
                    _groupGameOver.gameObject.SetActive(false);
                });
        }
        
        /// <summary>
        /// Opens Game Over screen
        /// </summary>
        /// <param name="colorP1">Player 1 color</param>
        /// <param name="colorP2">Player 2 color</param>
        /// <param name="scoreP1">Player 1 score</param>
        /// <param name="scoreP2">Player 2 score</param>
        private void PlayAnimation(Color colorP1, Color colorP2, int scoreP1, int scoreP2)
        {
            SetText(colorP1, colorP2, scoreP1, scoreP2);
            var groupGameObject = _groupGameOver.gameObject;
            groupGameObject.SetActive(true);
            LeanTween.alphaCanvas(_groupGameOver, 1f, _openAnimTime);
            LeanTween.scale(groupGameObject, new Vector3(1f, 1f, 1f), _openAnimTime)
                .setEase(_animationCurveScored).setOnComplete(() =>
                {
                    _animator.SetBool(_playAnim, true);
                });
        }

        /// <summary>
        /// Sets Game over Ui text
        /// </summary>
        /// <param name="colorP1">Player 1 color</param>
        /// <param name="colorP2">Player 2 color</param>
        /// <param name="scoreP1">Player 1 score</param>
        /// <param name="scoreP2">Player 2 score</param>
        private void SetText(Color colorP1, Color colorP2, int scoreP1, int scoreP2)
        {
            if (scoreP1 > scoreP2)
            {
                _txtPlayerXWins.text = "Player 1 Wins";
            }
            else if (scoreP1 < scoreP2)
            {
                _txtPlayerXWins.text = "Player 2 Wins";
            }
            else
            {
                _txtPlayerXWins.text = "It's a tie";
            }

            var c1 = colorP1;
            c1.a = 1;
            var c2 = colorP2;
            c2.a = 1;
            _txtPlayer1.color = c1;
            _txtPlayer2.color= c2;
            _txtPlayer1Score.text = scoreP1.ToString();
            _txtPlayer2Score.text = scoreP2.ToString();
        }
        
        #region IGameManager
        public void OnInit()
        {
            
        }

        public void OnPlayerScoreChange(Paddle paddleScorer, int newPlayer1Score, int newPlayer2Score)
        {
            
        }

        public void OnBallHit(BallHit ballHit)
        {
            
        }

        public void OnGameRetry(float timeTillGameStart)
        {
            InitScreen(timeTillGameStart);
        }

        public void OnGameEnd(Color colorP1, Color colorP2, int scoreP1, int scoreP2)
        {
            PlayAnimation(colorP1, colorP2, scoreP1, scoreP2);
        }

        public void OnPlayerJoined(Paddle paddle, Color color)
        {
            
        }

        public void OnPlayerLeft(Paddle paddle)
        {
            
        }
        #endregion
    }
}
