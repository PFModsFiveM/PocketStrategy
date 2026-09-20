using UnityEngine;

namespace PocketFront
{
    public class BaseController : MonoBehaviour
    {
        public Team team = Team.Player;
        public int maxHP = 5;

        public int gridX;
        public int gridY;

        public int currentHP;

        GridTile tile;

        public void Init(GridManager grid)
        {
            currentHP = maxHP;

            tile = grid.GetTile(gridX, gridY);
            if (tile == null)
            {
                Debug.LogWarning(name + " is not on the board.");
                return;
            }

            tile.baseOnTile = this;
            transform.position = grid.GetWorldPosition(gridX, gridY);
        }

        public bool IsAlive()
        {
            return currentHP > 0;
        }

        public GridTile GetTile()
        {
            return tile;
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
                gameObject.SetActive(false);
            }
        }
    }
}
