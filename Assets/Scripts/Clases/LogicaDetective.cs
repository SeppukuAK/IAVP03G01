using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LogicaDetective
{
    Pos _pos;

    public LogicaDetective()
    {
        _pos = new Pos(0, 0);


    }

	public LogicaDetective(Pos pos)
    {
        _pos = pos;
       
    }


    public Pos GetPos() { return _pos; }
	public void SetPos(Pos pos) { _pos = pos; }

}

