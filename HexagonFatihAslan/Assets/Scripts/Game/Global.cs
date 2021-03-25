using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    public static int[,] IsCellEven(int x) //to determine if next cell upright or downright
    {
        if(x%2==0)
            return new int[6,2] {{0,1},{1,1},{1,0},{0,-1},{-1,0},{-1,1}};
        else
            return new int[6,2] {{0,1},{1,0},{1,-1},{0,-1},{-1,-1},{-1,0}};
    }
}
