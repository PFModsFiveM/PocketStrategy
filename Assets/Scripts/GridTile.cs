using UnityEngine;

namespace PocketFront
{
    public enum TileHighlight { Normal, Selected, ValidMove, ValidAttack }

    public class GridTile : MonoBehaviour
    {
        public int x;
        public int y;
        public bool isBlocked;

        public Unit occupant;
        public BaseController baseOnTile;

        GridManager grid;
        Renderer tileRenderer;

        public void Init(GridManager gridManager)
        {
            grid = gridManager;
            tileRenderer = GetComponent<Renderer>();
            occupant = null;
            baseOnTile = null;
            SetHighlight(TileHighlight.Normal);
        }

        public bool IsFree()
        {
            if (isBlocked)
            {
                return false;
            }
            if (baseOnTile != null)
            {
                return false;
            }
            if (occupant != null && occupant.IsAlive())
            {
                return false;
            }
            return true;
        }

        public void SetHighlight(TileHighlight state)
        {
            if (tileRenderer == null)
            {
                return;
            }

            if (state == TileHighlight.Selected)
            {
                tileRenderer.material.color = grid.selectedColor;
            }
            else if (state == TileHighlight.ValidMove)
            {
                tileRenderer.material.color = grid.moveColor;
            }
            else if (state == TileHighlight.ValidAttack)
            {
                tileRenderer.material.color = grid.attackColor;
            }
            else
            {
                tileRenderer.material.color = grid.normalColor;
            }
        }
    }
}
