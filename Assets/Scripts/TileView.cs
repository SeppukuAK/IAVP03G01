using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileView : MonoBehaviour {

    Tile tile;

    // Use this for initialization
    void Start()
    {
    }

    public void ConstruyeCasilla(Tile tile)
    {
        this.tile = tile;
    }

}
