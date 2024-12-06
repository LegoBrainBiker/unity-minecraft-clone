using System;
using System.Collections.Generic;

public class Block {
    public readonly bool[] trasparency;
    public readonly int[] textureMapings;
    public static readonly Block air = new Block(new int[]{-1,-1,-1,-1,-1,-1}, new bool[]{true,true,true,true,true,true});
    public static readonly Block dirt = new Block(new int[]{3, 3, 3, 3, 3, 3}, new bool[]{false,false,false,false,false,false});
    public static readonly Block stone = new Block(new int[]{6, 6, 6, 6, 6, 6}, new bool[]{false,false,false,false,false,false});
    public static readonly Block grassBlock = new Block(new int[]{0, 3, 2, 2, 2, 2}, new bool[]{false,false,false,false,false,false});
    public static readonly Block cobbleStone = new Block(new int[]{7, 7, 7, 7, 7, 7}, new bool[]{false,false,false,false,false,false});
    public static readonly Block log = new Block(new int[]{4, 4, 5, 5, 5, 5}, new bool[]{false,false,false,false,false,false});
    public static readonly Block planks = new Block(new int[]{8, 8, 8, 8, 8, 8}, new bool[]{false,false,false,false,false,false});
    public static readonly Block leaves = new Block(new int[]{1, 1, 1, 1, 1, 1}, new bool[]{false,false,false,false,false,false});

    public Block(int[] faces, bool[] isTransparent) {
        textureMapings = faces;
        trasparency = isTransparent;
    }
}