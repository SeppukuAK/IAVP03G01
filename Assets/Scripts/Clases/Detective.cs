using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detective
{
    public Pos pos { get; set; }

    ConocimientoAgente conocimientoAgente;

    public Detective(Tile tile)
    {
        this.pos = tile.pos;
        conocimientoAgente = new ConocimientoAgente(tile);
    }

    public void  AvanzaAPos()
    {
        Pos nextPos = conocimientoAgente.NextBestPos(pos);

        conocimientoAgente.ActualizaConocimiento(GameManager.instance.GetTile(nextPos));

        //SE HACE EL A ESTRELLA HASTA QUE LLEGUE A LA POS
        AEstrella A = new AEstrella(conocimientoAgente.Matriz, pos, nextPos);

        GameManager.instance.MoverAgente(A.GetCamino());
    }

    public void VuelveACasa()
    {
        //SE HACE EL A ESTRELLA HASTA QUE LLEGUE A LA CASA
        AEstrella Acasa = new AEstrella(conocimientoAgente.Matriz, pos, GameManager.instance.PosCasa);

        GameManager.instance.MoverAgente(Acasa.GetCamino());
    }

    public bool ObjetivoCumplido()
    {
        return conocimientoAgente.ObjetivoCumplido;
    }

}
