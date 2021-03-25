using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField]
    const int Width = 8; //I didnt use hexagon tilemap, because when you TileMap.SetTile() and assign a gameobject to that tile,
    [SerializeField]     //all of tiles using same gameobject and also same cell script (same values in all of them).
    const int Height = 9;
    [SerializeField]
    GameObject CellObject;

    GameObject TempCell;
    Cell[,] Cells;

    void Start()
    {
        Cells = new Cell[Width, Height];
        AssignCells();
        SetupCells();
    }

    void AssignCells()
    {
        for (int h = -Height / 2; h < Height - Height / 2; h++)
        {
            for (int w = -Width / 2; w < Width - Width / 2; w++)
            {
                TempCell = Instantiate(CellObject, new Vector3((float)w * 0.75f, (float)h * 0.88f, 0), Quaternion.identity);
                if (w % 2 == 0)
                    TempCell.transform.position = new Vector3(TempCell.transform.position.x, TempCell.transform.position.y + 0.44f, 0);
                TempCell.transform.SetParent(transform);
                Cells[w + Width / 2, h + Height / 2] = TempCell.GetComponent<Cell>();   // [X,Y]
            }
        }

    }

    void SetupCells()
    {
        for (int h = 0; h < Height; h++)
        {
            for (int w = 0; w < Width; w++)
            {
                Cells[w , h].Setup(w , h );
            }
        }
    }

    List<Cell> GetNeighborCells(int cellx, int celly)
    {
        List<Cell> CellNeighbor = new List<Cell>();
        int[,] Direction = Global.IsCellEven(cellx);

        for (int i = 0; i < 6; i++)
            if (cellx + Direction[i, 0] < Width && cellx + Direction[i, 0] >= 0 && celly + Direction[i, 1] < Height && celly + Direction[i, 1] >= 0)
                CellNeighbor.Add(Cells[cellx + Direction[i, 0], celly + Direction[i, 1]]);

        return CellNeighbor;
    }

    public bool CanUseColor(int cellx, int celly, Color c)
    {
        List<Cell> Neighbors = GetNeighborCells(cellx, celly);
        for (int i = 0; i < Neighbors.Count; i++)
        {
            if (Neighbors[i].CurrentItemColor == c && Neighbors[(i + 1) % Neighbors.Count].CurrentItemColor == c)
                return false;
        }
        return true;
    }
}
