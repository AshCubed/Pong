using UnityEngine;

namespace Pong
{
    public interface IGameManager
    {
        public void OnInit();
        
        public void OnPlayerScoreChange(Paddle paddleScorer, int newPlayer1Score, int newPlayer2Score);
        
        public void OnBallHit(BallHit ballHit);

        public void OnGameRetry(float timeTillGameStart);

        public void OnGameEnd(Color colorP1, Color colorP2, int scoreP1, int scoreP2);

        public void OnPlayerJoined(Paddle paddle, Color color);

        public void OnPlayerLeft(Paddle paddle);
    }
}
