using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileView : MonoBehaviour {

    Tile tile;

    void Start()
    {
    }

    public void ConstruyeCasilla(Tile tile)
    {
        this.tile = tile;
    }

    public void OnMouseDown()
    {
        //Se coloca el cadaver cuando no hay casa
        if (GameManager.instance.Estado == Estado.COLOCACADAVER && !GameManager.instance.HayCasa(tile.pos))      
            GameManager.instance.ColocaCadaver(tile.pos); 
        
        //Se coloca el agujero cuando no hay cadaver ni arma
        else if (GameManager.instance.Estado == Estado.COLOCAAGUJERO && !GameManager.instance.HayCasa(tile.pos) && !tile.Cadaver && !tile.Arma)        
            GameManager.instance.ColocaAgujero(tile.pos);
        
    }

}
