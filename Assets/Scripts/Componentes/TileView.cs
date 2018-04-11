using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Componente asociado a los gameObject Tile, guarda la representación lógica de estos e informa al GameManager cuando son pulsados con el ratón
/// </summary>
public class TileView : MonoBehaviour {

    //Lógica del tile
    private Tile tile;

    /// <summary>
    /// Constructora que asocia la lógica del tile al gameObject
    /// </summary>
    /// <param name="tile"></param>
    public void ConstruyeCasilla(Tile tile)
    {
        this.tile = tile;
    }

    private void OnMouseDown()
    {
        //Si no está la casa en el tile clickado
        if (!GameManager.instance.PosCasa.Equals(tile.Pos))
        {
            //Se coloca el cadaver si estoy en el estado de colocar cadaver
            if (GameManager.instance.Estado == EstadoEscena.COLOCACADAVER)
                GameManager.instance.ColocaCadaver(tile.Pos);

            //Se coloca el agujero cuando estoy en el estado coloca agujero y no hay cadaver ni arma
            else if (GameManager.instance.Estado == EstadoEscena.COLOCAAGUJERO && !tile.Cadaver && !tile.Arma)
                GameManager.instance.ColocaAgujero(tile.Pos);
        }
    }

}
