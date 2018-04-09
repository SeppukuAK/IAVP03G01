using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Tile
{  
    public bool Barro { get; set; }
    public bool Agujero { get; set; }
    public bool Sangre { get; set; }


    public bool Cadaver { get; set; }
    public bool Arma { get; set; }

    public Pos pos { get; set; }

    public Tile()
    {
        pos = new Pos(0, 0);
        Barro = Agujero = Sangre = Cadaver = Arma = false;
    }

    public Tile(Pos pos)
    {
        Barro = Agujero = Sangre = Cadaver = Arma = false;
        this.pos = pos;
    }
}

