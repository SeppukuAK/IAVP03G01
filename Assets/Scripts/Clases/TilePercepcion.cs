using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TipoPercepcion { EXPLORADO, PRIORITARIO, SEGURO, RIESGOPRIORITARIO,RIESGO , NOEXPLORADO }

public class TilePercepcion {

    public TilePercepcion()
    {
        Barro = Sangre = Arma = Cuerpo = false;
        Percepcion = TipoPercepcion.NOEXPLORADO;
    }

    public bool Barro { get; set; }
    public bool Sangre { get; set; }

    public bool Arma { get; set; }
    public bool Cuerpo { get; set; }

    public TipoPercepcion Percepcion { get; set; }

}
