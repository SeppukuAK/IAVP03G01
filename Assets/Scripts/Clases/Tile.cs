using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Tile
{
    public Terreno terreno { get; set; }
    public Pos pos { get; set; }

    public Tile()
    {
        pos = new Pos(0, 0);
        terreno = Terreno.NULL;
    }

    public Tile(Terreno terreno, Pos pos)
    {
        this.terreno = terreno;
        this.pos = pos;
    }
}

