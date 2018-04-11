
/// <summary>
/// Representación lógica del Agente
/// </summary>
public class Agente
{
    public Pos Pos { get; set; }

    private IAAgente IAAgente;

    public Agente(Tile tile)
    {
        Pos = tile.Pos;
        IAAgente = new IAAgente(tile);
    }
    
    /// <summary>
    /// Avanza a la siguiente mejor posición. Hace un A* e informa al GameManager de que haga la corrutina
    /// Es llamado desde el GameManager, repetidas veces cuando esta en el estado PLAY
    /// </summary>
    public void  Avanza()
    {
        //Siguiente mejor posición
        Pos nextPos = IAAgente.NextBestPos(Pos);

        //Actualiza el conocimiento del agente
        IAAgente.ActualizaConocimiento(GameManager.instance.GetTile(nextPos));

        //A* hasta la nueva pos
        AEstrella A = new AEstrella(IAAgente.MatrizPercepcion, Pos, nextPos);

        //Informamos al GameManager de que empiece la corrutina
        GameManager.instance.MoverAgente(A.Camino);
    }

    /// <summary>
    /// Hace un A* a la casa e informa al gameManager de que empiece la corrutina
    /// Es llamado desde GameManager, cuando se ha completado la busqueda
    /// </summary>
    public void VuelveACasa()
    {
        //A* hasta la nueva pos
        AEstrella Acasa = new AEstrella(IAAgente.MatrizPercepcion, Pos, GameManager.instance.PosCasa);

        //Informamos al GameManager de que empiece la corrutina
        GameManager.instance.MoverAgente(Acasa.Camino);
    }

    /// <summary>
    /// Devuelve si el agente a completado la busqueda
    /// </summary>
    /// <returns></returns>
    public bool ObjetivoCumplido()
    {
        return IAAgente.Estado == EstadoAgente.BUSQUEDATERMINADA;
    }

    public bool AgenteMuerto()
    {
        return IAAgente.Estado == EstadoAgente.MUERTO;
    }

    public bool ArmaEncontrada()
    {
        return (IAAgente.Estado == EstadoAgente.BUSCACUERPO || IAAgente.Estado == EstadoAgente.BUSQUEDATERMINADA);
    }

    public bool CuerpoEncontrado()
    {
        return (IAAgente.Estado == EstadoAgente.BUSCAARMA || IAAgente.Estado == EstadoAgente.BUSQUEDATERMINADA);
    }
}
