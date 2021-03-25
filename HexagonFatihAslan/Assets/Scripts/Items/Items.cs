using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    [SerializeField]
    GameObject Hexagon, Bomb;

    public GameObject GetHexagon()
    {
        GameObject hex=Hexagon;
        return hex;
    }

}
