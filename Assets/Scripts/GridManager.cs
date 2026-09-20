using System.Collections.Generic;
using UnityEngine;

namespace PocketFront
{
    public class GridManager : MonoBehaviour
    {
        public int width = 5;
        public int height = 5;
        public float tileSize = 1.1f;
        public float tileThickness = 0.2f;

        public List<Vector2Int> blockedCells = new List<Vector2Int>
        {
            new Vector2Int(1, 2), new Vector2Int(3, 2), new Vector2Int(4, 1), new Vector2Int(0, 3)
        };

        public Material tileMaterial;
        public Material obstacleMaterial;

        public Color normalColor = Color.gray;
        public Color selectedColor = new Color(1f, 0.85f, 0.2f);
        public Color moveColor = new Color(0.35f, 0.75f, 1f);
        public Color attackColor = new Color(1f, 0.3f, 0.25f);

        GridTile[,] tiles;

        public void BuildBoard()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    go.name = "Tile " + x + "," + y;
                    go.transform.SetParent(transform, false);
                    go.transform.localPosition = new Vector3(TileOffset(x, width), 0f, TileOffset(y, height));
                    go.transform.localScale = new Vector3(tileSize * 0.94f, tileThickness, tileSize * 0.94f);

                    if (tileMaterial != null)
                    {
                        go.GetComponent<Renderer>().sharedMaterial = tileMaterial;
                    }

                    GridTile tile = go.AddComponent<GridTile>();
                    tile.x = x;
                    tile.y = y;
                    tile.isBlocked = blockedCells.Contains(new Vector2Int(x, y));

                    if (tile.isBlocked)
                    {
                        CreateObstacle(go.transform);
                    }
                }
            }
        }

        void CreateObstacle(Transform tile)
        {
            float rockHeight = 0.6f;
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rock.name = "Obstacle";
            rock.transform.SetParent(tile, false);
            rock.transform.localScale = new Vector3(0.7f, rockHeight / tileThickness, 0.7f);
            rock.transform.localPosition = new Vector3(0f, 0.5f + (rockHeight / tileThickness) * 0.5f, 0f);

            if (obstacleMaterial != null)
            {
                rock.GetComponent<Renderer>().sharedMaterial = obstacleMaterial;
            }
        }

        public void Init()
        {
            tiles = new GridTile[width, height];

            GridTile[] found = GetComponentsInChildren<GridTile>();
            for (int i = 0; i < found.Length; i++)
            {
                GridTile tile = found[i];
                if (IsInside(tile.x, tile.y))
                {
                    tiles[tile.x, tile.y] = tile;
                }
                tile.Init(this);
            }
        }

        public bool IsInside(int x, int y)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }

        public GridTile GetTile(int x, int y)
        {
            if (tiles == null || !IsInside(x, y))
            {
                return null;
            }
            return tiles[x, y];
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(
                transform.position.x + TileOffset(x, width),
                transform.position.y + tileThickness * 0.5f,
                transform.position.z + TileOffset(y, height));
        }

        float TileOffset(int index, int count)
        {
            return (index - (count - 1) * 0.5f) * tileSize;
        }

        public static int Distance(GridTile a, GridTile b)
        {
            if (a == null || b == null)
            {
                return 999;
            }
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        public void ClearHighlights()
        {
            if (tiles == null)
            {
                return;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (tiles[x, y] != null)
                    {
                        tiles[x, y].SetHighlight(TileHighlight.Normal);
                    }
                }
            }
        }
    }
}
