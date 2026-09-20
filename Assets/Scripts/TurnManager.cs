using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PocketFront
{
    public class TurnManager : MonoBehaviour
    {
        public int turnNumber = 0;

        public void StartPlayerTurn()
        {
            GameManager game = GameManager.Instance;
            if (game.IsGameOver())
            {
                return;
            }

            turnNumber = turnNumber + 1;
            game.state = GameState.PlayerTurn;
            ResetUnits(Team.Player);
            ResetUnits(Team.Enemy);
        }

        public void EndPlayerTurn()
        {
            GameManager game = GameManager.Instance;
            if (game.state != GameState.PlayerTurn)
            {
                return;
            }

            game.playerController.Deselect();
            StartCoroutine(EnemyTurn());
        }

        IEnumerator EnemyTurn()
        {
            GameManager game = GameManager.Instance;
            game.state = GameState.EnemyTurn;

            yield return game.enemyAI.TakeTurn();

            if (!game.IsGameOver())
            {
                StartPlayerTurn();
            }
        }

        void ResetUnits(Team team)
        {
            GameManager game = GameManager.Instance;
            List<Unit> units = game.GetLivingUnits(team);
            for (int i = 0; i < units.Count; i++)
            {
                units[i].SetActed(false);
            }
        }
    }
}
