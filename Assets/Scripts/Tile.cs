using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    int _index;
    Tile _nextInThePath;

    public int Index => _index;
    public Vector3 PlayerRoom => new Vector3(transform.position.x, 0f, transform.position.z - transform.localScale.z / 2);
    public Vector3 OpponentRoom => new Vector3(transform.position.x, 0f, transform.position.z + transform.localScale.z / 2);
    public Tile NextTile => _nextInThePath;

    TextMeshPro _textPlaceHolder;

    private void Awake()
    {
        _textPlaceHolder = transform.FindInChildrenRecursive("Text").GetComponent<TextMeshPro>();
    }

    public void CreateIndex(int x, int y, int sizeX, int sizeY)
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
