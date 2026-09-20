using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PocketFront
{
    public enum Team { Player, Enemy }

    public enum GameState { PlayerTurn, EnemyTurn, Victory, Defeat }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public GridManager grid;
        public TurnManager turns;
        public EnemyAI enemyAI;
        public PlayerController playerController;

        public List<Unit> playerUnits = new List<Unit>();
        public List<Unit> enemyUnits = new List<Unit>();
        public BaseController playerBase;
        public BaseController enemyBase;

        public GameState state = GameState.PlayerTurn;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            grid.Init();

            for (int i = 0; i < playerUnits.Count; i++)
            {
                playerUnits[i].Init(grid);
            }
            for (int i = 0; i < enemyUnits.Count; i++)
            {
                enemyUnits[i].Init(grid);
            }

            playerBase.Init(grid);
            enemyBase.Init(grid);

            turns.StartPlayerTurn();
        }

        public bool IsGameOver()
        {
            return state == GameState.Victory || state == GameState.Defeat;
        }

        public List<Unit> GetLivingUnits(Team team)
        {
            List<Unit> all = playerUnits;
            if (team == Team.Enemy)
            {
                all = enemyUnits;
            }

            List<Unit> alive = new List<Unit>();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i] != null && all[i].IsAlive())
                {
                    alive.Add(all[i]);
                }
            }
            return alive;
        }

        public void CheckGameOver()
        {
            if (IsGameOver())
            {
                return;
            }

            if (!enemyBase.IsAlive() || GetLivingUnits(Team.Enemy).Count == 0)
            {
                state = GameState.Victory;
                playerController.Deselect();
                grid.ClearHighlights();
                return;
            }

            if (!playerBase.IsAlive() || GetLivingUnits(Team.Player).Count == 0)
            {
                state = GameState.Defeat;
                playerController.Deselect();
                grid.ClearHighlights();
            }
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
