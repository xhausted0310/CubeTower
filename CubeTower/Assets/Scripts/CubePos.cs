using System;
using UnityEngine;

struct CubePos
{
    public int x;
    public int y;
    public int z;

    public CubePos(int X, int Y, int Z)
    {
        x = X;
        y = Y;
        z = Z;
    }

    public Vector3 GetVector()
    {
        return new Vector3(x, y, z);
    }

    public void SetVector(Vector3 pos)
    {
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
}