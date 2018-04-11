using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Estado de búsqueda del agente
/// </summary>
public enum EstadoAgente { BUSQUEDA, BUSCAARMA, BUSCACUERPO, BUSQUEDATERMINADA,MUERTO };

/// <summary>
/// Guarda la percepción del agente del mundo. Actua como un agente inteligente y siempre avanza a la mejor posición
/// </summary>
public class IAAgente
{
    /// <summary>
    /// Nodo que guarda un par Pos,Coste, para utilizar en la lista de frontera y obtener siempre el mejor nodo
    /// </summary>
    class NodoAgente
    {
        public Pos Pos { get; set; }
        public int Coste { get; set; }

        public NodoAgente(Pos pos, int coste)
        {
            Pos = pos;
            Coste = coste;
        }

        /// <summary>
        /// Compara dos nodos unicamente por sus posiciones
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var conocimiento = obj as NodoAgente;
            return conocimiento != null &&
                   EqualityComparer<Pos>.Default.Equals(Pos, conocimiento.Pos);
        }

        public override int GetHashCode()
        {
            var hashCode = 857480600;
            hashCode = hashCode * -1521134295 + EqualityComparer<Pos>.Default.GetHashCode(Pos);
            hashCode = hashCode * -1521134295 + Coste.GetHashCode();
            return hashCode;
        }
    }

    public TipoPercepcion[,] MatrizPercepcion { get; set; }

    List<NodoAgente> frontera;

    public EstadoAgente Estado { get; set; }

    public IAAgente(Tile tileIni)
    {
        MatrizPercepcion = new TipoPercepcion[GameManager.ALTO, GameManager.ANCHO];

        for (int i = 0; i < GameManager.ALTO; i++)
        {
            for (int j = 0; j < GameManager.ANCHO; j++)
            {
                MatrizPercepcion[i, j] = TipoPercepcion.NOEXPLORADO;
            }
        }

        Estado = EstadoAgente.BUSQUEDA;

        frontera = new List<NodoAgente>();

        ActualizaConocimiento(tileIni);
    }

    /// <summary>
    /// Actualiza la frontera y la matriz de percepción y comprueba si tiene que cambiar de estado
    /// Informa al GameManager de que renderice el nuevo sprite explorado
    /// Es llamado desde Detective,cuando se le dice que avance un paso
    /// </summary>
    /// <param name="tile"></param>
    public void ActualizaConocimiento(Tile tile)
    {
        //Cambios de estado
        if (tile.Arma)
        {
            if (Estado == EstadoAgente.BUSCAARMA)
                Estado = EstadoAgente.BUSQUEDATERMINADA;
            else
                Estado = EstadoAgente.BUSCACUERPO;

        }

        else if (tile.Cadaver)
        {
            if (Estado == EstadoAgente.BUSCACUERPO)
                Estado = EstadoAgente.BUSQUEDATERMINADA;
            else
                Estado = EstadoAgente.BUSCAARMA;

        }

        else if (tile.Agujero)
        {
            //Muerte
            Estado = EstadoAgente.MUERTO;
        }

        MatrizPercepcion[tile.Pos.Y, tile.Pos.X] = TipoPercepcion.EXPLORADO;

        ActualizaFrontera(tile);
    }

    /// <summary>
    /// Llama a ActualizarPercepcion de todas las casillas adyacentes
    /// </summary>
    /// <param name="tile"></param>
    void ActualizaFrontera(Tile tile)
    {
        //Actualizo a la izquierda
        if (tile.Pos.X > 0 )
            ActualizaPercepcion(tile, tile.Pos.X -1, tile.Pos.Y);

        //Actualizo a la derecha
        if (tile.Pos.X < GameManager.ANCHO - 1)
            ActualizaPercepcion(tile, tile.Pos.X + 1, tile.Pos.Y);

        //Actualizo arriba
        if (tile.Pos.Y > 0)
            ActualizaPercepcion(tile, tile.Pos.X, tile.Pos.Y - 1);

        //Actualizo abajo
        if (tile.Pos.Y < GameManager.ALTO - 1)
            ActualizaPercepcion(tile, tile.Pos.X, tile.Pos.Y + 1);

    }

    /// <summary>
    /// Actualiza la percepción de un tile
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    //
    void ActualizaPercepcion(Tile tile, int x, int y)
    {
        //Si la casilla no ha sido explorada
        if (MatrizPercepcion[y, x] != TipoPercepcion.EXPLORADO)
        {
            //Si la casilla ya es prioritaria, no hay nada mejor a ser prioritaria
            if (MatrizPercepcion[y, x] != TipoPercepcion.PRIORITARIO)
            {
                NodoAgente nodoAgente = new NodoAgente(new Pos(x, y),0);
                //Todos los adyacentes son prioritarios seguros si no hay barro y estoy en sangre, en cadaver o en arma
                if (!tile.Barro && (tile.Sangre || tile.Cadaver|| tile.Arma))
                {
                    MatrizPercepcion[y, x] = TipoPercepcion.PRIORITARIO;
                    if (frontera.Contains(nodoAgente))
                        frontera.Remove(nodoAgente);

                    nodoAgente.Coste = 1;
                     frontera.Add(nodoAgente);
                }

                //Si es segura, ya no me interesa saber más
                else if (MatrizPercepcion[y, x] != TipoPercepcion.SEGURO)
                {
                    //Si es tierra vacia
                    if (!tile.Barro && !tile.Sangre)
                    {
                        MatrizPercepcion[y, x] = TipoPercepcion.SEGURO;

                        if (frontera.Contains(nodoAgente))
                            frontera.Remove(nodoAgente);
                        nodoAgente.Coste = 10;
                        frontera.Add(nodoAgente);
                    }

                    //Si es riesgoPrioritario, no me interesa saber si me he encontrado un riesgoPrioritario o riesgo
                    else if (MatrizPercepcion[y, x] != TipoPercepcion.RIESGOPRIORITARIO)
                    {
                        if (tile.Barro && (tile.Sangre || tile.Cadaver || tile.Arma))
                        {
                            MatrizPercepcion[y, x] = TipoPercepcion.RIESGOPRIORITARIO;
  
                            if (frontera.Contains(nodoAgente))
                                frontera.Remove(nodoAgente);

                            nodoAgente.Coste = 100;
                            frontera.Add(nodoAgente);

                        }

                        //Ya está metido como Riesgo, no me interesa volver a meterlo
                        else if (MatrizPercepcion[y, x] != TipoPercepcion.RIESGO)
                        {
                            MatrizPercepcion[y, x] = TipoPercepcion.RIESGO;

                            if (frontera.Contains(nodoAgente))
                                frontera.Remove(nodoAgente);
                            nodoAgente.Coste = 200;
                            frontera.Add(nodoAgente);
                        }

                    }
                }
            }

        }

    }

    /// <summary>
    /// Devuelve la siguiente mejor posición
    /// </summary>
    /// <param name="actualPos"></param>
    /// <returns></returns>
    public Pos NextBestPos(Pos actualPos)
    {
        NodoAgente nearest = nearest = frontera
            .OrderBy(t => t.Pos.ManhattanDistance(actualPos) + t.Coste)
            .FirstOrDefault();
            frontera.Remove(nearest);

        return nearest.Pos;
    }

}
