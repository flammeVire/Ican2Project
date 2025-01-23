using UnityEngine;

[CreateAssetMenu(fileName = "Fire", menuName = "ScriptableObjects/FireExpansion", order = 1)]
public class FireExpansion: ScriptableObject
{
    public direction nextDirectionWanted;
    public direction[] nextDirectionOrder = new direction[4];
    public bool[] haveAlreadyExpand = new bool[4];
    public Vector3Int Position;
    public enum direction
    {
        North,South,West,Est
    }
}
