using UnityEngine;

namespace Pong.Managers
{
    public interface IGameManager
    {
        /// <summary>
        /// Subscribes to an event in Game Manager, on game Initializing this will also be called
        /// </summary>
        public void OnInit();
        
        /// <summary>
        /// Subscribes to an event in Game Manager, when a player scores this will also be called
        /// </summary>
        public void OnPlayerScoreChange(Paddle paddleScorer, int newPlayer1Score, int newPlayer2Score);
        
        /// <summary>
        /// Subscribes to an event in Game Manager, when the ball hits something, this will also be called
        /// </summary>
        public void OnBallHit(BallHit ballHit);

        /// <summary>
        /// Subscribes to an event in Game Manager, within the Retry function this will also be called
        /// </summary>
        public void OnGameRetry(float timeTillGameStart);

        /// <summary>
        /// Subscribes to an event in Game Manager, when the game end this will also be called
        /// </summary>
        public void OnGameEnd(Color colorP1, Color colorP2, int scoreP1, int scoreP2);

        /// <summary>
        /// Subscribes to an event in Game Manager, when a new player joins this will also be called
        /// </summary>
        public void OnPlayerJoined(Paddle paddle, Color color);

        /// <summary>
        /// Subscribes to an event in Game Manager, when a new player leaves this will also be called
        /// </summary>
        public void OnPlayerLeft(Paddle paddle);
    }
}
