using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    Transform _playerRoom, _opponentRoom;
    [SerializeField]
    TextMeshPro _textPlaceHolder;

    int _index;
    Tile _nextInThePath;

    public int Index => _index;
    public Transform PlayerRoom => _playerRoom;
    public Transform OpponentRoom => _opponentRoom;
    public Tile NextTile => _nextInThePath;


    public void CreateIndex(int x, int y, int sizeX)
    {
        if ((y & 1) == 0)
        {
            _index = y * sizeX + 1 + x;
        }
        else
        {
            _index = y * sizeX + sizeX - x;
        }
    }

    public void SetNextInPath(Tile tile)
    {
        _nextInThePath = tile;
    }

    public void SetText(string text)
    {
        _textPlaceHolder.text = text;
    }

}
