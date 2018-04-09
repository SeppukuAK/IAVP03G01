using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//--------TIPOS---------
public enum Terreno {TIERRA, BARRO, AGUJERO, SANGRE,SANGREBARRO, NULL};
public enum Estado { COLOCACADAVER, COLOCAAGUJERO, PAUSA, PLAY};

public enum EstadoAgente { EXPLORASEGURO, EXPLORARIESGO,BUSCACUERPO, BUSCAARMA, BUSCACRIMEN, PAUSA };

//public enum Direccion { ARRIBA, ABAJO, IZQUIERDA, DERECHA, IDLE };

public class Pos
{
    public int x { get; set; }
    public int y { get; set; }

    public Pos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

	public override string ToString ()
	{
		return string.Format (x.ToString() + ":" + y.ToString());
	}

    public override bool Equals(object obj)
    {
        var pos = obj as Pos;
        return pos != null &&
               x == pos.x &&
               y == pos.y;
    }

    public override int GetHashCode()
    {
        var hashCode = 1502939027;
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }

    public int ManhattanDistance (Pos pos)
    {
        return (Math.Abs(pos.x - x) + Math.Abs(pos.y - y));
    }

}

//--------TIPOS---------
