using UnityEngine;
using TMPro;
using System.Linq;

public class Board : MonoBehaviour
{
    [SerializeField]
    Vector2Int _boardSize = new Vector2Int(10, 10);

    [SerializeField]
    Tile tilePrefab = default;

    [SerializeField]
    Transform _tilesRoot;

    Tile[] _tiles;


    public static Board Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    public void Initialize()
    {
        Vector2 offset = new Vector2((_boardSize.x - 1) * 0.5f, (_boardSize.y - 1) * 0.5f);
        _tiles = new Tile[_boardSize.x * _boardSize.y];
        for (int y = 0, i = 0; y < _boardSize.y; y++)
        {
            for (int x = 0; x < _boardSize.x; x++, i++)
            {
                Tile tile = Instantiate(tilePrefab);
                tile.transform.SetParent(_tilesRoot, false);
                tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);
                tile.CreateIndex(x, y, _boardSize.x);
                tile.SetText(tile.Index.ToString());
                _tiles[i] = tile;
            }
        }

        for (int i = 1; i < _boardSize.x * _boardSize.y; i++)
        {
            var currentTile = _tiles.First(x => x.Index == i);
            var nextTile = _tiles.First(x => x.Index == i + 1);
            currentTile.SetNextInPath(nextTile);
        }

    }

    public Tile GetTile(int tileNumber)
    {
        return _tiles.First(x => x.Index == tileNumber);
    }

    public int MaxIndex => _tiles.Count();
}
