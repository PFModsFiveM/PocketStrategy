using System.Collections.Generic;
using UnityEngine;

namespace PocketFront
{
    public class Unit : MonoBehaviour
    {
        public string unitName = "Soldier";
        public Team team = Team.Player;
        public int maxHP = 3;
        public int damage = 1;
        public int moveRange = 1;
        public int attackRange = 1;
        public Color teamColor = Color.blue;

        public int gridX;
        public int gridY;

        public int currentHP;
        public bool hasActed;

        GridManager grid;
        GridTile tile;

        public void Init(GridManager gridManager)
        {
            grid = gridManager;
            currentHP = maxHP;
            hasActed = false;

            tile = grid.GetTile(gridX, gridY);
            if (tile == null)
            {
                Debug.LogWarning(unitName + " has a start tile that is not on the board.");
                gameObject.SetActive(false);
                return;
            }

            tile.occupant = this;
            transform.position = grid.GetWorldPosition(gridX, gridY);
            SetColor(teamColor);
        }

        public bool IsAlive()
        {
            return currentHP > 0;
        }

        public bool CanAct()
        {
            return IsAlive() && !hasActed;
        }

        public GridTile GetTile()
        {
            return tile;
        }

        public List<GridTile> GetMoveTiles()
        {
            List<GridTile> result = new List<GridTile>();
            if (!CanAct())
            {
                return result;
            }

            AddMoveLine(result, 1, 0);
            AddMoveLine(result, -1, 0);
            AddMoveLine(result, 0, 1);
            AddMoveLine(result, 0, -1);
            return result;
        }

        void AddMoveLine(List<GridTile> list, int stepX, int stepY)
        {
            for (int step = 1; step <= moveRange; step++)
            {
                GridTile next = grid.GetTile(gridX + stepX * step, gridY + stepY * step);
                if (next == null || !next.IsFree())
                {
                    return;
                }
                list.Add(next);
            }
        }

        public List<GridTile> GetAttackTiles()
        {
            List<GridTile> result = new List<GridTile>();
            if (!CanAct())
            {
                return result;
            }

            AddAttackLine(result, 1, 0);
            AddAttackLine(result, -1, 0);
            AddAttackLine(result, 0, 1);
            AddAttackLine(result, 0, -1);
            return result;
        }

        void AddAttackLine(List<GridTile> list, int stepX, int stepY)
        {
            for (int step = 1; step <= attackRange; step++)
            {
                GridTile next = grid.GetTile(gridX + stepX * step, gridY + stepY * step);
                if (next == null || next.isBlocked)
                {
                    return;
                }

                if (next.occupant != null && next.occupant.IsAlive())
                {
                    if (next.occupant.team != team)
                    {
                        list.Add(next);
                    }
                    return;
                }

                if (next.baseOnTile != null)
                {
                    if (next.baseOnTile.team != team && next.baseOnTile.IsAlive())
                    {
                        list.Add(next);
                    }
                    return;
                }
            }
        }

        public void MoveTo(GridTile target)
        {
            if (!CanAct() || target == null || !target.IsFree())
            {
                return;
            }

            tile.occupant = null;
            tile = target;
            tile.occupant = this;
            gridX = target.x;
            gridY = target.y;
            transform.position = grid.GetWorldPosition(gridX, gridY);
            SetActed(true);
        }

        public void Attack(Unit target)
        {
            if (!CanAct() || target == null || !target.IsAlive())
            {
                return;
            }
            if (GridManager.Distance(tile, target.GetTile()) > attackRange)
            {
                return;
            }

            target.TakeDamage(damage);
            SetActed(true);
        }

        public void AttackBase(BaseController target)
        {
            if (!CanAct() || target == null || !target.IsAlive())
            {
                return;
            }
            if (GridManager.Distance(tile, target.GetTile()) > attackRange)
            {
                return;
            }

            target.TakeDamage(damage);
            SetActed(true);
        }

        public void TakeDamage(int amount)
        {
            if (!IsAlive())
            {
                return;
            }

            currentHP = currentHP - amount;
            if (currentHP <= 0)
            {
                currentHP = 0;
                Die();
            }
        }

        void Die()
        {
            if (tile != null && tile.occupant == this)
            {
                tile.occupant = null;
            }
            gameObject.SetActive(false);
        }

        public void SetActed(bool value)
        {
            hasActed = value;
            if (value)
            {
                SetColor(teamColor * 0.45f);
            }
            else
            {
                SetColor(teamColor);
            }
        }

        void SetColor(Color color)
        {
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = color;
            }
        }
    }
}
