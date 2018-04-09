using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConocimientoAgente
{
    EstadoAgente Estado { get; set; }

    public TilePercepcion[,] Matriz { get; set; }

    //Listas por orden de prioridad
    List<Pos> fronteraPrio;
    List<Pos> fronteraSegura;
    List<Pos> fronteraRiesgoPrio;
    List<Pos> fronteraRiesgo;

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

        fronteraPrio = new List<Pos>();
        fronteraSegura = new List<Pos>();
        fronteraRiesgoPrio = new List<Pos>();
        fronteraRiesgo = new List<Pos>();

        ActualizaConocimiento(tileIni);

        //COMPROBAR SI PASO A BUSCAARMA,BUSCACUERPO O BUSCA CRIMEN
        if (fronteraPrio.Count > 0)
            Estado = EstadoAgente.BUSCAARMA;

        else if (fronteraSegura.Count > 0)
            Estado = EstadoAgente.EXPLORASEGURO;
        else if (fronteraRiesgo.Count > 0)
            Estado = EstadoAgente.EXPLORARIESGO;

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
                //Todos los adyacentes son prioritarios seguros si no hay barro y estoy en sangre, en cadaver o en arma
                if (!tile.Barro && (tile.Sangre || tile.Cadaver|| tile.Arma))
                {
                    Matriz[y, x].Percepcion = TipoPercepcion.PRIORITARIO;
                    fronteraPrio.Add(new Pos(x, y));

                }

                //Si es segura, ya no me interesa saber más
                else if (Matriz[y, x].Percepcion != TipoPercepcion.SEGURO)
                {
                    //Si es tierra vacia
                    if (!tile.Barro && !tile.Sangre)
                    {
                        Matriz[y, x].Percepcion = TipoPercepcion.SEGURO;
                        fronteraSegura.Add(new Pos(x, y));
                    }

                    else if (Matriz[y, x].Percepcion != TipoPercepcion.RIESGOPRIORITARIO)
                    {
                        if (tile.Barro && (tile.Sangre || tile.Cadaver || tile.Arma))
                        {
                            Matriz[y, x].Percepcion = TipoPercepcion.RIESGOPRIORITARIO;
                            fronteraRiesgoPrio.Add(new Pos(x, y));
                       
                        }

                        //Ya está metido como Riesgo, no me interesa volver a meterlo
                        else if (Matriz[y, x].Percepcion != TipoPercepcion.RIESGO)
                        {
                            Matriz[y, x].Percepcion = TipoPercepcion.RIESGOPRIORITARIO;
                            fronteraRiesgo.Add(new Pos(x, y));
                        }

                    }
                }
            }

        }

    }

    //Devuelve la siguiente mejor posición, teniendo en cuenta el estado en el que se encuentra
    public Pos NextBestPos(Pos actualPos)
    {
        Pos nearest;

        if (fronteraPrio.Count > 0)
        {
            nearest = fronteraPrio
            .OrderBy(t => t.ManhattanDistance(actualPos))
            .FirstOrDefault();
            fronteraPrio.Remove(nearest);

        }

        else if (fronteraSegura.Count > 0)
        {
            nearest = fronteraSegura
            .OrderBy(t => t.ManhattanDistance(actualPos))
            .FirstOrDefault();
            fronteraSegura.Remove(nearest);

        }

        else if (fronteraRiesgoPrio.Count > 0)
        {
            nearest = fronteraRiesgoPrio
            .OrderBy(t => t.ManhattanDistance(actualPos))
            .FirstOrDefault();
            fronteraRiesgoPrio.Remove(nearest);
        }

        else
        {
            nearest = fronteraRiesgo
            .OrderBy(t => t.ManhattanDistance(actualPos))
            .FirstOrDefault();
            fronteraRiesgo.Remove(nearest);
        }

        return nearest;
    }

}
