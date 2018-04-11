
/// <summary>
/// Representación lógica de las casillas. Guardan toda la información contenida en ella
/// </summary>
public class Tile
{  
    public bool Barro { get; set; }
    public bool Agujero { get; set; }
    public bool Sangre { get; set; }

    public bool Cadaver { get; set; }
    public bool Arma { get; set; }

    public Pos Pos { get; set; }

    public Tile(Pos pos)
    {
        Pos = pos;
        Barro = Agujero = Sangre = Cadaver = Arma = false;
    }
}

