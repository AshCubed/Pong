using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pong.Managers
{
    public class UiController : MonoBehaviour, IGameManager
    {
        [Header("Ui - Main")]
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

        private Paddle _player1;
        private Paddle _player2;

        // Start is called before the first frame update
        private void Start()
        {
            _joinPlayer1.SetActive(true);
            _joinPlayer2.SetActive(false);
            OnInit();
        }

        /// <summary>
        /// Animates a "Player X has scored" ui element
        /// </summary>
        /// <param name="canvasGroup">Canvas group of the player who scored</param>
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
        
        #region IGameManager
        public void OnInit()
        {
            _groupPlayer1Scored.alpha = 0;
            _groupPlayer2Scored.alpha = 0;
        }

        public void OnPlayerScoreChange(Paddle paddleScorer, int newPlayer1Score, int newPlayer2Score)
        {
            _txtPlayer1Score.text = newPlayer1Score.ToString(CultureInfo.InvariantCulture);
            _txtPlayer2Score.text = newPlayer2Score.ToString(CultureInfo.InvariantCulture);
            if (paddleScorer != null)
                AnimationScore(paddleScorer == _player1 ?  _groupPlayer1Scored : _groupPlayer2Scored);
        }

        public void OnBallHit(BallHit ballHit)
        {
            
        }

        public void OnGameRetry(float timeTillGameStart)
        {
            
        }

        public void OnGameEnd(Color colorP1, Color colorP2, int scoreP1, int scoreP2)
        {
            
        }
        
        public void OnPlayerJoined(Paddle paddle, Color color)
        {
            if (!_player1)
            {
                _player1 = paddle;
                _player1.PaddleColor = color;
                _imagePlayer1Scored.color = color;
                _joinPlayer1.SetActive(false);
                _joinPlayer2.SetActive(true);
            }
            else
            {
                _player2 = paddle;
                _player2.PaddleColor = color;
                _imagePlayer2Scored.color = color;
                _joinPlayer2.SetActive(false);
            }
        }
        
        public void OnPlayerLeft(Paddle paddle)
        {
            if (paddle == _player1)
            {
                _joinPlayer1.SetActive(true);
            }
            else
            {
                _joinPlayer2.SetActive(true);
            }
        }
        #endregion
    }
}
