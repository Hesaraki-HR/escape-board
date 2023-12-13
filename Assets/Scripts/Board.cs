using UnityEngine;
using System.Linq;
using System.Collections.Generic;


public class Board : MonoBehaviour
{
    [SerializeField]
    Vector2Int _boardSize = new Vector2Int(10, 10);

    [SerializeField]
    Tile tilePrefab = default;

    [SerializeField]
    List<PortalObject> _portals;

    Tile[] _tiles;


    public static Board Instance { get; private set; }
    public int MaxIndex => _tiles.Count();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    public void Initialize()
    {

        _tiles = new Tile[_boardSize.x * _boardSize.y];
        for (int y = 0, i = 0; y < _boardSize.y; y++)
        {
            for (int x = 0; x < _boardSize.x; x++, i++)
            {
                Tile tile = Instantiate(tilePrefab);
                tile.transform.SetParent(transform);
                tile.transform.localPosition = new Vector3(x, 0f, y);
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


}
