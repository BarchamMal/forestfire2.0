using System;

namespace forestfire;

public enum CellType : byte
{
    Charred = 0,
    Dirt = 1,
    Grass = 2,
    Brush = 3,
    Tree = 4,
    ThickTree = 5,
    Fire = 6,
}

public struct CellState
{
    public CellType Type;
    public byte BurnTime;
    public ForestFire? Fire;

    public CellState(CellType type, byte burnTime = 0, ForestFire? fire = null)
    {
        Type = type;
        BurnTime = burnTime;
        Fire = fire;
    }
}
