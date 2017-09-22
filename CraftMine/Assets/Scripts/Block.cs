using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {

    private bool isTransparent;

    public static Block Air = new Block(true);
    public static Block Dirt = new Block(false);
    public static Block Stone = new Block(false);

    public Block(bool isTransparent) {
        this.isTransparent = isTransparent;
    }

}
