using UnityEngine;

public class Items : MonoBehaviour
{
    [SerializeField]
    GameObject Hexagon, Bomb;

    public GameObject GetHexagon()
    {
        GameObject hex = Hexagon;
        return hex;
    }

    public GameObject GetBomb()
    {
        GameObject bomb = Bomb;
        return bomb;
    }

    public float DropHeight()
    {
        return Screen.height / 35;  //fall height of spawned objects;
    }
}