
using UnityEngine;

public enum PortalTypes
{
    Forward,
    Backward
}

[CreateAssetMenu(fileName = "Portal", menuName = "ScriptableObjects/PortalObject", order = 1)]
public class PortalObject : ScriptableObject
{
    public PortalTypes PortalType = PortalTypes.Forward;
    public GameObject PortalPrefab = default;
    public int SourceIndex = default;
    public int DestinationIndex = default;
}