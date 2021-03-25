using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public void SetColor(Color color)
    {
        gameObject.GetComponent<SpriteRenderer>().color=color;
    }
}
