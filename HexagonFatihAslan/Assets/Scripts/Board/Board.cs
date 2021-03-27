using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{

    public AudioSource MatchSound;

    public int Width = 8;    //I didnt use hexagon tilemap, because when you use TileMap.SetTile() and assign a gameobject to that tile,
    public int Height = 9;   //all of tiles using same gameobject and also same cell script (same values in all of them). You can prevent this by cloning that object but then you will have two cell
    public int SpawnBombLimit = 1000;   //new bomb for every 1000 score
    public float CellSize = 0.1f;

    [SerializeField]
    GameObject CellObject, CenterSphereObj, HintObj;
    GameObject TempCell, TempCenterObj;
    List<GameObject> HintObjects = new List<GameObject>();

    Cell[,] Cells;
    Cell[] SelectedCells = new Cell[3]; //selected group
    List<Cell> Matches = new List<Cell>();  //matched cells

    bool Rotating = false;  //is group rotating
    bool BoardChecked = false;
    bool MatchDestroy = false;
    bool HintShowed = false;
    float HintTimer = 2f;
    float MatchDestroyDelay;
    float RotateDuration = 0.9f;    //rotate duration of selected group
    float TempRotateDuration;
    float RotateAngle = 360f;
    int RotateCount = 0;

    void Start()
    {
        CellObject.transform.localScale = new Vector3(CellSize, CellSize, CellSize);
        CenterSphereObj.transform.localScale = CellObject.transform.localScale * 3.20f; //can modify cell size without selectedobjects fits problem
        Cells = new Cell[Width, Height];    //[X,Y]
        AssignCells();
        SetupCells();
    }

    void AssignCells()
    {
        for (int h = -Height / 2; h < Height - Height / 2; h++)
        {
            for (int w = -Width / 2; w < Width - Width / 2; w++)
            {
                TempCell = Instantiate(CellObject, new Vector3((float)w * Global.CellXAxisMapping(CellObject.transform.localScale.x), (float)h * Global.CellYAxisMapping(CellObject.transform.localScale.y), 0), Quaternion.identity);
                if (Math.Abs(w) % 2 == (Width / 2) % 2) //same pattern for all boards with different size
                    TempCell.transform.position = new Vector3(TempCell.transform.position.x, TempCell.transform.position.y + Global.CellYAxisMapping(CellObject.transform.localScale.y) / 2, 0);
                TempCell.transform.SetParent(transform);
                Cells[w + Width / 2, h + Height / 2] = TempCell.GetComponent<Cell>();
            }
        }
    }

    void SetupCells()
    {
        for (int h = 0; h < Height; h++)
        {
            for (int w = 0; w < Width; w++)
            {
                Cells[w, h].NeighBorGroupCells = Global.GetNeighborGroupCells(w, h, Width, Height, Cells);
                Cells[w, h].Setup(w, h);
                Cells[w, h].UpperCell = (h + 1 < Height) ? Cells[w, h + 1] : null;  //assign upper cell if there is any
            }
        }
    }

    bool BoardReady()   //check every cell if their items are ready
    {
        for (int h = 0; h < Height; h++)
        {
            for (int w = 0; w < Width; w++)
            {
                if (!Cells[w, h].CurrentItem)
                {
                    BoardChecked = false;
                    return false;
                }
            }
        }
        return true;
    }

    void IsGameOver()
    {
        foreach (Cell c in Cells)   //checking every cells in board
        {
            foreach (List<Cell> group in c.NeighBorGroupCells)  //checking that cell's groups
            {
                foreach (Cell groupscell in group)    //checking that group's cells
                {
                    foreach (List<Cell> neighborcellsgroup in groupscell.NeighBorGroupCells) //and than checking this cell's groups
                    {
                        int groupcount = 0;
                        foreach (Cell neighborcells in neighborcellsgroup)  //now if there are two color which is same with current cells color which we are looking for right now (c) than there is an avaible match
                        {
                            if (neighborcells.CurrentItemColor != c.CurrentItemColor || neighborcells == c) //two color?
                                break;

                            groupcount++;
                            if (groupcount == neighborcellsgroup.Count)     //if yes than return or game will end
                            {
                                if (Global.Hints)
                                {
                                    HintObjects.Add(Instantiate(HintObj, neighborcells.transform.position, Quaternion.identity)); //this was a test for me to see which items can match in board
                                    HintObjects.Add(Instantiate(HintObj, c.transform.position, Quaternion.identity));
                                }
                                return; //remove this line for more than two hints
                            }
                        }
                    }
                }
            }
        }
        if (HintObjects.Count == 0)
            Global.GameOver = true;
    }

    void RotateGroup(int direction)
    {
        int dir = direction;
        GameObject temp;
        for (int i = 0; i < SelectedCells.Length; i++)  //rotate items inside selected group with given direction
        {
            temp = SelectedCells[0].CurrentItem;
            SelectedCells[0].SetItem(SelectedCells[(i + dir + SelectedCells.Length) % SelectedCells.Length].CurrentItem, false);
            SelectedCells[(i + dir + SelectedCells.Length) % SelectedCells.Length].SetItem(temp, false);
        }
        CheckBoard(false);  //check only selected group's neighbors for matches
    }

    void CheckBoard(bool All)   //Check board for matches, can send Cells parameters
    {
        Cell[] CellsToCheck;    //decide to check all cells or just selectedgroup
        if (All)
        {
            CellsToCheck = new Cell[Height * Width];
            int i = 0;
            for (int h = 0; h < Height; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    CellsToCheck[i++] = Cells[w, h];
                }
            }
        }
        else
            CellsToCheck = SelectedCells;

        bool Matched = false;
        Matches = new List<Cell>();
        foreach (Cell cell in CellsToCheck)
        {
            foreach (List<Cell> group in cell.NeighBorGroupCells)   //get groups of cell
            {
                int groupcount = 0;
                foreach (Cell groupscell in group)  //than check if all colors are same with cell's in group
                {
                    if (groupscell.CurrentItemColor != cell.CurrentItemColor)
                        break;
                    groupcount++;
                    if (groupcount == group.Count)  //!Match
                    {
                        if (!Matches.Contains(cell))
                            Matches.Add(cell);
                        foreach (Cell matchedcell in group)
                            if (!Matches.Contains(matchedcell))
                                Matches.Add(matchedcell);
                        Matched = true;
                    }
                }
            }
        }
        if (Matched)
        {
            Matched = false;
            Rotating = false;
            MatchDestroyDelay = 0.5f;
            if (!All)
                Global.Move++;  //we matched selected group not all board
        }
    }

    void ClearSelectedGroup()
    {
        SelectedCells = new Cell[3];
        Destroy(TempCenterObj);
    }

    void RemoveHints()  //disable hints
    {
        HintTimer = 5f;
        for (int i = 0; i < HintObjects.Count; i++)
            Destroy(HintObjects[i]);
        HintObjects = new List<GameObject>();
        HintShowed = false;
    }

    void Update()
    {
        HintTimer -= Time.deltaTime;
        if (HintTimer < 0 && !HintShowed)   //enable hints
        {
            foreach (GameObject h in HintObjects)
                h.SetActive(true);
            HintShowed = true;
        }

        if (MatchDestroyDelay > 0)  //matched object's destroyment delay
        {
            MatchDestroy = true;
            MatchDestroyDelay -= Time.deltaTime;
        }
        else if (MatchDestroy)
        {
            MatchDestroy = false;
            MatchSound.Play();      //sound
            foreach (Cell c in Matches)
                c.DestroyCurrentItem();
            RemoveHints();
        }

        if (BoardReady())
        {
            if (!BoardChecked)
            {
                BoardChecked = true;
                CheckBoard(true);   //check whole board for any matches
                IsGameOver();
            }
        }
        
        if (Global.Slide)   //slide
        {
            Global.Slide = false;
            if (TempCenterObj)
            {
                Rotating = true;
                TempRotateDuration = RotateDuration / 3;    //rotate 3 times to find any match
                RotateCount = 0;
            }
        }

        if (Rotating)
        {
            if (!Global.RotateClockWise)
                RotateAngle = 360f;
            else
                RotateAngle = -360f;
            TempRotateDuration -= Time.deltaTime;

            if (TempRotateDuration <= 0)
            {
                RotateCount++;
                if (RotateCount <= 3)
                {
                    TempRotateDuration = RotateDuration / 3;
                    RotateGroup(Global.RotateClockWise ? 1 : -1);   //rotate and find matches
                }
                else
                {
                    Rotating = false;
                }
            }
            if (Rotating)
            {
                TempCenterObj.transform.RotateAround(TempCenterObj.transform.position, Vector3.forward, RotateAngle * Time.deltaTime / RotateDuration);
            }
            else
                TempCenterObj.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void CellPressed(int cellx, int celly, float hitx, float hity)
    {
        if (Rotating)
            return;
        RemoveHints();
        Vector3 hit = new Vector3(hitx, hity, 0);   //touch pos
        Vector3 Center = new Vector3(0, 0, 0);
        Vector3 tempCenter = new Vector3(0, 0, 0);
        float distant = 1000f;
        float tempdistant = 0f;

        foreach (List<Cell> group in Cells[cellx, celly].NeighBorGroupCells)    //check all neighbor groups
        {
            tempCenter = Cells[cellx, celly].transform.position;
            foreach (Cell cell in group)
            {
                tempCenter += cell.transform.position;
            }
            tempCenter /= group.Count + 1;
            tempdistant = Vector3.Distance(tempCenter, hit);
            if (tempdistant < distant)  //and find closest group depending on touch pos
            {
                ClearSelectedGroup();
                distant = tempdistant;
                for (int i = 0; i < group.Count + 1; i++)   //assign selected group
                    if (i == 0)
                        SelectedCells[i] = Cells[cellx, celly];
                    else
                        SelectedCells[i] = group[i - 1];

                if (Global.DirectionOfSelectedGroupObject(SelectedCells))   //direction
                {
                    Center = new Vector3(tempCenter.x + CellObject.transform.localScale.x, tempCenter.y, tempCenter.z - 1);
                    CenterSphereObj.GetComponent<SpriteRenderer>().flipX = false;
                }
                else
                {
                    Center = new Vector3(tempCenter.x - CellObject.transform.localScale.x, tempCenter.y, tempCenter.z - 1);
                    CenterSphereObj.GetComponent<SpriteRenderer>().flipX = true;
                }

                TempCenterObj = Instantiate(CenterSphereObj, Center, Quaternion.identity);
                Global.SelectedGroupObjectx = Center.x;     //for deciding if the rotation will be clockwise or otherwise
            }
        }
    }
}