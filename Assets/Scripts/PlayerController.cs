using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace PocketFront
{
    public class PlayerController : MonoBehaviour
    {
        public Camera gameCamera;

        public Unit selected;

        List<GridTile> moveTiles = new List<GridTile>();
        List<GridTile> attackTiles = new List<GridTile>();

        void Update()
        {
            GameManager game = GameManager.Instance;
            if (game == null || game.state != GameState.PlayerTurn)
            {
                return;
            }
            if (gameCamera == null)
            {
                gameCamera = Camera.main;
            }
            if (gameCamera == null || Mouse.current == null)
            {
                return;
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Deselect();
                return;
            }
            if (!Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Ray ray = gameCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500f))
            {
                HandleClick(hit.collider);
            }
            else
            {
                Deselect();
            }
        }

        void HandleClick(Collider hitCollider)
        {
            GridTile tile = hitCollider.GetComponentInParent<GridTile>();
            Unit unit = hitCollider.GetComponentInParent<Unit>();
            BaseController clickedBase = hitCollider.GetComponentInParent<BaseController>();

            if (tile == null && unit != null)
            {
                tile = unit.GetTile();
            }
            if (tile == null && clickedBase != null)
            {
                tile = clickedBase.GetTile();
            }
            if (tile == null)
            {
                Deselect();
                return;
            }

            if (unit == null)
            {
                unit = tile.occupant;
            }
            if (clickedBase == null)
            {
                clickedBase = tile.baseOnTile;
            }
            if (unit != null && !unit.IsAlive())
            {
                unit = null;
            }

            if (unit != null && unit.team == Team.Player)
            {
                Select(unit);
                return;
            }

            if (selected != null && attackTiles.Contains(tile))
            {
                if (unit != null)
                {
                    selected.Attack(unit);
                }
                else if (clickedBase != null)
                {
                    selected.AttackBase(clickedBase);
                }
                Deselect();
                GameManager.Instance.CheckGameOver();
                return;
            }

            if (selected != null && moveTiles.Contains(tile))
            {
                selected.MoveTo(tile);
                Deselect();
                return;
            }

            Deselect();
        }

        void Select(Unit unit)
        {
            Deselect();
            selected = unit;
            unit.GetTile().SetHighlight(TileHighlight.Selected);

            if (!unit.CanAct())
            {
                return;
            }

            moveTiles = unit.GetMoveTiles();
            attackTiles = unit.GetAttackTiles();

            for (int i = 0; i < moveTiles.Count; i++)
            {
                moveTiles[i].SetHighlight(TileHighlight.ValidMove);
            }
            for (int i = 0; i < attackTiles.Count; i++)
            {
                attackTiles[i].SetHighlight(TileHighlight.ValidAttack);
            }
        }

        public void Deselect()
        {
            selected = null;
            moveTiles.Clear();
            attackTiles.Clear();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.grid.ClearHighlights();
            }
        }
    }
}
