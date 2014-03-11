using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public struct Block
{
    public BlockType blockType;
    public Block(BlockType blocktype)
    {
        blockType = blocktype;
    }
}

public enum BlockSide : byte
{
    Top = 0,
    Side = 1,
    Bottom = 2
}

public enum BlockType : byte
{
    Air = 0,
    Grass = 1,
    Dirt = 3,
    Stone = 6
}