using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class LogicaTile
{
    Terreno _terreno;
    Pos _pos;

	public LogicaTile()
    {
        _pos = new Pos(0, 0);
        _terreno = Terreno.vacio;
    }

	public LogicaTile(Terreno terreno, Pos pos)
    {
        _terreno = terreno;
        _pos = pos;
    }

    public Terreno GetTerreno() { return _terreno; }
    public void SetTerreno(Terreno terreno) { _terreno = terreno; }

    public Pos GetPos() { return _pos; }

}

