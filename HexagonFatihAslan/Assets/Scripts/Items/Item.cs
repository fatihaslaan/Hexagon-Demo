using UnityEngine;

public abstract class Item : MonoBehaviour
{
    float speed;

    void Update()
    {
        if (transform.localPosition.y <= 0)  //fall
        {
            transform.localPosition = Vector3.zero;
            return;
        }
        else
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - DropSpeed(), transform.localPosition.z);
    }

    float DropSpeed()   //speed of fall
    {
        speed = ((20f / (transform.parent.GetComponent<Cell>().CellY + 1)) + (20f / (transform.parent.GetComponent<Cell>().CellX + 1)));
        return speed;
    }

    public void SetColor(Color color)
    {
        gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    public Color GetColor()
    {
        return gameObject.GetComponent<SpriteRenderer>().color;
    }

    public virtual int Score()
    {
        return 0;
    }
}