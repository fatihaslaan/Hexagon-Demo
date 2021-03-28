using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Color CurrentItemColor;
    public GameObject CurrentItem;

    public List<List<Cell>> NeighBorGroupCells; //neighbor groups
    public Cell UpperCell;

    float CorX, CorY;   //cell coordinates
    public int CellX, CellY;   //cell grid coordinates

    GameObject Parent;

    Board GameBoard;    //for getting board
    Items AllItems;     //for getting items
    Colors AllColors;   //for getting colors

    void CellSetup(int cellx, int celly)
    {
        CorX = transform.position.x;
        CorY = transform.position.y;
        CellX = cellx;
        CellY = celly;
        Parent = transform.parent.gameObject;
        AllItems = Parent.GetComponent<Items>();
        AllColors = Parent.GetComponent<Colors>();
        GameBoard = Parent.GetComponent<Board>();
    }

    void ItemSetUp()
    {
        GameObject SpawnObj=AllItems.GetHexagon();
        if (Global.GameScore / (GameBoard.SpawnBombLimit * (1 + Global.TotalBombsSpawned))>0)
        {
            SpawnObj = AllItems.GetBomb();
            Global.TotalBombsSpawned++;
        }
        int colorcounter = 0;
        CurrentItem = Instantiate(SpawnObj, new Vector3(CorX, CorY + AllItems.DropHeight(), 0), Quaternion.identity);  //start with hexagons
        //CurrentItem.transform.localScale = transform.localScale;  //change items scale to ownercells scale, can modify cell size without any problem with this line (not working due to bomb item right now), also move this line to setitem to be able to make diffirent sized cells
        CurrentItem.transform.SetParent(transform);
        do
        {
            /*TEST
            int a=Random.Range(0,2);
            if(a==0)
                CurrentItemColor=Color.red;
            else if(a==1)
                CurrentItemColor=Color.green;
            else
                CurrentItemColor=Color.blue;
            */
            if (colorcounter >= 1000)
            {
                Debug.Log("Not Enough Color");
                return;
            }
            colorcounter++;
            CurrentItemColor = AllColors.ColorList[Random.Range(0, AllColors.ColorList.Count)];
        } while (!CanUseColor());
        CurrentItem.GetComponent<Item>().SetColor(CurrentItemColor);
    }

    bool CanUseColor()  //check if cell can use that color to spawn an object
    {
        foreach (List<Cell> group in NeighBorGroupCells)
        {
            int groupcount = 0;
            foreach (Cell cell in group)
            {
                if (cell.CurrentItemColor != CurrentItemColor)
                    break;
                groupcount++;
                if (groupcount == group.Count)
                    return false;
            }
        }
        return true;
    }

    void Update()   //getting upper cells item
    {
        while (!CurrentItem)
        {
            if (UpperCell)
                if (UpperCell.CurrentItem)
                {
                    SetItem(UpperCell.CurrentItem,true);
                    UpperCell.CurrentItem = null;
                }
                else
                    return;
            else
                ItemSetUp();
        }
    }

    public void Setup(int cellx, int celly)
    {
        CellSetup(cellx, celly);
        ItemSetUp();
    }

    public void SetItem(GameObject Item,bool fall)  //set new item while rotating and falling
    {
        CurrentItem = Item;
        CurrentItemColor = CurrentItem.GetComponent<Item>().GetColor();
        CurrentItem.transform.SetParent(transform);
        if(!fall)
            CurrentItem.transform.localPosition = Vector3.zero;
    }

    public void Pressed(float x, float y)
    {
        GameBoard.CellPressed(CellX, CellY, x, y);
    }

    public void DestroyCurrentItem()
    {
        if (CurrentItem)
        {
            Global.GameScore += CurrentItem.GetComponent<Item>().Score();
            CurrentItemColor = Color.clear;
            Destroy(CurrentItem);
        }
    }
}