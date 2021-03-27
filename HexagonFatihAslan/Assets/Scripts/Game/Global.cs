using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    public static bool GameOver=false;

    public static int GameScore=0;
    public static int Move=0;
    public static bool BombSpawned=false;
    public static int TotalBombsSpawned=0;

    public static float SelectedGroupObjectx;
    public static bool Slide,RotateClockWise;
    
    static int[,] IsCellEven(int x,int BoardWidth) //to determine if next cell upright or downright
    {
        if(x%2==0)
            return new int[6,2] {{0,1},{1,1},{1,0},{0,-1},{-1,0},{-1,1}};
        else
            return new int[6,2] {{0,1},{1,0},{1,-1},{0,-1},{-1,-1},{-1,0}};
    }

    static bool IsInsideGrid(int i,int cellx, int celly,int BoardWidth, int BoardHeight,int[,] Direction)   //is inside grid
    {
        
        if(cellx + Direction[i % Direction.GetLength(0), 0] < BoardWidth && cellx + Direction[i % Direction.GetLength(0), 0] >= 0 && celly + Direction[i % Direction.GetLength(0), 1] < BoardHeight && celly + Direction[i % Direction.GetLength(0), 1] >= 0)
            return true;
        return false;
    }

    public static List<List<Cell>> GetNeighborGroupCells(int cellx, int celly,int BoardWidth, int BoardHeight,Cell[,] Cells)    //get neighbor duos of a cell
    {
        List<List<Cell>> CellNeighborGroup = new List<List<Cell>>();
        int[,] Direction = IsCellEven(cellx, BoardWidth);

        for (int i = 0; i < Direction.GetLength(0); i++)
            if (IsInsideGrid(i,cellx,celly,BoardWidth,BoardHeight,Direction))
            {
                CellNeighborGroup.Add(new List<Cell>());
                CellNeighborGroup[CellNeighborGroup.Count - 1].Add(Cells[cellx + Direction[i, 0], celly + Direction[i, 1]]);
                if (IsInsideGrid(i+1,cellx,celly,BoardWidth,BoardHeight,Direction))
                {
                    CellNeighborGroup[CellNeighborGroup.Count - 1].Add(Cells[cellx + Direction[(i + 1) % Direction.GetLength(0), 0], celly + Direction[(i + 1) % Direction.GetLength(0), 1]]);
                }
                else
                {
                    CellNeighborGroup.RemoveAt(CellNeighborGroup.Count - 1);
                }
            }
        return CellNeighborGroup;
    }

    public static float CellXAxisMapping(float HexagonScale) //for placing cells
    {
        return HexagonScale * 6.2f;
    }

    public static float CellYAxisMapping(float HexagonScale)
    {
        return HexagonScale * 7f;
    }

    public static bool DirectionOfSelectedGroupObject(Cell[] group) //direction of selected group object depending on selected cells
    {
        for (int i = 0; i < group.Length; i++)
        {
            if (group[i].transform.position.x == group[(i + 1) % group.Length].transform.position.x)
                if (group[i].transform.position.x < group[(i + 2) % group.Length].transform.position.x)
                    return true;
        }
        return false;
    }
}
