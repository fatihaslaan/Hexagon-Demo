using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cell : MonoBehaviour
{
    public Color CurrentItemColor;
    GameObject Parent;
    float CorX, CorY;
    int CellX, CellY;
    GameObject CurrentItem;

    Board GameBoard;
    Items AllItems;
    Colors AllColors;

    void OnMouseUpAsButton()
    {

    }

    void SetItem()
    {
        CurrentItem = Instantiate(AllItems.GetHexagon(), new Vector3(CorX, CorY, 0), Quaternion.identity);
        CurrentItem.transform.SetParent(transform);
        do
        {
            CurrentItemColor=AllColors.ColorList[Random.Range(0,AllColors.ColorList.Count)];
        }while(!GameBoard.CanUseColor(CellX,CellY,CurrentItemColor));
        CurrentItem.GetComponent<Hexagon>().SetColor(CurrentItemColor);
    }

    public void Setup(int cellx, int celly)
    {
        CorX = transform.position.x;
        CorY = transform.position.y;
        CellX = cellx;
        CellY = celly;
        Parent = transform.parent.gameObject;
        AllItems = Parent.GetComponent<Items>();
        AllColors = Parent.GetComponent<Colors>();
        GameBoard = Parent.GetComponent<Board>();
        SetItem();
    }
}
