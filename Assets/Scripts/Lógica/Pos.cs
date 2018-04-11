using System;

/// <summary>
/// Clase que representa una posición 2D. Proporciona método auxiliares
/// </summary>
public class Pos
{
    public int X { get; set; }
    public int Y { get; set; }

    public Pos(int x, int y)
    {
        X = x;
        Y = y;
    }

    //Distancia de un punto a otro. Solo direcciones cardinales
    public int ManhattanDistance(Pos pos)
    {
        return (Math.Abs(pos.X - X) + Math.Abs(pos.Y - Y));
    }

    public override string ToString()
    {
        return string.Format(X.ToString() + ":" + Y.ToString());
    }

    public override bool Equals(object obj)
    {
        var pos = obj as Pos;
        return pos != null &&
               X == pos.X &&
               Y == pos.Y;
    }

    public override int GetHashCode()
    {
        var hashCode = 1502939027;
        hashCode = hashCode * -1521134295 + X.GetHashCode();
        hashCode = hashCode * -1521134295 + Y.GetHashCode();
        return hashCode;
    }

}

//--------TIPOS---------
