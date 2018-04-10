using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

 

public class ConocimientoAgente
{
    class NodoConocimiento
    {
        public NodoConocimiento(Pos pos, int coste)
        {
            Pos = pos;
            Coste = coste;
        }

        public Pos Pos { get; set; }
        public int Coste {get; set;}

        public override bool Equals(object obj)
        {
            var conocimiento = obj as NodoConocimiento;
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
    EstadoAgente Estado { get; set; }

    public TilePercepcion[,] Matriz { get; set; }

    //Listas por orden de prioridad
    List<NodoConocimiento> frontera;

    bool cuerpoEncontrado;
    bool armaEncontrada;

    public ConocimientoAgente(Tile tileIni)
    {
        Matriz = new TilePercepcion[GameManager.ALTO, GameManager.ANCHO];

        for (int i = 0; i < GameManager.ALTO; i++)
        {
            for (int j = 0; j < GameManager.ANCHO; j++)
            {
                Matriz[i, j] = new TilePercepcion();
            }
        }

        cuerpoEncontrado = armaEncontrada = false;

        frontera = new List<NodoConocimiento>();

        ActualizaConocimiento(tileIni);

        /*
        //COMPROBAR SI PASO A BUSCAARMA,BUSCACUERPO O BUSCA CRIMEN
        if (fronteraPrio.Count > 0)
            Estado = EstadoAgente.BUSCAARMA;

        else if (fronteraSegura.Count > 0)
            Estado = EstadoAgente.EXPLORASEGURO;
        else if (fronteraRiesgo.Count > 0)
            Estado = EstadoAgente.EXPLORARIESGO;
            */
    }

    public void ActualizaConocimiento(Tile tile)
    {
        if (tile.Arma)
            armaEncontrada = true;

        else if (tile.Cadaver)
            cuerpoEncontrado = true;

        if (armaEncontrada && cuerpoEncontrado)
        {
            Debug.Log("HOLA");
        }

        
        Matriz[tile.pos.y, tile.pos.x].Percepcion = TipoPercepcion.EXPLORADO;

        //Este tile se renderiza en el GameManager
        GameManager.instance.Renderiza(tile.pos);
        ActualizaFrontera(tile);
    }

    //Llama a ActualizarPercepcion de todas las casillas adyacentes
    void ActualizaFrontera(Tile tile)
    {
        //Actualizo a la izquierda
        if (tile.pos.x > 0 )
        {
            ActualizaPercepcion(tile, tile.pos.x -1, tile.pos.y);
        }

        //Actualizo a la derecha
        if (tile.pos.x < GameManager.ANCHO - 1)
        {
            ActualizaPercepcion(tile, tile.pos.x + 1, tile.pos.y);

        }
        //Actualizo arriba
        if (tile.pos.y > 0)
        {
            ActualizaPercepcion(tile, tile.pos.x, tile.pos.y - 1);

        }

        //Actualizo abajo
        if (tile.pos.y < GameManager.ALTO - 1)
        {
            ActualizaPercepcion(tile, tile.pos.x, tile.pos.y + 1);

        }
    }

    //Actualiza la percepción de una casilla a segura, insegura o prioritaria
    void ActualizaPercepcion(Tile tile, int x, int y)
    {
        //Si la casilla no ha sido explorada
        if (Matriz[y, x].Percepcion != TipoPercepcion.EXPLORADO)
        {
            //Si la casilla ya es prioritaria, no hay nada mejor a ser prioritaria
            if (Matriz[y, x].Percepcion != TipoPercepcion.PRIORITARIO)
            {
                NodoConocimiento nodoConocimiento = new NodoConocimiento(new Pos(x, y),0);
                //Todos los adyacentes son prioritarios seguros si no hay barro y estoy en sangre, en cadaver o en arma
                if (!tile.Barro && (tile.Sangre || tile.Cadaver|| tile.Arma))
                {
                    Matriz[y, x].Percepcion = TipoPercepcion.PRIORITARIO;
                    if (frontera.Contains(nodoConocimiento))
                        frontera.Remove(nodoConocimiento);

                    nodoConocimiento.Coste = 1;
                     frontera.Add(nodoConocimiento);
                }

                //Si es segura, ya no me interesa saber más
                else if (Matriz[y, x].Percepcion != TipoPercepcion.SEGURO)
                {
                    //Si es tierra vacia
                    if (!tile.Barro && !tile.Sangre)
                    {

                        Matriz[y, x].Percepcion = TipoPercepcion.SEGURO;

                        if (frontera.Contains(nodoConocimiento))
                            frontera.Remove(nodoConocimiento);
                        nodoConocimiento.Coste = 10;
                        frontera.Add(nodoConocimiento);
                    }

                    else if (Matriz[y, x].Percepcion != TipoPercepcion.RIESGOPRIORITARIO)
                    {
                        if (tile.Barro && (tile.Sangre || tile.Cadaver || tile.Arma))
                        {

                            Matriz[y, x].Percepcion = TipoPercepcion.RIESGOPRIORITARIO;

                            if (frontera.Contains(nodoConocimiento))
                                frontera.Remove(nodoConocimiento);
                            nodoConocimiento.Coste = 100;
                            frontera.Add(nodoConocimiento);

                        }

                        //Ya está metido como Riesgo, no me interesa volver a meterlo
                        else if (Matriz[y, x].Percepcion != TipoPercepcion.RIESGO)
                        {
                            Matriz[y, x].Percepcion = TipoPercepcion.RIESGO;

                            if (frontera.Contains(nodoConocimiento))
                                frontera.Remove(nodoConocimiento);
                            nodoConocimiento.Coste = 200;
                            frontera.Add(nodoConocimiento);
                        }

                    }
                }
            }

        }

    }

    //Devuelve la siguiente mejor posición, teniendo en cuenta el estado en el que se encuentra
    public Pos NextBestPos(Pos actualPos)
    {
        NodoConocimiento nearest = nearest = frontera
            .OrderBy(t => t.Pos.ManhattanDistance(actualPos) + t.Coste)
            .FirstOrDefault();
            frontera.Remove(nearest);

        return nearest.Pos;
    }

}
