using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PocketFront
{
    public class EnemyAI : MonoBehaviour
    {
        public float actionDelay = 0.45f;

        public IEnumerator TakeTurn()
        {
            GameManager game = GameManager.Instance;
            yield return new WaitForSeconds(actionDelay);

            List<Unit> enemies = game.GetLivingUnits(Team.Enemy);
            for (int i = 0; i < enemies.Count; i++)
            {
                Unit enemy = enemies[i];
                if (game.IsGameOver())
                {
                    break;
                }
                if (!enemy.IsAlive())
                {
                    continue;
                }

                DoAction(enemy);
                game.CheckGameOver();
                yield return new WaitForSeconds(actionDelay);
            }
        }

        void DoAction(Unit enemy)
        {
            List<GridTile> targets = enemy.GetAttackTiles();
            GridTile bestTarget = null;

            for (int i = 0; i < targets.Count; i++)
            {
                GridTile target = targets[i];
                if (target.occupant == null)
                {
                    continue;
                }
                if (bestTarget == null || bestTarget.occupant == null || target.occupant.currentHP < bestTarget.occupant.currentHP)
                {
                    bestTarget = target;
                }
            }

            if (bestTarget == null && targets.Count > 0)
            {
                bestTarget = targets[0];
            }

            if (bestTarget != null)
            {
                if (bestTarget.occupant != null)
                {
                    enemy.Attack(bestTarget.occupant);
                }
                else
                {
                    enemy.AttackBase(bestTarget.baseOnTile);
                }
                return;
            }

            GridTile goal = FindGoal(enemy);
            if (goal != null)
            {
                MoveToward(enemy, goal);
            }
        }

        GridTile FindGoal(Unit enemy)
        {
            GameManager game = GameManager.Instance;
            List<Unit> players = game.GetLivingUnits(Team.Player);

            GridTile goal = null;
            int shortest = 999;

            for (int i = 0; i < players.Count; i++)
            {
                int distance = GridManager.Distance(enemy.GetTile(), players[i].GetTile());
                if (distance < shortest)
                {
                    shortest = distance;
                    goal = players[i].GetTile();
                }
            }

            if (goal == null && game.playerBase.IsAlive())
            {
                goal = game.playerBase.GetTile();
            }
            return goal;
        }

        void MoveToward(Unit enemy, GridTile goal)
        {
            List<GridTile> moves = enemy.GetMoveTiles();
            GridTile bestMove = null;
            int shortest = GridManager.Distance(enemy.GetTile(), goal);

            for (int i = 0; i < moves.Count; i++)
            {
                int distance = GridManager.Distance(moves[i], goal);
                if (distance < shortest)
                {
                    shortest = distance;
                    bestMove = moves[i];
                }
            }

            if (bestMove != null)
            {
                enemy.MoveTo(bestMove);
            }
        }
    }
}
