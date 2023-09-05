using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPosition
{
    public int x;
    public int z;

    public GridPosition(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public override string ToString()
    {
        return $"x: {x}, z: {z}";
    }

    public bool Equals(GridPosition other)
    {
        return x == other.x && z == other.z;
    }

    //TODO: Create an override for +, so we can Vector2 + GridPosition. Look at codemonkey code.
}
