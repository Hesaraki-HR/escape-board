
using UnityEngine;

[CreateAssetMenu(fileName = "Portal", menuName = "ScriptableObjects/PortalObject", order = 1)]
public class PortalObject : ScriptableObject {
    public int SourceIndex = default;
    public int DestinationIndex = default;
}